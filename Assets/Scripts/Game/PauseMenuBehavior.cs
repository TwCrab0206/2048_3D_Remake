using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuBehavior : MonoBehaviour
{
    [SerializeField] GameObject _pauseBackground;
    [SerializeField] DirectionMarksBehavior _directionMarksBehavior;
    [SerializeField] PauseSettingsMenuBehavior _pauseSettingsMenuBehavior;
    [SerializeField] Image _resumeMarkImage;
    [SerializeField] Color _pressedColor = Color.white;
    [SerializeField] float _pressingTime = 0.15f;

    Animator _animator;
    bool _isSettingsOpen = false;
    readonly List<Color> _originalColorList = new();

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();

        _originalColorList.Add(_resumeMarkImage.color);
    }

    private void Start()
    {
        EnvironmentSettings.InputManager.Player.Pause.started -= OpenMenu;
        EnvironmentSettings.InputManager.UI.Resume.started -= CloseMenu;
        
        EnvironmentSettings.InputManager.Player.Pause.started += OpenMenu;
        EnvironmentSettings.InputManager.UI.Resume.started += CloseMenu;
    }

    public void OpenMenu(InputAction.CallbackContext _)
    {
        _animator.SetBool("isPaused", true);
        _pauseBackground.SetActive(true);
    }

    public void OpenMenu()
    {
        _animator.SetBool("isPaused", true);
        _pauseBackground.SetActive(true);
    }

    public void CloseMenu(InputAction.CallbackContext _)
    {
        _animator.SetBool("isPaused", false);
        _pauseBackground.SetActive(false);

        if (_isSettingsOpen)
        {
            _directionMarksBehavior.ShowTexts();
            GameBehavior.Instance.ShowGridText();

            _isSettingsOpen = false;
        }
    }

    public void CloseMenu()
    {
        _animator.SetBool("isPaused", false);
        _pauseBackground.SetActive(false);

        if (_isSettingsOpen)
        {
            _directionMarksBehavior.ShowTexts();
            GameBehavior.Instance.ShowGridText();

            _isSettingsOpen = false;
        }
    }

    private IEnumerator ChangeMarkColor(Image keyMark, int index)
    {
        keyMark.color = _pressedColor;

        yield return new WaitForSeconds(_pressingTime);

        keyMark.color = _originalColorList[index];
    }

    public void OnResumeButtonClick()
    {
        StartCoroutine(ChangeMarkColor(_resumeMarkImage, 0));
        CloseMenu();
    }

    public void OnSettingsButtonPressed()
    {
        if (_isSettingsOpen) GameBehavior.Instance.ShowGridText();
        else GameBehavior.Instance.HideGridText();

        if (_isSettingsOpen) _directionMarksBehavior.ShowTexts();
        else _directionMarksBehavior.HideTexts();

        if (_isSettingsOpen) _pauseSettingsMenuBehavior.CloseMenu();
        else _pauseSettingsMenuBehavior.OpenMenu();

        _isSettingsOpen = !_isSettingsOpen;
    }
}
