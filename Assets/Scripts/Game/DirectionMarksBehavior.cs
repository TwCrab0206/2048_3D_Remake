using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class DirectionMarksBehavior : MonoBehaviour
{
    [SerializeField] float _gap = 1f;
    [SerializeField] float _pressedTime = 0.15f;
    [SerializeField] Color _pressedColor = Color.white;

    //Left mark
    [SerializeField] Transform _leftMark;
    Transform _leftCanvasTransform;
    TMPro.TextMeshProUGUI _leftText;
    Material _leftMarkMaterial;

    //Right mark
    [SerializeField] Transform _rightMark;
    Transform _rightCanvasTransform;
    TMPro.TextMeshProUGUI _rightText;
    Material _rightMarkMaterial;

    //Forward mark
    [SerializeField] Transform _forwardMark;
    Transform _forwardCanvasTransform;
    TMPro.TextMeshProUGUI _forwardText;
    Material _forwardMarkMaterial;

    //Backward mark
    [SerializeField] Transform _backwardMark;
    Transform _backwardCanvasTransform;
    TMPro.TextMeshProUGUI _backwardText;
    Material _backwardMarkMaterial;

    //Input system
    InputSystem_Actions _inputSystemActions;
    bool _isKeyPressedThisFrame = false;

    readonly List<Color> _originalMaterialList = new();

    private void Awake()
    {
        _leftMarkMaterial = _leftMark.GetComponent<MeshRenderer>().material;
        _leftCanvasTransform = _leftMark.GetComponentInChildren<RectTransform>();
        _leftText = _leftCanvasTransform.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        _rightMarkMaterial = _rightMark.GetComponent<MeshRenderer>().material;
        _rightCanvasTransform = _rightMark.GetComponentInChildren<RectTransform>();
        _rightText = _rightCanvasTransform.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        _forwardMarkMaterial = _forwardMark.GetComponent<MeshRenderer>().material;
        _forwardCanvasTransform = _forwardMark.GetComponentInChildren<RectTransform>();
        _forwardText = _forwardCanvasTransform.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        _backwardMarkMaterial = _backwardMark.GetComponent<MeshRenderer>().material;
        _backwardCanvasTransform = _backwardMark.GetComponentInChildren<RectTransform>();
        _backwardText = _backwardCanvasTransform.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        GetOriginalColors();
    }

    private void Start()
    {
        EnvironmentSettings.InputManager.Player.Move.started -= PressKeyMarks;
        EnvironmentSettings.InputManager.Player.Move.started += PressKeyMarks;

        SetMarksPosition();
        SetKeysText();
    }

    private void Update()
    {
        MakeMarkCanvasLookFaceToCamera();
    }

    private void LateUpdate()
    {
        _isKeyPressedThisFrame = false;
    }

    private void MakeMarkCanvasLookFaceToCamera()
    {
        _leftCanvasTransform.LookAt(Camera.main.transform);
        _leftCanvasTransform.Rotate(0, 180, 0);

        _rightCanvasTransform.LookAt(Camera.main.transform);
        _rightCanvasTransform.Rotate(0, 180, 0);

        _forwardCanvasTransform.LookAt(Camera.main.transform);
        _forwardCanvasTransform.Rotate(0, 180, 0);

        _backwardCanvasTransform.LookAt(Camera.main.transform);
        _backwardCanvasTransform.Rotate(0, 180, 0);
    }

    public void ShowTexts()
    {
        _leftText.color = Color.white;
        _rightText.color = Color.white;
        _forwardText.color = Color.white;
        _backwardText.color = Color.white;
    }

    public void HideTexts()
    {
        _leftText.color = Color.clear;
        _rightText.color = Color.clear;
        _forwardText.color = Color.clear;
        _backwardText.color = Color.clear;
    }

    #region Blink mark methods

    private IEnumerator ChangeMarkColor(Material markMaterial, int index)
    {
        markMaterial.color = _pressedColor;

        yield return new WaitForSeconds(_pressedTime);

        markMaterial.color = _originalMaterialList[index];
    }

    private void ChooseMark(bool isPositive, int movingAxisIndex)
    {
        if (isPositive && movingAxisIndex == 0)
        {
            StartCoroutine(ChangeMarkColor(_rightMarkMaterial, 1));
        }
        else if (!isPositive && movingAxisIndex == 0)
        {
            StartCoroutine(ChangeMarkColor(_leftMarkMaterial, 0));
        }
        else if (isPositive && movingAxisIndex == 2)
        {
            StartCoroutine(ChangeMarkColor(_forwardMarkMaterial, 2));
        }
        else if (!isPositive && movingAxisIndex == 2)
        {
            StartCoroutine(ChangeMarkColor(_backwardMarkMaterial, 3));
        }
    }

    private void PressKeyMarks(InputAction.CallbackContext context)
    {
        if (_isKeyPressedThisFrame) return;
        _isKeyPressedThisFrame = true;

        List<int> moveValue = new() { (int)context.ReadValue<Vector3>().x, (int)context.ReadValue<Vector3>().y, (int)context.ReadValue<Vector3>().z };
        int movingAxisIndex = moveValue.FindIndex(target => target == 1 || target == -1);
        bool isPositive = moveValue[movingAxisIndex] == 1;

        ChooseMark(isPositive, movingAxisIndex);
    }

    #endregion

    #region Initialization methods

    private void GetOriginalColors()
    {
        _originalMaterialList.Add(_leftMarkMaterial.color);
        _originalMaterialList.Add(_rightMarkMaterial.color);
        _originalMaterialList.Add(_forwardMarkMaterial.color);
        _originalMaterialList.Add(_backwardMarkMaterial.color);
    }

    private void SetMarksPosition()
    {
        float halfWidth = GlobalData.BlockDistance * (float)(GlobalData.GridSize - 1) * 0.5f;

        transform.position = new Vector3(halfWidth, -0.5f, halfWidth);

        _leftMark.localPosition = Vector3.left * (halfWidth + _gap);
        _rightMark.localPosition = Vector3.right * (halfWidth + _gap);
        _forwardMark.localPosition = Vector3.forward * (halfWidth + _gap);
        _backwardMark.localPosition = Vector3.back * (halfWidth + _gap);
    }

    public void SetKeysText()
    {
        var bindingsList = EnvironmentSettings.InputManager.Player.Move.bindings.ToList();

        if (EnvironmentSettings.CurrentUsingDevice == EnvironmentSettings.AvailableDevices.KeyboardMouse)
        {
            _leftText.text = EnvironmentSettings.CurrentUsingKeysMap[bindingsList[3].effectivePath];
            _rightText.text = EnvironmentSettings.CurrentUsingKeysMap[bindingsList[4].effectivePath];
            _forwardText.text = EnvironmentSettings.CurrentUsingKeysMap[bindingsList[5].effectivePath];
            _backwardText.text = EnvironmentSettings.CurrentUsingKeysMap[bindingsList[6].effectivePath];
        }
        else if (
            EnvironmentSettings.CurrentUsingDevice == EnvironmentSettings.AvailableDevices.PSGamepad ||
            EnvironmentSettings.CurrentUsingDevice == EnvironmentSettings.AvailableDevices.XboxGamepad)
        {
            _leftText.text = EnvironmentSettings.CurrentUsingKeysMap[bindingsList[10].effectivePath];
            _rightText.text = EnvironmentSettings.CurrentUsingKeysMap[bindingsList[11].effectivePath];
            _forwardText.text = EnvironmentSettings.CurrentUsingKeysMap[bindingsList[12].effectivePath];
            _backwardText.text = EnvironmentSettings.CurrentUsingKeysMap[bindingsList[13].effectivePath];
        }
    }

    #endregion
}
