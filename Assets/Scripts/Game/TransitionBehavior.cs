using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionBehavior : MonoBehaviour
{
    [SerializeField] DirectionMarksBehavior _directionMarksBehavior;
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        MenuFadeOut();
    }

    private void Start()
    {
        _directionMarksBehavior.HideTexts();
    }

    public void MenuSlideIn()
    {
        GameBehavior.Instance.HideGridText();
        _directionMarksBehavior.HideTexts();
        _animator.SetBool("IsOn", false);
    }

    public void MenuFadeOut()
    {
        _animator.SetBool("IsOn", true);
    }

    public void OnFadeEnd()
    {
        _directionMarksBehavior.ShowTexts();
    }

    public void OnSlideEnd()
    {
        EnvironmentSettings.InputManager.Dispose();
        EnvironmentSettings.InputManager.Disable();
        SceneManager.LoadScene(0);
    }
}
