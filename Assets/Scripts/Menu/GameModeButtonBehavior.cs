using UnityEngine;

public class GameModeButtonBehavior : MonoBehaviour
{
    [SerializeField] GameSettings.GameModes _representGameMode = GameSettings.GameModes.None;
    [SerializeField] NewGameUIBehavior _newGameUIBehavior;
    [SerializeField] ModeConfigsBehavior _modeConfigsBehavior;

    RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void ChangeGameMode()
    {
        GameSettings.CurrentGameMode = _representGameMode;
        _newGameUIBehavior.MoveGameModeSelectBox(_rectTransform);
        _modeConfigsBehavior.OpenModeConfig(_representGameMode);
    }

    public void FindGameMode()
    {
        if (GameSettings.CurrentGameMode != _representGameMode) return;

        ChangeGameMode();
    }
}
