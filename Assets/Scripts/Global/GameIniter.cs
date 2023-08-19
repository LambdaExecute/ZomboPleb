using UnityEngine;
using UnityEngine.SceneManagement;

public class GameIniter : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject bulletPrefab;
    [Space]
    [SerializeField] private Transform playerStartPosition;
    [SerializeField] private Transform playerTarget;
    [Space]
    [SerializeField] private ZombieGenerator zombieGenerator;
    [SerializeField] private WindowsManager windowsManager;
    
    private GameObject playerObject;

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        PlayerUI playerUI = windowsManager.Open(WindowType.PlayerUI) as PlayerUI;
        playerObject = Instantiate(playerPrefab, playerStartPosition.position, Quaternion.identity);
        
        Player player = playerObject.GetComponent<Player>();
        player.Init(playerUI, playerTarget, bulletPrefab);
        player.onDead.AddListener(GameLose);
        player.onWin.AddListener(GameWin);
        
        zombieGenerator.onZombiesGenerated.AddListener(() => { 
            FindObjectOfType<LoadingScreen>().Close();
            player.StartMoving();
        });
        zombieGenerator.Init();
    }

    private void GameWin()
    {
        windowsManager.CloseAll();
        windowsManager.Open(WindowType.GameOver, new MessageBox("You win! Well done!")).GetComponent<GameOver>();
    }

    private void GameLose()
    {
        windowsManager.CloseAll();
        Instantiate(Resources.Load<GameObject>("Prefabs/DeathScene"), Vector3.zero + Vector3.down * 40, Quaternion.identity).GetComponent<GameOverScene>().Init();
    }

    public void RetryGame()
    {
        LoadingScreen loadingScreen = windowsManager.Open(WindowType.LoadingScreen) as LoadingScreen;
        loadingScreen.onScreenShowed.AddListener(() => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }
}
