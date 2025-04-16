using UnityEngine;

public class AimBlockButtonBehavior : MonoBehaviour
{
    [SerializeField] int _representBlock;
    [SerializeField] NewGameUIBehavior _newGameUIBehavior;

    RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void ChangeAimBlock()
    {
        GameSettings.AimBlockValue = _representBlock;
        _newGameUIBehavior.MoveAimBlockSelectBox(_rectTransform);
    }

    public void FindAimBlock()
    {
        if (GameSettings.AimBlockValue != _representBlock) return;

        ChangeAimBlock();
    }
}
