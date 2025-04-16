using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Jobs;

[BurstCompile]
public struct CalculatePathJob : IJobParallelForTransform
{
    [ReadOnly] public float timePass;
    [ReadOnly] public NativeArray<float3> targetPositions;
    [ReadOnly] public NativeArray<float3> sourcePositions;
    public void Execute(int index, TransformAccess transformInput)
    {
        //Calculate the displacement
        float3 finalDisplacement = targetPositions[index] - sourcePositions[index];

        float3 displacement = (0.514286f - timePass) * 15.555555f * timePass * finalDisplacement;

        transformInput.position = sourcePositions[index] + displacement;
    }
}

public class MovingBehavior : MonoBehaviour
{
    public static MovingBehavior Instance { get; private set; } = null;

    public bool IsTargetSet { get; private set; }

    readonly List<Vector3Int> _mergeTargetIndexes = new ();

    //Input system
    bool _isMoveKeyPressed = false;

    //Job system
    JobHandle _jobHandle;
    NativeList<float3> _sourcePositions;
    NativeList<float3> _targetPositions;
    TransformAccessArray _transformAccessArray;
    
    //Moving data
    float[] _movingDirection = new float[3];
    int _movingAxisIndex = new();
    bool _isPositive = new();
    Coroutine _currentWorkingCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        IsTargetSet = false;

        //Prepare containers
        _transformAccessArray = new TransformAccessArray(GlobalData.GridSizeCubic);
        _sourcePositions = new NativeList<float3>(Allocator.Persistent);
        _targetPositions = new NativeList<float3>(Allocator.Persistent);
    }

    private void Start()
    {
        //Input System
        EnvironmentSettings.InputManager.Player.Move.started -= MoveBlocks;
        EnvironmentSettings.InputManager.Player.Move.started += MoveBlocks;
    }

    private void LateUpdate()
    {
        _isMoveKeyPressed = false;
    }

    private void OnDestroy()
    {
        //Dispose containers
        _sourcePositions.Dispose();
        _targetPositions.Dispose();
        _transformAccessArray.Dispose();
    }

    #region Move block methods

    private void BreakMoving()
    {
        //Break moving
        if (_currentWorkingCoroutine != null) StopCoroutine(_currentWorkingCoroutine);
        _currentWorkingCoroutine = null;

        _jobHandle.Complete();

        //Set all the blocks to their target positions
        for (int i = 0; i < _transformAccessArray.length; i++)
        {
            _transformAccessArray[i].position = _targetPositions[i];
        }

        //Finish merging and summoning new blocks instantly
        SummonBehavior.Instance.SummonMergedBlocks(_mergeTargetIndexes, true);
        SummonBehavior.Instance.SummonNewBlocks(true);
        GameBehavior.Instance.RefreshLayerVisibility();

        //Clear old data
        _transformAccessArray.Dispose();
        _sourcePositions.Clear();
        _targetPositions.Clear();
        _transformAccessArray = new TransformAccessArray(GlobalData.GridSizeCubic);
    }

    private IEnumerator MoveToTarget()
    {
        float timePassed = 0f;

        while (timePassed < 0.3f)
        {
            //Set job data
            CalculatePathJob job = new ()
            {
                timePass = timePassed,
                sourcePositions = _sourcePositions.AsArray(),
                targetPositions = _targetPositions.AsArray()
            };

            _jobHandle = job.Schedule(_transformAccessArray);

            yield return new WaitUntil(() => _jobHandle.IsCompleted);

            _jobHandle.Complete();

            timePassed += Time.deltaTime;
        }

        //Finish movement
        for (int i = 0; i < _transformAccessArray.length; i++)
        {
            _transformAccessArray[i].position = _targetPositions[i];
        }

        //Start merging and summoning new blocks
        SummonBehavior.Instance .SummonMergedBlocks(_mergeTargetIndexes, false);
        SummonBehavior.Instance.SummonNewBlocks(false);
        GameBehavior.Instance.RefreshLayerVisibility();

        //Clear old data
        _transformAccessArray.Dispose();
        _sourcePositions.Clear();
        _targetPositions.Clear();
        _transformAccessArray = new TransformAccessArray(GlobalData.GridSizeCubic);

        IsTargetSet = false;
    }

    private List<int> FindChangeableTarget(List<int> selectBlockIndex3D, List<int> targetBlockIndex3D)
    {
        //Buffer to store the previous targetBlockIndex3D
        List<int> previousTargetBlockIndex3D = new(targetBlockIndex3D);

        //Move next the target block
        targetBlockIndex3D[_movingAxisIndex] += _isPositive ? 1 : -1;

        //Check if target block is out-of-bound, which means the selected block is at the edge
        if (targetBlockIndex3D[_movingAxisIndex] < 0 || targetBlockIndex3D[_movingAxisIndex] >= GlobalData.GridSize) return null;

        //Try move next the target block if the current block is empty in order to find the farthest block it "might" move to
        while (GlobalData.TransformsListGrid3D[targetBlockIndex3D[0], targetBlockIndex3D[1], targetBlockIndex3D[2]].Count == 0)
        {
            previousTargetBlockIndex3D = new (targetBlockIndex3D);
            targetBlockIndex3D[_movingAxisIndex] += _isPositive ? 1 : -1;

            //Check if next target block is out-of-bound, which means the previous target block is the farthest block it "could" move to
            if (targetBlockIndex3D[_movingAxisIndex] < 0 || targetBlockIndex3D[_movingAxisIndex] >= GlobalData.GridSize) return previousTargetBlockIndex3D;
        }

        //Check if the target block's value is same as the select block's
        List<Transform> selectTransformList = GlobalData.TransformsListGrid3D[selectBlockIndex3D[0], selectBlockIndex3D[1], selectBlockIndex3D[2]];
        List<Transform> targetTransformList = GlobalData.TransformsListGrid3D[targetBlockIndex3D[0], targetBlockIndex3D[1], targetBlockIndex3D[2]];
        int selectBlockValue = selectTransformList[0].GetComponent<BlockBehavior>().Value;
        int targetBlockValue = targetTransformList[0].GetComponent<BlockBehavior>().Value;

        if (selectBlockValue == targetBlockValue && targetTransformList.Count == 1)
        {
            _mergeTargetIndexes.Add(new Vector3Int(targetBlockIndex3D[0], targetBlockIndex3D[1], targetBlockIndex3D[2]));

            return targetBlockIndex3D;
        }

        //Check if the target block is the select block
        if (previousTargetBlockIndex3D.SequenceEqual(selectBlockIndex3D)) return null;

        //Exit
        return previousTargetBlockIndex3D;
    }

    private void SetTargetPerSlice(int sliceIndex, ref bool isMovable)
    {
        List<int> selectBlockIndex3D = new();
        List<int> targetBlockIndex3D = new();

        //Traverse every blocks in the slice
        for (int i = 0; i < GlobalData.GridSize; i++)
        {
            for (int j = 0; j < GlobalData.GridSize; j++)
            {
                //Select a block
                selectBlockIndex3D.Clear();
                selectBlockIndex3D.Add(i);
                selectBlockIndex3D.Add(j);
                selectBlockIndex3D.Insert(_movingAxisIndex, sliceIndex);

                //Check if the block exists
                List<Transform> blockTransformsList = GlobalData.TransformsListGrid3D[selectBlockIndex3D[0], selectBlockIndex3D[1], selectBlockIndex3D[2]];
                if (blockTransformsList.Count == 0) continue;

                //Set target block
                targetBlockIndex3D?.Clear();
                targetBlockIndex3D = new (selectBlockIndex3D);

                //Find changeable target block's 3d index
                targetBlockIndex3D = FindChangeableTarget(selectBlockIndex3D, targetBlockIndex3D);

                //Continue if the changeable block doesn't exist
                if (targetBlockIndex3D == null) continue;
                isMovable = true;

                //Upload transform
                Transform selectBlockTransform = blockTransformsList[0];
                GlobalData.TransformsListGrid3D[targetBlockIndex3D[0], targetBlockIndex3D[1], targetBlockIndex3D[2]].Add(selectBlockTransform);

                //Remove old transform
                blockTransformsList.RemoveAt(0);

                //Upload job data for each block
                _sourcePositions.Add(GlobalData.FixedPositionGrid[selectBlockIndex3D[0], selectBlockIndex3D[1], selectBlockIndex3D[2]]);
                _targetPositions.Add(GlobalData.FixedPositionGrid[targetBlockIndex3D[0], targetBlockIndex3D[1], targetBlockIndex3D[2]]);
                _transformAccessArray.Add(selectBlockTransform);
            }
        }
    }

    public void MoveBlocks(Vector3 value)
    {
        //Avoid multiple inputs in a frame
        if (_isMoveKeyPressed) return;
        _isMoveKeyPressed = true;

        //End previous movement
        if (IsTargetSet) BreakMoving();

        IsTargetSet = false;

        //Read input
        Vector3 moveValue = value;

        //Set moving data
        _movingDirection = new float[3] { moveValue.x, moveValue.y, moveValue.z };
        _movingAxisIndex = Array.FindIndex(_movingDirection, target => target == 1f || target == -1f);
        _isPositive = _movingDirection[_movingAxisIndex] > 0f;

        //Flag to check if any block is movable
        bool isMovable = false;

        //Find every blocks' target positions
        if (_isPositive)
        {
            for (int sliceIndex = GlobalData.GridSize - 1; sliceIndex >= 0; sliceIndex--)
            {
                SetTargetPerSlice(sliceIndex, ref isMovable);
            }
        }
        else
        {
            for (int sliceIndex = 0; sliceIndex < GlobalData.GridSize; sliceIndex++)
            {
                SetTargetPerSlice(sliceIndex, ref isMovable);
            }
        }

        //Cancel movement if all full
        if (!isMovable)
        {
            GameBehavior.Instance.OnGridFull?.Invoke();
            return;
        }

        IsTargetSet = true;

        //Start motion animation calculation coroutine
        _currentWorkingCoroutine = StartCoroutine(MoveToTarget());
    }

    public void MoveBlocks(InputAction.CallbackContext context)
    {
        //Avoid multiple inputs in a frame
        if (_isMoveKeyPressed) return;
        _isMoveKeyPressed = true;

        //End previous movement
        if (IsTargetSet) BreakMoving();

        IsTargetSet = false;

        //Read input
        Vector3 moveValue = context.ReadValue<Vector3>();

        //Set moving data
        _movingDirection = new float[3] { moveValue.x, moveValue.y, moveValue.z };
        _movingAxisIndex = Array.FindIndex(_movingDirection, target => target == 1f || target == -1f);
        _isPositive = _movingDirection[_movingAxisIndex] > 0f;

        //Flag to check if any block is movable
        bool isMovable = false;

        //Find every blocks' target positions
        if (_isPositive)
        {
            for (int sliceIndex = GlobalData.GridSize - 1; sliceIndex >= 0; sliceIndex--)
            {
                SetTargetPerSlice(sliceIndex, ref isMovable);
            }
        }
        else
        {
            for (int sliceIndex = 0; sliceIndex < GlobalData.GridSize; sliceIndex++)
            {
                SetTargetPerSlice(sliceIndex, ref isMovable);
            }
        }

        //Cancel movement if all full
        if (!isMovable)
        {
            GameBehavior.Instance.OnGridFull?.Invoke();
            return;
        }

        GameBehavior.Instance.AddMove();
        IsTargetSet = true;

        //Start motion animation calculation coroutine
        _currentWorkingCoroutine = StartCoroutine(MoveToTarget());
    }

    #endregion
}
