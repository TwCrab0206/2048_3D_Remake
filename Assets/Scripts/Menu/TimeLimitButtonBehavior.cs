using UnityEngine;

public class TimeLimitButtonBehavior : MonoBehaviour
{
    [SerializeField] int _representTime;
    [SerializeField] NewGameUIBehavior _newGameUIBehavior;

    RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void ChangeTimeLimit()
    {
        GameSettings.TimeLimit = _representTime;
        _newGameUIBehavior.MoveTimeLimitSelectBox(_rectTransform);
    }

    public void FindTimeLimit()
    {
        if (GameSettings.TimeLimit != _representTime) return;

        ChangeTimeLimit();
    }
}
