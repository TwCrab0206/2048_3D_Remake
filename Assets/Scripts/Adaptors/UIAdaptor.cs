using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UIAdaptor : MonoBehaviour
{
    [SerializeField] protected GameObject _selectBackground;

    protected bool _isSelected = false;
    protected bool _isEnter = false;

    protected RectTransform _rectTransform;
    protected RectTransform _callerRectTransform;

    //Used to change highlight
    public void Select()
    {
        _selectBackground.SetActive(true);
    }

    public void UnSelect()
    {
        _selectBackground.SetActive(false);
    }

    //Used to change selection
    public virtual void Return(InputAction.CallbackContext _)
    {
        if (EnvironmentSettings.CurrentUsingDevice == EnvironmentSettings.AvailableDevices.KeyboardMouse) return;

        if (!_isEnter || !_isSelected) return;

        _isSelected = false;
        _callerRectTransform.GetComponent<UIAdaptor>().Return();

        _callerRectTransform = null;
    }

    public virtual void Return()
    {
        _isEnter = false;
    }

    public virtual void Enter(InputAction.CallbackContext _) { }

    public virtual void Enter(RectTransform callerRectTransform) { }
}
