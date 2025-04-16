using UnityEngine;

public class BlockSpawnAmountButtonBehavior : MonoBehaviour
{
    [SerializeField] int _representAmount;
    [SerializeField] NewGameUIBehavior _newGameUIBehavior;

    RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void ChangeSpawnAmount()
    {
        GameSettings.SpawnAmount = _representAmount;
        _newGameUIBehavior.MoveSpawnAmountSelectBox(_rectTransform);
    }

    public void FindSpawnAmount()
    {
        if (GameSettings.SpawnAmount != _representAmount) return;

        ChangeSpawnAmount();
    }
}
