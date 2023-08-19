using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ZombieGenerator : MonoBehaviour
{
    public Bounds zombieSpawnField = new Bounds(Vector3.zero, Vector3.one * 5);
    public UnityEvent onZombiesGenerated = new UnityEvent();

    [Space]
    [SerializeField] private Vector3 zombiesRotation;
    [Space]
    [SerializeField] private int zombieCount;
    [SerializeField] private float minDistanceBetweenPositions;

    private List<GameObject> zombies = new List<GameObject>();
    private GameObject zombiePrefab;

    public void Init()
    {
        zombiePrefab = Resources.Load<GameObject>("Prefabs/Zombies/ZombiePrefab");
        StartCoroutine(GenerateZombies());
    }

    private IEnumerator GenerateZombies()
    {
        float step = 1.5f;
        List<GameObject> zombiesPrefabs = Resources.LoadAll<GameObject>("Prefabs/Zombies/Zombie0/ZombieSkin").ToList();
        for(float z = zombieSpawnField.min.z; z < zombieSpawnField.max.z; z += step)
        {
            yield return null;
            for (float x = zombieSpawnField.min.x; x < zombieSpawnField.max.x; x += step)
            {
                Vector3 spawnPoint = new Vector3(x + Random.Range(-0.25f, 0.25f), zombieSpawnField.min.y, z + Random.Range(-0.25f, 0.25f));

                Zombie zombie = Instantiate(zombiePrefab, spawnPoint, Quaternion.identity).GetComponent<Zombie>();
                zombie.Init(zombiesPrefabs, 0);
                zombie.transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                
                zombies.Add(zombie.gameObject);
            }
        }
        Instantiate(Resources.Load<GameObject>("Prefabs/DeadZone")).GetComponent<DeadZone>().Init(zombieSpawnField);

        yield return new WaitForSecondsRealtime(2f);

        onZombiesGenerated.Invoke();
        onZombiesGenerated.RemoveAllListeners();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(zombieSpawnField.center, zombieSpawnField.size);
    }
}
