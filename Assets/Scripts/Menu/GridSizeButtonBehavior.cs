using UnityEngine;

public class GridSizeButtonBehavior : MonoBehaviour
{
    [SerializeField] int _representSize;
    [SerializeField] NewGameUIBehavior _newGameUIBehavior;

    RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void ChangeGridSize()
    {
        GameSettings.GridSize = _representSize;
        _newGameUIBehavior.MoveGridSizeSelectBox(_rectTransform);
    }

    public void FindGridSize()
    {
        if (GameSettings.GridSize != _representSize) return;

        ChangeGridSize();
    }
}
