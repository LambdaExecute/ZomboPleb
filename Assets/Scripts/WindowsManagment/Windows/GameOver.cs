using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GameOver : Window
{
    public UnityEvent onRetry;

    [SerializeField] private Button retryButton;
    [SerializeField] private Button toMainMenuButton;
    [SerializeField] private TMP_Text messageField;

    public override void Show(WindowsManager windowsManager, object data)
    {
        base.Show(windowsManager, data);
        messageField.text = ((MessageBox)data).message;

        retryButton.onClick.AddListener(Retry);
        toMainMenuButton.onClick.AddListener(ToMainMenu);
    }

    public override void Close() => base.Close();

    private void ToMainMenu() => SceneManager.LoadScene("MainMenu");


    private void Retry()
    {
        onRetry.Invoke();
        Close();
    }
}
