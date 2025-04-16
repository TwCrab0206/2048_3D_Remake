using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWinUIBehavior : MonoBehaviour
{
    [SerializeField] AnimationClip _winScreenFadeDeep;
    [SerializeField] TextMeshProUGUI _timeDisplayer;
    [SerializeField] TextMeshProUGUI _scoreDisplayer;
    [SerializeField] TextMeshProUGUI _moveDisplayer;

    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OpenMenu()
    {
        int minute = GameBehavior.Instance.Time / 60;
        int second = GameBehavior.Instance.Time % 60;

        _timeDisplayer.text = $"Time : {minute:D2}:{second:D2}";
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
        FileIO.ReloadInputManager();
        SceneManager.LoadScene(0);
    }
}
