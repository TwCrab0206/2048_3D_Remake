using UnityEngine;
using UnityEngine.InputSystem;

public class IntSliderAdaptor : SliderAdaptor
{
    [SerializeField] [Min(0)] float _threshold = 0.5f;

    public override void ChangeValue(InputAction.CallbackContext context)
    {
        if (EnvironmentSettings.CurrentUsingDevice == EnvironmentSettings.AvailableDevices.KeyboardMouse) return;

        if (!_isSelected) return;

        Vector2 move = context.ReadValue<Vector2>();
        int value = 0;

        if (_axis == AxisType.Vertical && move.y > _threshold || move.y < -_threshold)
        {
            value = (move.y > 0) ? 1 : -1;
        }
        else if (_axis == AxisType.Horizontal && move.x > _threshold || move.x < -_threshold)
        {
            value = (move.x > 0) ? 1 : -1;
        }

        _slider.value += (_incrementalReverse) ? -value : value;

        _slider.onValueChanged?.Invoke(_slider.value);
    }
}
