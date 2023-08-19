using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameOver : Window
{
    [SerializeField] private Button retryButton;
    [SerializeField] private Button toMainMenuButton;
    [SerializeField] private TMP_Text messageField;

    private CanvasGroup canvasGroup;

    public override void Show(WindowsManager windowsManager, object data)
    {
        canvasGroup = GetComponent<CanvasGroup>();
        base.Show(windowsManager, data);
        messageField.text = ((MessageBox)data).message;

        retryButton.onClick.AddListener(Retry);
        toMainMenuButton.onClick.AddListener(ToMainMenu);
        StartCoroutine(IShow());
    }

    private IEnumerator IShow()
    {
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            yield return null;
            canvasGroup.alpha = i;
        }
        canvasGroup.alpha = 1;
    }

    public override void Close() => base.Close();

    private void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Close();
    }

    private void Retry()
    {
        FindObjectOfType<GameIniter>().RetryGame();
        Close();
    }
}
