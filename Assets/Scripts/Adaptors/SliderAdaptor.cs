using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SliderAdaptor : UIAdaptor
{
    [SerializeField] protected Slider _slider;
    [SerializeField] protected AxisType _axis = AxisType.Vertical;
    [SerializeField] protected bool _incrementalReverse = false;

    public enum AxisType
    {
        Vertical,
        Horizontal
    }

    protected void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        EnvironmentSettings.InputManager.UI.Select.started -= Return;
        EnvironmentSettings.InputManager.UI.Select.performed -= ChangeValue;

        EnvironmentSettings.InputManager.UI.Return.started += Return;
        EnvironmentSettings.InputManager.UI.Select.performed += ChangeValue;
    }

    public virtual void ChangeValue(InputAction.CallbackContext context)
    {
        if (EnvironmentSettings.CurrentUsingDevice == EnvironmentSettings.AvailableDevices.KeyboardMouse) return;

        if (!_isSelected) return;

        float value = (_axis == AxisType.Vertical) ? context.ReadValue<Vector2>().y : context.ReadValue<Vector2>().x;
        value *= 0.1f;
        _slider.value += (_incrementalReverse) ? -value : value;
        
        _slider.onValueChanged?.Invoke(_slider.value);
    }

    public override void Enter(RectTransform callerRectTransform)
    {
        _isSelected = true;

        _callerRectTransform = callerRectTransform;
    }
}
