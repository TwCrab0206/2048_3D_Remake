using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NewGameUIBehavior : MonoBehaviour
{
    [SerializeField] RectTransform _gridSizeSelectBox;
    [SerializeField] RectTransform _gameModeSelectBox;
    [SerializeField] RectTransform _aimScoreSelectBox;
    [SerializeField] RectTransform _aimBlockSelectBox;
    [SerializeField] RectTransform _timeLimitSelectBox;
    [SerializeField] RectTransform _spawnAmountSelectBox;
    [SerializeField] AnimationClip _newGameFadeOutClip;
    [SerializeField] float _accuracy;
    [SerializeField] float _transitionSpeed;

    public UnityEvent OnGameSettingsLoad;

    Animator _animator;
    Coroutine _workingCoroutine_GridSize;
    Coroutine _workingCoroutine_GameMode;
    Coroutine _workingCoroutine_AimScore;
    Coroutine _workingCoroutine_AimBlock;
    Coroutine _workingCoroutine_TimeLimit;
    Coroutine _workingCoroutine_SpawnAmount;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        FileIO.LoadGameSettings();
        OnGameSettingsLoad?.Invoke();
    }

    private void OnApplicationQuit()
    {
        FileIO.SaveGameSettings();
    }

    public void MenuFadeIn()
    {
        _animator.SetBool("IsOn", true);
    }

    public void MenuFadeOut()
    {
        _animator.SetBool("IsOn", false);
    }

    #region Move config select box methods

    private IEnumerator MoveSelectBox(RectTransform rectTransform , Vector3 pos)
    {
        while (Vector3.Distance(pos, rectTransform.position) > _accuracy)
        {
            rectTransform.position = Vector3.Lerp(rectTransform.position, pos, Time.deltaTime * _transitionSpeed);

            yield return null;
        }

        rectTransform.position = pos;
    }

    public void MoveGridSizeSelectBox(RectTransform rectTransform)
    {
        Vector3 pos = rectTransform.position;

        if (_workingCoroutine_GridSize != null) StopCoroutine(_workingCoroutine_GridSize);
        _workingCoroutine_GridSize = StartCoroutine(MoveSelectBox(_gridSizeSelectBox , pos));
    }

    public void MoveGameModeSelectBox(RectTransform rectTransform)
    {
        Vector3 pos = rectTransform.position;

        if (_workingCoroutine_GameMode != null) StopCoroutine(_workingCoroutine_GameMode);
        _workingCoroutine_GameMode = StartCoroutine(MoveSelectBox(_gameModeSelectBox, pos));
    }

    public void MoveAimScoreSelectBox(RectTransform rectTransform)
    {
        Vector3 pos = rectTransform.position;

        if (_workingCoroutine_AimScore != null) StopCoroutine(_workingCoroutine_AimScore);
        _workingCoroutine_AimScore = StartCoroutine(MoveSelectBox(_aimScoreSelectBox, pos));
    }

    public void MoveAimBlockSelectBox(RectTransform rectTransform)
    {
        Vector3 pos = rectTransform.position;

        if (_workingCoroutine_AimBlock != null) StopCoroutine(_workingCoroutine_AimBlock);
        _workingCoroutine_AimBlock = StartCoroutine(MoveSelectBox(_aimBlockSelectBox, pos));
    }

    public void MoveTimeLimitSelectBox(RectTransform rectTransform)
    {
        Vector3 pos = rectTransform.position;

        if (_workingCoroutine_TimeLimit != null) StopCoroutine(_workingCoroutine_TimeLimit);
        _workingCoroutine_TimeLimit = StartCoroutine(MoveSelectBox(_timeLimitSelectBox, pos));
    }

    public void MoveSpawnAmountSelectBox(RectTransform rectTransform)
    {
        Vector3 pos = rectTransform.position;

        if (_workingCoroutine_SpawnAmount != null) StopCoroutine(_workingCoroutine_SpawnAmount);
        _workingCoroutine_SpawnAmount = StartCoroutine(MoveSelectBox(_spawnAmountSelectBox, pos));
    }

    #endregion

    public void StartNewGame()
    {
        FileIO.SaveGameSettings();
        _animator.SetBool("IsOn", false);
        Invoke(nameof(LoadGame), _newGameFadeOutClip.averageDuration);
    }

    private void LoadGame()
    {
        FileIO.ReloadInputManager();
        SceneManager.LoadScene(1);
    }
}
