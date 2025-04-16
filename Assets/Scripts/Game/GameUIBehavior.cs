using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameUIBehavior : MonoBehaviour
{
    [SerializeField] Color _pressedColor = Color.white;
    [SerializeField] float _pressingTime = 0.15f;

    [SerializeField] MovingBehavior _movingBehavior;

    //Key marks
    [SerializeField] Image _upMarkImage;
    [SerializeField] TextMeshProUGUI _upMarkText;

    [SerializeField] Image _downMarkImage;
    [SerializeField] TextMeshProUGUI _downMarkText;

    [SerializeField] Image _pauseMarkImage;

    //Input system
    bool _isKeyPressedThisFrame = false;

    readonly List<Color> _originalColorList = new();

    private void Awake()
    {
        _originalColorList.Add(_upMarkImage.color);
        _originalColorList.Add(_downMarkImage.color);
        _originalColorList.Add(_pauseMarkImage.color);
    }

    private void Start()
    {
        EnvironmentSettings.InputManager.Player.Move.started -= PressMovementKey;
        EnvironmentSettings.InputManager.Player.Pause.started -= OpenPauseMenu;

        EnvironmentSettings.InputManager.Player.Move.started += PressMovementKey;
        EnvironmentSettings.InputManager.Player.Pause.started += OpenPauseMenu;

        SetKeysText();
    }

    private void LateUpdate()
    {
        _isKeyPressedThisFrame = false;
    }

    private string FindKeyDisplayText(int index)
    {
        string path = EnvironmentSettings.InputManager.Player.Move.bindings[index].effectivePath;
        return EnvironmentSettings.CurrentUsingKeysMap[path];
    }

    public void SetKeysText()
    {
        if (EnvironmentSettings.CurrentUsingDevice == EnvironmentSettings.AvailableDevices.KeyboardMouse)
        {
            _upMarkText.text = FindKeyDisplayText(1);
            _downMarkText.text = FindKeyDisplayText(2);
        }
        else if (
            EnvironmentSettings.CurrentUsingDevice == EnvironmentSettings.AvailableDevices.PSGamepad ||
            EnvironmentSettings.CurrentUsingDevice == EnvironmentSettings.AvailableDevices.XboxGamepad)
        {
            _upMarkText.text = FindKeyDisplayText(8);
            _downMarkText.text = FindKeyDisplayText(9);
        }
    }

    #region Change UI key color methods

    private IEnumerator ChangeKeyColor(Image keyMark, int index)
    {
        keyMark.color = _pressedColor;

        yield return new WaitForSeconds(_pressingTime);

        keyMark.color = _originalColorList[index];
    }

    private void ChooseKeyMark(bool isPositive)
    {
        if (isPositive)
        {
            StartCoroutine(ChangeKeyColor(_upMarkImage, 0));
        }
        else
        {
            StartCoroutine(ChangeKeyColor(_downMarkImage, 1));
        }
    }

    public void PressMovementKey(InputAction.CallbackContext context)
    {
        if (_isKeyPressedThisFrame) return;
        _isKeyPressedThisFrame = true;

        List<int> moveValue = new() { (int)context.ReadValue<Vector3>().x, (int)context.ReadValue<Vector3>().y, (int)context.ReadValue<Vector3>().z };
        int movingAxisIndex = moveValue.FindIndex(target => target == 1 || target == -1);

        if (movingAxisIndex != 1) return;

        bool isPositive = moveValue[movingAxisIndex] == 1;

        ChooseKeyMark(isPositive);
    }

    public void OpenPauseMenu(InputAction.CallbackContext context)
    {
        if (_isKeyPressedThisFrame) return;
        _isKeyPressedThisFrame = true;

        StartCoroutine(ChangeKeyColor(_pauseMarkImage, 2));
    }

    #endregion

    #region Methods for UI marks click event

    public void OnUpMarkClick()
    {
        if (_isKeyPressedThisFrame) return;
        _isKeyPressedThisFrame = true;

        _movingBehavior.MoveBlocks(Vector3.up);
    }

    public void OnDownMarkClick()
    {
        if (_isKeyPressedThisFrame) return;
        _isKeyPressedThisFrame = true;

        _movingBehavior.MoveBlocks(Vector3.down);
    }

    public void OnPauseMarkClick()
    {
        if (_isKeyPressedThisFrame) return;
        _isKeyPressedThisFrame = true;
    }

    #endregion
}
