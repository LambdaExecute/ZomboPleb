using System.Collections.Generic;
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
        LevelDataContainer.LevelData levelData = LevelManager.GetLevelData();
        zombieGenerator.Init(levelData.zombieDensity, levelData.zombieFieldDepth);
        
        Vector3 boundsPosition = zombieGenerator.zombieSpawnField.center;
        boundsPosition.z = playerTarget.position.z - levelData.distanceFromZombiesToTarget - levelData.zombieFieldDepth;
        zombieGenerator.zombieSpawnField.center = boundsPosition;

        Vector3 newPlayerStartPosition = playerStartPosition.position;
        newPlayerStartPosition.z = zombieGenerator.zombieSpawnField.center.z - levelData.zombieFieldDepth - levelData.distanceToZombies;
        playerStartPosition.position = newPlayerStartPosition;

        PlayerUI playerUI = windowsManager.Open(WindowType.PlayerUI) as PlayerUI;
        playerObject = Instantiate(playerPrefab, playerStartPosition.position, Quaternion.identity);
        
        Player player = playerObject.GetComponent<Player>();
        player.Init(levelData.playerMass, playerUI, playerTarget, bulletPrefab);
        player.onDead.AddListener(GameLose);
        player.onWin.AddListener(GameWin);

        List<Soldier> soldiers = new List<Soldier>();
        List<Transform> soldierPostions = player.soldiersPositions;
        int soldiersCount = levelData.countOfSoldiers > soldierPostions.Count ? soldierPostions.Count : levelData.countOfSoldiers;
        for (int i = 0; i < soldiersCount; i++)
        {
            Soldier soldier = Instantiate(Resources.Load<GameObject>("Prefabs/NewSoldier"), soldierPostions[i].position, Quaternion.identity).GetComponent<Soldier>();
            soldiers.Add(soldier);
        }

        zombieGenerator.GenerateZombies();
        zombieGenerator.onZombiesGenerated.AddListener(() => { 
            FindObjectOfType<LoadingScreen>().Close();
            player.StartMoving();
            foreach(Soldier soldier in soldiers)
            {
                soldier.StartMoving();
                soldier.StartFindingTarget();
            }
        });
    }
        

    private void GameWin()
    {
        windowsManager.CloseAll();
        windowsManager.Open(WindowType.GameOver, new MessageBox("You win! Well done!"));
        LevelManager.SaveLevelUp();
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
