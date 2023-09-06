using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : Window
{
    [SerializeField] private Button startGame;
    [SerializeField] private Button quitButton;
    
    private void Start()
    {
        windowsManager = FindObjectOfType<WindowsManager>();
        startGame.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(() => Application.Quit());
        LevelManager.Init();
    }

    private void StartGame()
    {
        (windowsManager.Open(WindowType.LoadingScreen) as LoadingScreen).onScreenShowed.AddListener(() => { 
            SceneManager.LoadScene("PizzaPlace");
            Close();
        });
    }
}
