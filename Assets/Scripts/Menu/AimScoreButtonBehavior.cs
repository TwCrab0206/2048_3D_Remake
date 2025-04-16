using UnityEngine;

public class AimScoreButtonBehavior : MonoBehaviour
{
    [SerializeField] int _representAimScore;
    [SerializeField] NewGameUIBehavior _newGameUIBehavior;

    RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void ChangeAimScore()
    {
        GameSettings.AimScore = _representAimScore;
        _newGameUIBehavior.MoveAimScoreSelectBox(_rectTransform);
    }

    public void FindAimScore()
    {
        if (GameSettings.AimScore != _representAimScore) return;

        ChangeAimScore();
    }
}
