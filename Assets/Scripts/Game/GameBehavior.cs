using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance { get; private set; }

    [SerializeField] Transform _centerAnchor;
    [SerializeField] TextMeshProUGUI _scoreDisplayer;
    [SerializeField] TextMeshProUGUI _timer;
    [SerializeField] TextMeshProUGUI _moveDisplayer;
    [SerializeField] CinemachineInputAxisController _cinemachineInputAxisController;

    public UnityEvent OnGridFull;

    public UnityEvent<Transform> OnBlockAttackWin;
    public UnityEvent OnScoreAttackWin;
    public UnityEvent OnLimitedTimeEnd;

    int _selectedLayerIndex = 0;
    bool _isLayerSelectedThisFrame = false;
    bool _isTimerActive = false;

    public int Time { get; private set; } = 0;

    public int Score { get; private set; } = 0;

    public int Move { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        GlobalData.SetGrid(GameSettings.GridSize);
        SetCenter();

        _timer.text = "00:00";
        _scoreDisplayer.text = "0";
        _moveDisplayer.text = "0";

        RemoveEventListener();
        AddEventListener();
    }

    private void Start()
    {
        StartTimer();
        GameStart(new());
    }

    private void LateUpdate()
    {
        _isLayerSelectedThisFrame = false;
    }

    private void OnApplicationQuit()
    {
        FileIO.SaveEnvironmentSettings();
        FileIO.SaveGameSettings();

        EnvironmentSettings.InputManager.Dispose();
        EnvironmentSettings.InputManager.Disable();
    }

    public void GameEnd()
    {
        EnvironmentSettings.InputManager.Player.Disable();
        EnvironmentSettings.InputManager.UI.Enable();
        EndTimer();
        _cinemachineInputAxisController.enabled = false;
    }

    public void GamePause(InputAction.CallbackContext _)
    {
        EnvironmentSettings.InputManager.Player.Disable();
        EnvironmentSettings.InputManager.UI.Enable();
        PauseTimer();
        _cinemachineInputAxisController.enabled = false;
    }

    public void GamePause()
    {
        EnvironmentSettings.InputManager.Player.Disable();
        EnvironmentSettings.InputManager.UI.Enable();
        PauseTimer();
        _cinemachineInputAxisController.enabled = false;
    }

    public void GameStart(InputAction.CallbackContext _)
    {
        EnvironmentSettings.InputManager.Player.Enable();
        EnvironmentSettings.InputManager.UI.Disable();
        ResumeTimer();
        _cinemachineInputAxisController.enabled = true;
    }

    public void GameStart()
    {
        EnvironmentSettings.InputManager.Player.Enable();
        EnvironmentSettings.InputManager.UI.Disable();
        ResumeTimer();
        _cinemachineInputAxisController.enabled = true;
    }

    private void SetCenter()
    {
        float halfGridLength = GlobalData.BlockDistance * (GlobalData.GridSize - 1) * 0.5f;
        _centerAnchor.localPosition = new(halfGridLength, halfGridLength, halfGridLength);
        _centerAnchor.localScale = 2 * halfGridLength * Vector3.one;
    }

    private void AddEventListener()
    {
        //Input System
        EnvironmentSettings.InputManager.Player.SelectLayer1.started += SelectLayer1;
        EnvironmentSettings.InputManager.Player.SelectLayer2.started += SelectLayer2;
        EnvironmentSettings.InputManager.Player.SelectLayer3.started += SelectLayer3;
        EnvironmentSettings.InputManager.Player.SelectLayer4.started += SelectLayer4;
        EnvironmentSettings.InputManager.Player.SelectLayer5.started += SelectLayer5;
        EnvironmentSettings.InputManager.Player.SelectLayer6.started += SelectLayer6;
        EnvironmentSettings.InputManager.Player.LoopLayer.started += LoopSelectGridLayer;
        EnvironmentSettings.InputManager.Player.Pause.started += GamePause;
        EnvironmentSettings.InputManager.UI.Resume.started += GameStart;
    }

    private void RemoveEventListener()
    {
        //Input System
        EnvironmentSettings.InputManager.Player.SelectLayer1.started -= SelectLayer1;
        EnvironmentSettings.InputManager.Player.SelectLayer2.started -= SelectLayer2;
        EnvironmentSettings.InputManager.Player.SelectLayer3.started -= SelectLayer3;
        EnvironmentSettings.InputManager.Player.SelectLayer4.started -= SelectLayer4;
        EnvironmentSettings.InputManager.Player.SelectLayer5.started -= SelectLayer5;
        EnvironmentSettings.InputManager.Player.SelectLayer6.started -= SelectLayer6;
        EnvironmentSettings.InputManager.Player.LoopLayer.started -= LoopSelectGridLayer;
        EnvironmentSettings.InputManager.Player.Pause.started -= GamePause;
        EnvironmentSettings.InputManager.UI.Resume.started -= GameStart;
    }

    #region Layer focusing control methods

    public void ShowGridText()
    {
        if (_isLayerSelectedThisFrame) return;
        _isLayerSelectedThisFrame = true;

        foreach (var list in GlobalData.TransformsListGrid3D)
        {
            if (list.Count == 0) continue;

            foreach (var block in list)
            {
                BlockBehavior blockBehavior = block.GetComponent<BlockBehavior>();
                blockBehavior.ChangeTextVisibility(true);
            }
        }
    }

    public void HideGridText()
    {
        if (_isLayerSelectedThisFrame) return;
        _isLayerSelectedThisFrame = true;

        foreach (var list in GlobalData.TransformsListGrid3D)
        {
            if (list.Count == 0) continue;

            foreach (var block in list)
            {
                BlockBehavior blockBehavior = block.GetComponent<BlockBehavior>();
                blockBehavior.ChangeTextVisibility(false);
            }
        }
    }

    public void RefreshLayerVisibility()
    {
        if (_selectedLayerIndex == 0) return;

        for (int x = 0; x < GlobalData.GridSize; x++)
        {
            for (int y = 0; y < GlobalData.GridSize; y++)
            {
                for (int z = 0; z < GlobalData.GridSize; z++)
                {
                    List<Transform> transformList = GlobalData.TransformsListGrid3D[x, y, z];

                    if (transformList.Count == 0) continue;

                    BlockBehavior blockBehavior = transformList[0].GetComponent<BlockBehavior>();

                    if ((y + 1) == _selectedLayerIndex)
                    {
                        blockBehavior.ChangeVisibility(true);
                    }
                    else
                    {
                        blockBehavior.ChangeVisibility(false);
                    }
                }
            }
        }
    }

    private void TraverseLayer(int layerIndex, bool isVisible)
    {
        for (int x = 0; x < GlobalData.GridSize; x++)
        {
            for (int z = 0; z < GlobalData.GridSize; z++)
            {
                List<Transform> list = GlobalData.TransformsListGrid3D[x, layerIndex - 1, z];

                if (list.Count == 0)
                {
                    continue;
                }

                foreach (var block in list)
                {
                    BlockBehavior blockBehavior = block.GetComponent<BlockBehavior>();
                    blockBehavior.ChangeVisibility(isVisible);
                }
            }
        }
    }

    private void TraverseGrid(bool isVisible)
    {
        foreach (var list in GlobalData.TransformsListGrid3D)
        {
            if (list.Count == 0) continue;

            foreach (var block in list)
            {
                BlockBehavior blockBehavior = block.GetComponent<BlockBehavior>();
                blockBehavior.ChangeVisibility(isVisible);
            }
        }
    }

    private void ChangeGridLayerVisibility(int layerIndex)
    {
        //Ignore not used keys
        if (layerIndex > GlobalData.GridSize || layerIndex == 0)
        {
            return;
        }

        //Stop focusing on layer if pressed the same key
        if (layerIndex == _selectedLayerIndex)
        {
            TraverseGrid(true);

            _selectedLayerIndex = 0;
            return;
        }

        //Change focusing target if index transit from 1 ~ GridSize to 1 ~ GridSize
        if (_selectedLayerIndex != layerIndex && _selectedLayerIndex != 0)
        {
            TraverseLayer(_selectedLayerIndex, false);
            TraverseLayer(layerIndex, true);

            _selectedLayerIndex = layerIndex;
        }

        //Focus on layer if index transit from 0 to 1 ~ GridSize
        TraverseGrid(false);
        TraverseLayer(layerIndex, true);

        _selectedLayerIndex = layerIndex;
    }

    //Select layer method only available for keyboard
    private void SelectGridLayer(int layer)
    {
        //Check if current using device is keyboard
        if (EnvironmentSettings.CurrentUsingDevice != EnvironmentSettings.AvailableDevices.KeyboardMouse) return;

        //Avoid multiple selection in one frame
        if (_isLayerSelectedThisFrame) return;
        _isLayerSelectedThisFrame = true;

        ChangeGridLayerVisibility(layer);
    }

    public void SelectLayer1(InputAction.CallbackContext _) { SelectGridLayer(1); }

    public void SelectLayer2(InputAction.CallbackContext _) { SelectGridLayer(2); }

    public void SelectLayer3(InputAction.CallbackContext _) { SelectGridLayer(3); }

    public void SelectLayer4(InputAction.CallbackContext _) { SelectGridLayer(4); }

    public void SelectLayer5(InputAction.CallbackContext _) { SelectGridLayer(5); }

    public void SelectLayer6(InputAction.CallbackContext _) { SelectGridLayer(6); }

    //Loop layer method available for any device
    public void LoopSelectGridLayer(InputAction.CallbackContext context)
    {
        int nextLayerIndex = _selectedLayerIndex + 1;
        if (nextLayerIndex > GlobalData.GridSize) nextLayerIndex = 0;

        if (nextLayerIndex == 0)
        {
            TraverseGrid(true);
            _selectedLayerIndex = 0;
        }
        else
        {
            ChangeGridLayerVisibility(nextLayerIndex);
        }
    }

    #endregion

    #region Timer control methods

    public void StartTimer()
    {
        if (_isTimerActive) return;

        if (GameSettings.CurrentGameMode == GameSettings.GameModes.LimitedTime) Time = GameSettings.TimeLimit;
        else if (GameSettings.CurrentGameMode == GameSettings.GameModes.BlockAttack) Time = 0;
        else if (GameSettings.CurrentGameMode == GameSettings.GameModes.ScoreAttack) Time = 0;
        else if (GameSettings.CurrentGameMode == GameSettings.GameModes.Free) Time = 0;
        else if (GameSettings.CurrentGameMode == GameSettings.GameModes.None) return;

        int minutes = Time / 60;
        int seconds = Time % 60;

        _timer.text = $"{minutes:D2}:{seconds:D2}";

        InvokeRepeating(nameof(TimerStep), 1, 1);
        _isTimerActive = true;
    }

    public void EndTimer()
    {
        if (!_isTimerActive) return;

        CancelInvoke(nameof(TimerStep));
        _isTimerActive = false;
    }

    public void PauseTimer()
    {
        if (!_isTimerActive) return;

        CancelInvoke(nameof(TimerStep));
        _isTimerActive = false;
    }

    public void ResumeTimer()
    {
        if (_isTimerActive) return;

        InvokeRepeating(nameof(TimerStep), 1, 1);
        _isTimerActive = true;
    }

    private void TimerStep()
    {
        if (GameSettings.CurrentGameMode == GameSettings.GameModes.BlockAttack) Time++;
        else if (GameSettings.CurrentGameMode == GameSettings.GameModes.ScoreAttack) Time++;
        else if (GameSettings.CurrentGameMode == GameSettings.GameModes.LimitedTime) Time--;
        else if (GameSettings.CurrentGameMode == GameSettings.GameModes.Free) Time++;

        if (Time < 0) EndTimer();

        int minutes = Time / 60;
        int seconds = Time % 60;

        _timer.text = $"{minutes:D2}:{seconds:D2}";

        if (GameSettings.CurrentGameMode == GameSettings.GameModes.LimitedTime && Time <= 0)
        {
            EndTimer();
            _timer.text = "00:00";
            OnLimitedTimeEnd?.Invoke();
        }
    }

    #endregion

    public void AddScore(int score)
    {
        Score += score;
        _scoreDisplayer.text = Score.ToString();

        if (GameSettings.CurrentGameMode == GameSettings.GameModes.BlockAttack && Score >= GameSettings.AimScore)
        {
            EndTimer();
        }
    }

    public void AddMove()
    {
        Move++;
        _moveDisplayer.text = Move.ToString();
    }
}
