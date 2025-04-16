using UnityEngine;
using UnityEngine.UI;

public class ButtonAdaptor : UIAdaptor
{
    [SerializeField] Button _button;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public override void Enter(RectTransform callerRectTransform) //Enter then immediately return
    {
        _button.onClick?.Invoke();

        callerRectTransform.GetComponent<UIAdaptor>().Return();
    }
}
