using UnityEngine;

public class GameIniter : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject zombiePrefab;
    [Space]
    [SerializeField] private Transform playerStartPosition;
    [SerializeField] private Transform playerTarget;
    [Space]
    [SerializeField] private ZombieGenerator zombieGenerator;
    [SerializeField] private WindowsManager windowsManager;
    [SerializeField] private FinalDoor finalDoor;
    [SerializeField] private Camera camera;

    private GameObject playerObject;

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        camera.gameObject.SetActive(false);
        PlayerUI playerUI = windowsManager.Open(WindowType.PlayerUI) as PlayerUI;
        playerObject = Instantiate(playerPrefab, playerStartPosition.position, Quaternion.identity);
        Player player = playerObject.GetComponent<Player>();
        player.Init(playerUI, playerTarget, bulletPrefab);
        player.StartMoving();
        zombieGenerator.Init(zombiePrefab);

        player.onDead.AddListener(GameLose);
        player.onWin.AddListener(GameWin);
    }

    private void GameWin()
    {
        windowsManager.CloseAll();
        windowsManager.Open(WindowType.GameOver, new MessageBox("You win! Well done!")).GetComponent<GameOver>().onRetry.AddListener(RetryGame);
        //finalDoor.Open();
    }

    private void GameLose()
    {
        windowsManager.CloseAll();
        camera.gameObject.SetActive(true);
        windowsManager.Open(WindowType.GameOver, new MessageBox("You lose! Try again.")).GetComponent<GameOver>().onRetry.AddListener(RetryGame);
    }

    private void RetryGame()
    {
        if (playerObject != null)
            Destroy(playerObject);
        windowsManager.CloseAll();

        //finalDoor.Close();
        zombieGenerator.ClearZombies();
        StartGame();
    }
}
