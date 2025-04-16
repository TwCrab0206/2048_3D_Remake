using UnityEngine;
using UnityEngine.UI;

public class ToggleAdaptor : UIAdaptor
{
    [SerializeField] Toggle _toggle;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public override void Enter(RectTransform callerRectTransform) //Flip then immediately return
    {
        _toggle.isOn = !_toggle.isOn;
        _toggle.onValueChanged?.Invoke(_toggle.isOn);

        callerRectTransform.GetComponent<UIAdaptor>().Return();
    }
}
