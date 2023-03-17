using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : Window
{
    [SerializeField] private Button startGame;

    private void Start()
    {
        startGame.onClick.AddListener(() => SceneManager.LoadScene("Game"));
    }
}
