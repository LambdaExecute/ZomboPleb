using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ZombieGenerator : MonoBehaviour
{
    public UnityEvent onZombiesGenerated = new UnityEvent();
    public Bounds zombieSpawnField = new Bounds(Vector3.zero, Vector3.one * 5);

    private List<GameObject> zombies = new List<GameObject>();
    private GameObject zombiePrefab;
    private float density;

    public void Init(float density, float depth)
    {
        this.density = density;
        zombiePrefab = Resources.Load<GameObject>("Prefabs/Zombies/ZombiePrefab");
        
        Vector3 newSize = zombieSpawnField.extents;
        newSize.z = depth;
        zombieSpawnField.extents = newSize;
    }

    public void GenerateZombies() => StartCoroutine(IGenerateZombies());

    private IEnumerator IGenerateZombies()
    {
        float step = 1 / Mathf.Sqrt(density) * 1.75f;
        List<GameObject> zombiesPrefabs = Resources.LoadAll<GameObject>($"Prefabs/Zombies/Zombie{0}/ZombieSkin").ToList();
        for(float z = zombieSpawnField.min.z; z < zombieSpawnField.max.z; z += step)
        {
            yield return null;
            for (float x = zombieSpawnField.min.x; x < zombieSpawnField.max.x; x += step)
            {
                Vector3 spawnPoint = new Vector3(x + Random.Range(-0.75f, 0.75f), zombieSpawnField.min.y, z + Random.Range(-0.75f, 0.75f));

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
