using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsUIBehavior : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _xAxisSensitiveText;
    [SerializeField] Slider _xAxisSensitiveSlider;

    [SerializeField] TextMeshProUGUI _yAxisSensitiveText;
    [SerializeField] Slider _yAxisSensitiveSlider;

    [SerializeField] TMP_Dropdown _resolutionDropdown;

    [SerializeField] Toggle _isFullScreenToggle;

    [SerializeField] TextMeshProUGUI _targetFPSText;
    [SerializeField] Slider _targetFPSTextSlider;

    [SerializeField] Toggle _isVSyncToggle;

    [SerializeField] TMP_Dropdown _usingDeviceDropdown;

    [SerializeField] GameObject _bindings;
    [SerializeField] GameObject _message;

    public UnityEvent OnBindingsSetDefault;

    List<Resolution> _resolutionsList = new();
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        SetSupportResolutions();
        SetSupportDevice();
    }

    private void Start()
    {
        SetSettings();
    }

    public void MenuFadeIn()
    {
        _animator.SetBool("IsOn", true);
        RefreshSettings();
    }

    public void MenuFadeOut()
    {
        _animator.SetBool("IsOn", false);
    }

    private void RefreshSettings()
    {
        _xAxisSensitiveText.text = $"X-Axis Sensitive : {EnvironmentSettings.XAxisSensitive:F1}";
        _xAxisSensitiveSlider.value = EnvironmentSettings.XAxisSensitive;

        _yAxisSensitiveText.text = $"Y-Axis Sensitive : {EnvironmentSettings.YAxisSensitive:F1}";
        _yAxisSensitiveSlider.value = EnvironmentSettings.YAxisSensitive;

        _resolutionDropdown.value = EnvironmentSettings.ResolutionIndex;

        _isFullScreenToggle.isOn = EnvironmentSettings.IsFullScreen;

        _targetFPSText.text = $"Target FPS : {EnvironmentSettings.TargetFPS}";
        _targetFPSTextSlider.value = EnvironmentSettings.TargetFPS / 10;

        _isVSyncToggle.isOn = EnvironmentSettings.IsVSync;

        _usingDeviceDropdown.value = (int)EnvironmentSettings.CurrentUsingDevice;
    }

    #region Initialization methods

    private void SetSettings()
    {
        Screen.SetResolution(
            _resolutionsList[EnvironmentSettings.ResolutionIndex].width,
            _resolutionsList[EnvironmentSettings.ResolutionIndex].height,
            EnvironmentSettings.IsFullScreen
        );
        Application.targetFrameRate = EnvironmentSettings.TargetFPS;
        QualitySettings.vSyncCount = EnvironmentSettings.IsVSync ? 1 : 0;

        if (EnvironmentSettings.CurrentUsingDevice != EnvironmentSettings.AvailableDevices.KeyboardMouse)
        {
            _bindings.SetActive(false);
            _message.SetActive(true);
        }
        else
        {
            _bindings.SetActive(true);
            _message.SetActive(false);
        }

        RefreshSettings();
    }

    private void SetSupportResolutions()
    {
        _resolutionsList = Screen.resolutions.ToList();
        _resolutionsList.Reverse();

        //Ignore duplicate resolutions because of different refresh rates
        for (int i = 0; i < _resolutionsList.Count; i++)
        {
            for (int j = i + 1; j < _resolutionsList.Count; j++)
            {
                if (_resolutionsList[i].width == _resolutionsList[j].width &&
                    _resolutionsList[i].height == _resolutionsList[j].height)
                {
                    _resolutionsList.RemoveAt(j);
                    j--;
                }
            }
        }

        _resolutionDropdown.ClearOptions();

        List<string> strings = new();

        foreach (var resolution in _resolutionsList)
        {
            string text = $"{resolution.width} x {resolution.height}";
            strings.Add(text);
        }

        _resolutionDropdown.AddOptions(strings);
    }

    private void SetSupportDevice()
    {
        List<string> strings = Enum.GetNames(typeof(EnvironmentSettings.AvailableDevices)).ToList();

        _usingDeviceDropdown.ClearOptions();
        _usingDeviceDropdown.AddOptions(strings);
    }

    #endregion

    #region Upload data methods

    public void OnXAxisSensitiveChanged(float value)
    {
        EnvironmentSettings.XAxisSensitive = value;
        _xAxisSensitiveText.text = $"X-Axis Sensitive : {value:F1}";
    }

    public void OnYAxisSensitiveChanged(float value)
    {
        EnvironmentSettings.YAxisSensitive = value;
        _yAxisSensitiveText.text = $"Y-Axis Sensitive : {value:F1}";
    }

    public void OnResolutionChanged(int index)
    {
        EnvironmentSettings.ResolutionIndex = index;
        EnvironmentSettings.Resolution = _resolutionsList[index];
        Screen.SetResolution(_resolutionsList[index].width, _resolutionsList[index].height, EnvironmentSettings.IsFullScreen);
    }

    public void OnFullScreenChanged(bool isFullScreen)
    {
        EnvironmentSettings.IsFullScreen = isFullScreen;
        Screen.fullScreen = isFullScreen;
    }

    public void OnTargetFPSChanged(float value)
    {
        int FPS = (int)value * 10;

        EnvironmentSettings.TargetFPS = FPS;
        _targetFPSText.text = $"Target FPS : {FPS}";
        Application.targetFrameRate = FPS;
    }

    public void OnVSyncChanged(bool isVSync)
    {
        EnvironmentSettings.IsVSync = isVSync;
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
    }

    public void OnUsingDeviceChange(int index)
    {
        EnvironmentSettings.CurrentUsingDevice = (EnvironmentSettings.AvailableDevices)index;
        EnvironmentSettings.CurrentUsingKeysMap = EnvironmentSettings.AvailableKeysMaps.FindKeyMap(index);

        if (EnvironmentSettings.CurrentUsingDevice != EnvironmentSettings.AvailableDevices.KeyboardMouse)
        {
            _bindings.SetActive(false);
            _message.SetActive(true);
        }
        else
        {
            _bindings.SetActive(true);
            _message.SetActive(false);
        }
    }

    #endregion

    #region Set default methods

    public void SetMouseSettingsDefault()
    {
        EnvironmentSettings.XAxisSensitive = 1.4f;
        _xAxisSensitiveText.text = $"X-Axis Sensitive : 1.4";
        _xAxisSensitiveSlider.value = 1.4f;

        EnvironmentSettings.YAxisSensitive = 1.4f;
        _yAxisSensitiveText.text = $"Y-Axis Sensitive : 1.4";
        _yAxisSensitiveSlider.value = 1.4f;
    }

    public void SetGraphicSettingsDefault()
    {
        int index = _resolutionsList.FindIndex(target => target.width == 1920 && target.height == 1080);
        EnvironmentSettings.ResolutionIndex = index;
        EnvironmentSettings.Resolution = _resolutionsList[index];
        Screen.SetResolution(_resolutionsList[index].width, _resolutionsList[index].height, EnvironmentSettings.IsFullScreen);
        _resolutionDropdown.value = index;
        _resolutionDropdown.RefreshShownValue();

        EnvironmentSettings.IsFullScreen = true;
        Screen.fullScreen = true;
        _isFullScreenToggle.isOn = true;

        EnvironmentSettings.TargetFPS = 170;
        _targetFPSText.text = $"Target FPS : 170";
        _targetFPSTextSlider.value = 17;
        Application.targetFrameRate = 170;

        EnvironmentSettings.IsVSync = true;
        QualitySettings.vSyncCount = 1;
        _isVSyncToggle.isOn = true;
    }

    public void SetBindingsSettingsDefault()
    {
        EnvironmentSettings.CurrentUsingDevice = EnvironmentSettings.AvailableDevices.KeyboardMouse;
        EnvironmentSettings.CurrentUsingKeysMap = EnvironmentSettings.AvailableKeysMaps.KeyboardKeyMap;
        _usingDeviceDropdown.value = (int)EnvironmentSettings.AvailableDevices.KeyboardMouse;
        _usingDeviceDropdown.RefreshShownValue();
        _bindings.SetActive(true);
        _message.SetActive(false);

        EnvironmentSettings.InputManager.Player.Move.RemoveAllBindingOverrides();
        EnvironmentSettings.InputManager.Player.SelectLayer1.RemoveAllBindingOverrides();
        EnvironmentSettings.InputManager.Player.SelectLayer2.RemoveAllBindingOverrides();
        EnvironmentSettings.InputManager.Player.SelectLayer3.RemoveAllBindingOverrides();
        EnvironmentSettings.InputManager.Player.SelectLayer4.RemoveAllBindingOverrides();
        EnvironmentSettings.InputManager.Player.SelectLayer5.RemoveAllBindingOverrides();
        EnvironmentSettings.InputManager.Player.SelectLayer6.RemoveAllBindingOverrides();
        EnvironmentSettings.InputManager.Player.LoopLayer.RemoveAllBindingOverrides();
        OnBindingsSetDefault?.Invoke();
    }

    #endregion
}
