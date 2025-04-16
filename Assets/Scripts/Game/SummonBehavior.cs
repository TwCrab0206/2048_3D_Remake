using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonBehavior : MonoBehaviour
{
    public static SummonBehavior Instance { get; private set; } = null;

    [SerializeField] GameObject _blockPrefab;
    [SerializeField] List<Material> _materialsList;
    [SerializeField] List<Color> _colorList;

    readonly List<Transform> _mergedBlockTransformList = new();

    Coroutine _currentWorkingCoroutine;

    int _score = 0;
    bool _isGameSet = false;

    public int ReachedBlock { get; private set; } = 0;

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
    }

    private void Start()
    {
        //Generate initial blocks
        SummonNewBlocks(true);
    }

    #region Summon block methods

    private void BreakScaling()
    {
        StopCoroutine(_currentWorkingCoroutine);
        _currentWorkingCoroutine = null;

        foreach (var transform in _mergedBlockTransformList)
        {
            if (transform == null) continue;
            transform.localScale = Vector3.one;
        }

        _mergedBlockTransformList.Clear();
    }

    private IEnumerator ScaleUpBlocks(List<Transform> transformArray)
    {
        float timePass = 0f;
        
        while (timePass < 0.4f)
        {
            float scale = (5f - 6.25f * timePass) * timePass;

            foreach (var transform in transformArray)
            {
                if (transform == null) continue;
                transform.localScale = Vector3.one * scale;
            }

            timePass += Time.deltaTime;

            yield return null;
        }

        foreach (var transform in transformArray)
        {
            if (transform == null) continue;
            transform.localScale = Vector3.one;
        }
    }

    private Transform SummonNew2Or4()
    {
        //Check if transform grid is full
        bool isFull = true;

        foreach (var list in GlobalData.TransformsListGrid3D)
        {
            if (list.Count == 0)
            {
                isFull = false;
                break;
            }
        }

        if (isFull) return null;

        //Generate random position index
        Vector3Int index3D;

        do
        {
            index3D = new Vector3Int()
            {
                x = UnityEngine.Random.Range(0, GlobalData.GridSize),
                y = UnityEngine.Random.Range(0, GlobalData.GridSize),
                z = UnityEngine.Random.Range(0, GlobalData.GridSize)
            };
        }
        while (GlobalData.TransformsListGrid3D[index3D.x, index3D.y, index3D.z].Count != 0);

        //Instantiate block
        GameObject block = Instantiate(_blockPrefab, GlobalData.FixedPositionGrid[index3D.x, index3D.y, index3D.z], Quaternion.identity, gameObject.transform);
        BlockBehavior blockController = block.GetComponent<BlockBehavior>();

        //Store block transform
        GlobalData.TransformsListGrid3D[index3D.x, index3D.y, index3D.z].Add(block.transform);

        //Set block value
        int value = UnityEngine.Random.Range(1, 3);
        blockController.SetBlock(value * 2, _materialsList[value - 1], _colorList[value - 1], value - 1);

        return block.transform;
    }

    public void SummonNewBlocks(bool isFinishInstantly)
    {
        List<Transform> newTransformList = new();

        for (int i = 0; i < GameSettings.SpawnAmount; i++)
        {
            Transform block = SummonNew2Or4();

            if (block != null) newTransformList.Add(block);

            //Avoid useless loop
            if (newTransformList.Count == 0) return;
        }

        if (isFinishInstantly) return;

        //Scale up blocks
        foreach (var newBlockTransform in newTransformList)
        {
            newBlockTransform.localScale = Vector3.zero;
        }

        StartCoroutine(ScaleUpBlocks(newTransformList));
    }

    public void SummonMergedBlocks(List<Vector3Int> mergeTargetIndexes, bool isFinishInstantly)
    {
        if (_currentWorkingCoroutine != null) BreakScaling();

        //Check if merge action is needed
        if (mergeTargetIndexes.Count == 0) return;

        Transform gameSetBlock = null;

        foreach (var index3D in mergeTargetIndexes)
        {
            List<Transform> targetTransformList = GlobalData.TransformsListGrid3D[index3D.x, index3D.y, index3D.z];
            int newValue = targetTransformList[0].GetComponent<BlockBehavior>().Value * 2;
            int newMatIndex = targetTransformList[0].GetComponent<BlockBehavior>().MaterialIndex + 1;

            //Instantiate block
            GameObject block = Instantiate(_blockPrefab, GlobalData.FixedPositionGrid[index3D.x, index3D.y, index3D.z], Quaternion.identity.normalized, gameObject.transform);
            BlockBehavior blockController = block.GetComponent<BlockBehavior>();
            block.transform.localScale = isFinishInstantly ? Vector3.one : Vector3.zero;

            //Store block transform
            GlobalData.TransformsListGrid3D[index3D.x, index3D.y, index3D.z].Add(block.transform);
            _mergedBlockTransformList.Add(block.transform);

            //Set block
            blockController.SetBlock(newValue, _materialsList[newMatIndex], _colorList[newMatIndex], newMatIndex);

            //Destroy old blocks
            Destroy(targetTransformList[0].gameObject);
            Destroy(targetTransformList[1].gameObject);

            //Clear old block transforms
            targetTransformList.RemoveRange(0,2);

            //Add score
            _score += newValue / 2;

            if (GameSettings.CurrentGameMode == GameSettings.GameModes.BlockAttack && newValue == GameSettings.AimBlockValue)
            {
                EnvironmentSettings.InputManager.Player.Disable();
                _isGameSet = true;
                gameSetBlock = block.transform;
            }

            ReachedBlock = (ReachedBlock < newValue) ? newValue : ReachedBlock;
        }

        //Clear old indexes list
        mergeTargetIndexes.Clear();
        GameBehavior.Instance.AddScore(_score);
        _score = 0;

        if (GameSettings.CurrentGameMode == GameSettings.GameModes.BlockAttack && _isGameSet)
        {
            GameBehavior.Instance.OnBlockAttackWin?.Invoke(gameSetBlock);
        }
        
        if (GameSettings.CurrentGameMode == GameSettings.GameModes.ScoreAttack && GameBehavior.Instance.Score >= GameSettings.AimScore)
        {
            EnvironmentSettings.InputManager.Player.Disable();
            _isGameSet = true;
            GameBehavior.Instance.OnScoreAttackWin?.Invoke();
        }

        if (isFinishInstantly) return;

        //Scale up blocks
        _currentWorkingCoroutine = StartCoroutine(ScaleUpBlocks(_mergedBlockTransformList));
    }

    #endregion
}
