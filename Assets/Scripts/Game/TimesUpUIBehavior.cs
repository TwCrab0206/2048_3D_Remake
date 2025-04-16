using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimesUpUIBehavior : MonoBehaviour
{
    [SerializeField] AnimationClip _timesUpScreenFadeDeep;
    [SerializeField] TextMeshProUGUI _blockDisplayer;
    [SerializeField] TextMeshProUGUI _scoreDisplayer;
    [SerializeField] TextMeshProUGUI _moveDisplayer;

    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OpenMenu()
    {
        _blockDisplayer.text = $"Block : {SummonBehavior.Instance.ReachedBlock}";
        _scoreDisplayer.text = $"Score : {GameBehavior.Instance.Score}";
        _moveDisplayer.text = $"Move : {GameBehavior.Instance.Move}";

        GameBehavior.Instance.GameEnd();
        _animator.SetBool("IsOn", true);
    }

    public void BackToMenu()
    {
        _animator.SetBool("IsOut", true);
        Invoke(nameof(ToMenu), 0.7f);
    }

    private void ToMenu()
    {
        EnvironmentSettings.InputManager.Disable();
        EnvironmentSettings.InputManager.Dispose();
        SceneManager.LoadScene(0);
    }
}
