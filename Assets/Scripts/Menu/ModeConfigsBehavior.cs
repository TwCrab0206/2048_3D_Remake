using UnityEngine;

public class ModeConfigsBehavior : MonoBehaviour
{
    [SerializeField] CanvasGroup _aimBlockCanvasGroup;
    [SerializeField] CanvasGroup _aimScoreCanvasGroup;
    [SerializeField] CanvasGroup _timeLimitCanvasGroup;

    private void SetCanvasGroupOn(CanvasGroup target)
    {
        target.alpha = 1;
        target.blocksRaycasts = true;
    }

    private void SetCanvasGroupOff(CanvasGroup target)
    {
        target.alpha = 0;
        target.blocksRaycasts = false;
    }

    private void ResetCanvasGroup()
    {
        SetCanvasGroupOff(_aimBlockCanvasGroup);
        SetCanvasGroupOff(_aimScoreCanvasGroup);
        SetCanvasGroupOff(_timeLimitCanvasGroup);
    }

    public void OpenModeConfig(GameSettings.GameModes gameMode)
    {
        ResetCanvasGroup();

        switch (gameMode)
        {
            case GameSettings.GameModes.BlockAttack:
                SetCanvasGroupOn(_aimBlockCanvasGroup);
                break;
            case GameSettings.GameModes.ScoreAttack:
                SetCanvasGroupOn(_aimScoreCanvasGroup);
                break;
            case GameSettings.GameModes.LimitedTime:
                SetCanvasGroupOn(_timeLimitCanvasGroup);
                break;
        }
    }
}
