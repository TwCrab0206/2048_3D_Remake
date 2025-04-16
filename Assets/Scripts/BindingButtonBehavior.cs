using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class BindingButtonBehavior : MonoBehaviour
{
    public int bindingIndex = 0;
    public ActionType actionType = ActionType.Move;

    [Header("Leave empty if the binding button isn't used in game scene")]
    [SerializeField] PauseSettingsMenuBehavior _pauseSettingsMenuBehavior;

    public enum ActionType
    {
        Move,
        SelectLayer1,
        SelectLayer2,
        SelectLayer3,
        SelectLayer4,
        SelectLayer5,
        SelectLayer6,
        LoopLayer
    }

    Button _bindingButton;
    TextMeshProUGUI _bindingText;

    InputControl _inputControl;
    InputSystem_Actions _inputSystemAction;
    bool _isInRebinding = false;

    private void Awake()
    {
        _inputSystemAction = new();
        _inputSystemAction.Disable();

        _bindingButton = GetComponentInChildren<Button>();
        _bindingText = _bindingButton.GetComponentInChildren<TextMeshProUGUI>();

        InputSystem.onAnyButtonPress.Call(control => _inputControl = control);

        _inputSystemAction.UI.AnyInput.started -= ReadInput;
        _inputSystemAction.UI.AnyInput.started += ReadInput;
    }

    private void OnApplicationQuit()
    {
        _inputSystemAction.UI.AnyInput.started -= ReadInput;
    }

    public void RefreshText()
    {
        switch (actionType)
        {
            case ActionType.Move:
                _bindingText.text = EnvironmentSettings.CurrentUsingKeysMap[EnvironmentSettings.InputManager.Player.Move.bindings[bindingIndex].effectivePath];
                break;
            case ActionType.SelectLayer1:
                _bindingText.text = EnvironmentSettings.CurrentUsingKeysMap[EnvironmentSettings.InputManager.Player.SelectLayer1.bindings[bindingIndex].effectivePath];
                break;
            case ActionType.SelectLayer2:
                _bindingText.text = EnvironmentSettings.CurrentUsingKeysMap[EnvironmentSettings.InputManager.Player.SelectLayer2.bindings[bindingIndex].effectivePath];
                break;
            case ActionType.SelectLayer3:
                _bindingText.text = EnvironmentSettings.CurrentUsingKeysMap[EnvironmentSettings.InputManager.Player.SelectLayer3.bindings[bindingIndex].effectivePath];
                break;
            case ActionType.SelectLayer4:
                _bindingText.text = EnvironmentSettings.CurrentUsingKeysMap[EnvironmentSettings.InputManager.Player.SelectLayer4.bindings[bindingIndex].effectivePath];
                break;
            case ActionType.SelectLayer5:
                _bindingText.text = EnvironmentSettings.CurrentUsingKeysMap[EnvironmentSettings.InputManager.Player.SelectLayer5.bindings[bindingIndex].effectivePath];
                break;
            case ActionType.SelectLayer6:
                _bindingText.text = EnvironmentSettings.CurrentUsingKeysMap[EnvironmentSettings.InputManager.Player.SelectLayer6.bindings[bindingIndex].effectivePath];
                break;
            case ActionType.LoopLayer:
                _bindingText.text = EnvironmentSettings.CurrentUsingKeysMap[EnvironmentSettings.InputManager.Player.LoopLayer.bindings[bindingIndex].effectivePath];
                break;
            default:
                return;
        }
    }

    public void ReadInput(InputAction.CallbackContext context)
    {
        if (!_isInRebinding || _inputControl == null) return;

        string keyPath = _inputControl.path;
        _inputControl = null;

        if (keyPath.Contains("/Keyboard")) keyPath = keyPath.Replace("/Keyboard", "<Keyboard>");

        if (EnvironmentSettings.AvailableKeysMaps.KeyboardKeyMap.ContainsKey(keyPath))
        {
            string name = EnvironmentSettings.AvailableKeysMaps.KeyboardKeyMap[keyPath];
            _bindingText.text = name;

            ChangeBinding(keyPath);
        }

        _bindingText.fontStyle = FontStyles.Bold;
        _isInRebinding = false;
        _inputSystemAction.Disable();
    }

    public void OnRebindingButtonClick()
    {
        if (_isInRebinding) return;
        _isInRebinding = true;

        _inputSystemAction.Enable();
        _bindingText.fontStyle = FontStyles.Bold | FontStyles.Italic;
    }

    private void ChangeBinding(string keyPath)
    {
        InputAction action;

        switch (actionType)
        {
            case ActionType.Move:
                action = EnvironmentSettings.InputManager.Player.Move;
                break;
            case ActionType.SelectLayer1:
                action = EnvironmentSettings.InputManager.Player.SelectLayer1;
                break;
            case ActionType.SelectLayer2:
                action = EnvironmentSettings.InputManager.Player.SelectLayer2;
                break;
            case ActionType.SelectLayer3:
                action = EnvironmentSettings.InputManager.Player.SelectLayer3;
                break;
            case ActionType.SelectLayer4:
                action = EnvironmentSettings.InputManager.Player.SelectLayer4;
                break;
            case ActionType.SelectLayer5:
                action = EnvironmentSettings.InputManager.Player.SelectLayer5;
                break;
            case ActionType.SelectLayer6:
                action = EnvironmentSettings.InputManager.Player.SelectLayer6;
                break;
            case ActionType.LoopLayer:
                action = EnvironmentSettings.InputManager.Player.LoopLayer;
                break;
            default:
                return;
        }

        InputActionRebindingExtensions.ApplyBindingOverride(action, bindingIndex, keyPath);
        if (_pauseSettingsMenuBehavior != null) _pauseSettingsMenuBehavior.OnBindingChange?.Invoke();
    }
}
