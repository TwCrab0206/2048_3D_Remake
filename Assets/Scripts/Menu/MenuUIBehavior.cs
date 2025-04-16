using UnityEngine;

public class MenuUIBehavior : MonoBehaviour
{
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Invoke(nameof(MenuFadeIn), 0.5f);

        FileIO.LoadEnvironmentSettings();
    }

    private void OnApplicationQuit()
    {
        FileIO.SaveEnvironmentSettings();
    }

    public void MenuFadeIn()
    {
        _animator.SetBool("IsOn", true);
    }

    public void MenuFadeOut()
    {
        _animator.SetBool("IsOn", false);
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }
}
