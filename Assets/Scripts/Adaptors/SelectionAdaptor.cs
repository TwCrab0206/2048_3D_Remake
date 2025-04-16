using Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SelectionAdaptor : UIAdaptor
{
    [SerializeField] bool _isRoot = false;
    [SerializeField] TArray<UIAdaptor> _selectableUIAdaptors;

    public UnityEvent OnEnterCalled;
    public UnityEvent OnReturnPressed;

    Vector2Int _selectedAdaptorIndex = new(0, 0);

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _isSelected = _isRoot;

        EnvironmentSettings.InputManager.UI.Select.started -= ChangeSelectedAdaptor;
        EnvironmentSettings.InputManager.UI.Return.started -= Return;
        EnvironmentSettings.InputManager.UI.Enter.started -= Enter;

        EnvironmentSettings.InputManager.UI.Select.started += ChangeSelectedAdaptor;
        EnvironmentSettings.InputManager.UI.Return.started += Return;
        EnvironmentSettings.InputManager.UI.Enter.started += Enter;
    }

    //Used to change highlight
    public void ResetSelection()
    {
        _selectedAdaptorIndex = new Vector2Int(0, 0);
        _isSelected = _isRoot;
        _isEnter = false;
    }

    public void ChangeSelectionVisibility(bool isVisible)
    {
        if (isVisible && _selectableUIAdaptors[0, 0] != null)
        {
            _selectableUIAdaptors[0, 0].GetComponent<UIAdaptor>().Select();
        }
        else
        {
            UnSelect();
        }
    }

    //Used to change selection
    public void ChangeSelectedAdaptor(InputAction.CallbackContext context)
    {
        if (EnvironmentSettings.CurrentUsingDevice == EnvironmentSettings.AvailableDevices.KeyboardMouse) return;

        if (_isEnter || !_isSelected) return;

        Vector2Int move = new((int)context.ReadValue<Vector2>().x, (int)context.ReadValue<Vector2>().y);
        Vector2Int newIndex = _selectedAdaptorIndex + move;

        if (_selectableUIAdaptors[_selectedAdaptorIndex.x, _selectedAdaptorIndex.y] == null) return;

        if (_selectableUIAdaptors[_selectedAdaptorIndex.x, _selectedAdaptorIndex.y] != null)
        {
            _selectableUIAdaptors[_selectedAdaptorIndex.x, _selectedAdaptorIndex.y].GetComponent<UIAdaptor>().UnSelect();
        }

        if (_selectableUIAdaptors[newIndex.x, newIndex.y] != null)
        {
            _selectableUIAdaptors[newIndex.x, newIndex.y].GetComponent<UIAdaptor>().Select();
        }

        _selectedAdaptorIndex = newIndex;
    }

    public override void Return(InputAction.CallbackContext _)
    {
        base.Return(_);

        OnReturnPressed?.Invoke();
    }

    public override void Enter(InputAction.CallbackContext _)
    {
        if (EnvironmentSettings.CurrentUsingDevice == EnvironmentSettings.AvailableDevices.KeyboardMouse) return;

        if (_isEnter || !_isSelected) return;

        _isEnter = true;
        _selectableUIAdaptors[_selectedAdaptorIndex.x, _selectedAdaptorIndex.y].GetComponent<UIAdaptor>().Enter(_rectTransform);
    }

    public override void Enter(RectTransform callerRectTransform)
    {
        _isSelected = true;

        //Refresh the selection
        _selectableUIAdaptors[_selectedAdaptorIndex.x, _selectedAdaptorIndex.y].GetComponent<UIAdaptor>().UnSelect();
        _selectableUIAdaptors[0, 0].GetComponent<UIAdaptor>().Select();
        _selectedAdaptorIndex = new (0, 0);

        _callerRectTransform = callerRectTransform;
        OnEnterCalled?.Invoke();
    }
}
