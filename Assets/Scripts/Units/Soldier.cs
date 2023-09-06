using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    [Min(40)]
    [SerializeField] private float fov;
    [SerializeField] private float maxDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private float acceleration;
    [SerializeField] private float soldierLifeTime;
    [Header("Count of shots per second.")]
    [Min(1)]
    [SerializeField] private float shotsPerSec = 1;
    [Space]
    [SerializeField] private Transform bulletSpawnPoint;

    private Animator animator;
    private List<Zombie> zombies = new List<Zombie>();
    private new Transform transform;

    private Zombie target;

    private Zombie[] nearestZombies;
    private GameObject soldierBulletPrefab;

    private Coroutine moving;
    private Coroutine findTargetMachine;
    private Coroutine shooting;

    private bool isMoving = false;
    private bool isTargetInFov => Vector3.Angle(transform.forward, target.transform.position - transform.position) <= fov;

    public void StartMoving()
    {
        moving = StartCoroutine(IMove());
        StartCoroutine(ILiveStatusMachine());
    }

    public void StopMoving()
    {
        isMoving = false;
        animator.SetTrigger("Idle");
        if (moving != null)
        {
            StopCoroutine(moving);
            moving = null;
        }
    }

    public void StartFindingTarget() => findTargetMachine = StartCoroutine(IFindTargetMachine());

    private void Start()
    {
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        soldierBulletPrefab = Resources.Load<GameObject>("Prefabs/SoldierBullet");
    }

    private IEnumerator IMove()
    {
        isMoving = true;
        animator.SetTrigger("Run");
        while(true)
        {
            yield return null;
            transform.position += Vector3.forward * acceleration * Time.deltaTime;
        }
    }

    private IEnumerator IFindTargetMachine()
    {
        zombies = FindObjectsOfType<Zombie>().ToList();
        while (true)
        {
            while (target == null)
            {
                StopShooting();
                yield return null;

                nearestZombies = zombies.FindAll(z => Vector3.Distance(z.transform.position, transform.position) <= maxDistance && !z.isDead).ToArray();
                target = GetNearestZombie();

                if (target != null)
                {
                    if (!isTargetInFov)
                    {
                        zombies.Remove(target);
                        target = null;
                    }
                    else
                        StartShooting(target);
                }
            }

            yield return null;

            if (target != null)
            {
                if (!isTargetInFov)
                {
                    zombies.Remove(target);
                    target = null;
                }
            }

            if (Vector3.Distance(GetNearestZombie().transform.position, transform.position) <= minDistance)
                StopMoving();
            else if (!isMoving)
                StartMoving();
        }
    }

    private void StartShooting(Zombie target)
    {
        animator.SetLayerWeight(2, 1);
        shooting = StartCoroutine(IShooting(target));
    }

    private void StopShooting()
    {
        animator.SetLayerWeight(2, 0);
        if (shooting != null)
        {
            StopCoroutine(shooting);
            shooting = null;
        }
    }

    private IEnumerator IShooting(Zombie target)
    {
        while (!target.isDead)
        {
            yield return new WaitForSecondsRealtime(1 / shotsPerSec);

            GameObject bullet = Instantiate(soldierBulletPrefab, bulletSpawnPoint.transform.position, Quaternion.identity);
            bullet.GetComponent<SoldierBullet>().Init(target.transform.position - bullet.transform.position);
        }
        animator.SetLayerWeight(2, 0);
        zombies.Remove(target);
        this.target = null;
    }

    private IEnumerator ILiveStatusMachine()
    {
        yield return null;
        while(true)
        {
            yield return new WaitUntil(() => !isMoving);
            while(!isMoving)
            {
                Debug.Log("fff");
                yield return null;
                soldierLifeTime -= Time.deltaTime / 10;

                if (soldierLifeTime <= 0)
                    goto death;
            }
            yield return null;
        }
    death:
        StopAllCoroutines();
        Destroy(gameObject);
    }

    private Zombie GetNearestZombie()
    {
        if (nearestZombies.Length == 0)
            return null;

        Zombie nearestZombie = nearestZombies.First();
        float minDistance = maxDistance;
        foreach (Zombie currentZombie in nearestZombies)
        {
            float currentDistance = Vector3.Distance(transform.position, currentZombie.transform.position);
            if (currentDistance < minDistance)
            {
                nearestZombie = currentZombie;
                minDistance = currentDistance;
            }
        }
        return nearestZombie;
    }
}
