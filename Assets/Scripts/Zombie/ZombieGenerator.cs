using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZombieGenerator : MonoBehaviour
{
    public Bounds zombieSpawnField = new Bounds(Vector3.zero, Vector3.one * 5);

    [Space]
    [SerializeField] private Vector3 zombiesRotation;
    [Space]
    [SerializeField] private int zombieCount;
    [SerializeField] private float minDistanceBetweenPositions;

    private List<Vector3> spawnPositions = new List<Vector3>();
    private List<GameObject> zombies = new List<GameObject>();
    private GameObject zombiePrefab;

    public void Init(GameObject zombiePrefab)
    {
        this.zombiePrefab = zombiePrefab;
        GenerateZombies();
    }

    public void ClearZombies()
    {
        spawnPositions.Clear();
        while(zombies.Count > 0)
        {
            Destroy(zombies[0]);
            zombies.RemoveAt(0);
        }
    }

    private void GenerateZombies()
    {
        int failCount = 0;
        List<GameObject> zombiesPrefabs = Resources.LoadAll<GameObject>("Prefabs/Zombies/").ToList();
        for(int i = 0; i < zombieCount; i++)
        {
            Vector3 spawnPoint = new Vector3(Random.Range(zombieSpawnField.min.x, zombieSpawnField.max.x), 
                                             zombieSpawnField.min.y, 
                                             Random.Range(zombieSpawnField.min.z, zombieSpawnField.max.z));
            if(IsPositionValid(spawnPoint))
            {
                Zombie zombie = Instantiate(zombiePrefab, spawnPoint, Quaternion.identity).GetComponent<Zombie>();
                zombie.Init(zombiesPrefabs, 0);
                zombie.transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                spawnPositions.Add(spawnPoint);
                zombies.Add(zombie.gameObject);
            }
            else
            {
                i--;
                failCount++;
            }
            if (failCount > 50)
            {
                Debug.LogError($"Unable to generate zombies with setted params: zombies count: {zombieCount}, min distance between zombies: {minDistanceBetweenPositions}.\nGenerating zombies with more convenient params started");
                zombieCount -= 25;
                ClearZombies();
                GenerateZombies();
                return;
            }
        }
    }

    private bool IsPositionValid(Vector3 targetPosition)
    {
        foreach(Vector3 currentPosition in spawnPositions)
        {
            if(Vector3.Distance(targetPosition, currentPosition) < minDistanceBetweenPositions)
                return false;
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(zombieSpawnField.center, zombieSpawnField.size);
    }
}
