using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Soldier : MonoBehaviour
{
    [Min(40)]
    [SerializeField] private float fov;
    [SerializeField] private float maxDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private float acceleration;
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

    private bool isDead = false;
    private int aimingLayerID = 2;

    private bool isTargetInFov => Vector3.Angle(target.transform.position - transform.position, transform.forward) <= fov;
    private bool isMoving;

    private void Start()
    {
        soldierBulletPrefab = Resources.Load<GameObject>("Prefabs/SoldierBullet");

        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }

    public void StartMoving()
    {
        zombies = FindObjectsOfType<Zombie>().ToList();
        StartCoroutine(IShootMachine());
        StartCoroutine(IMove());
    }

    private IEnumerator IMove()
    {
        isMoving = true;
        animator.SetTrigger("Run");
        transform.rotation = Quaternion.identity;
        while (isMoving)
        {
            yield return null;
            transform.position += Vector3.forward * acceleration * Time.deltaTime;
        }
        animator.SetTrigger("Idle");
    }

    private IEnumerator IShootMachine()
    {
        while (true)
        {
            while (target == null)
            {
                yield return null;
                
                nearestZombies = zombies.FindAll(z => Vector3.Distance(z.transform.position, transform.position) <= maxDistance && !z.isDead).ToArray();

                if (nearestZombies.Length > 0)
                {
                    target = GetNearestZombie();

                    if (Vector3.Distance(target.transform.position, transform.position) < minDistance)
                    {
                        isMoving = false;
                        transform.LookAt(target.transform.position);
                    }
                    else if (isMoving && !isTargetInFov)
                    {
                        zombies.Remove(target);
                        target = null;
                    }
                }
            }
            
            for(float i = 0; i <= 1; i += Time.deltaTime * 4)
            {
                yield return null;
                animator.SetLayerWeight(aimingLayerID, i);
            }
            
            animator.SetLayerWeight(aimingLayerID, 1);
            StartCoroutine(IShooting());

            yield return new WaitUntil(() => target == null);
        }
    }

    private IEnumerator IShooting()
    {
        while (target != null)
        {
            if (target.isDead)
            {
                for (float i = 1; i >= 0; i -= Time.deltaTime * 4)
                {
                    yield return null;
                    animator.SetLayerWeight(aimingLayerID, i);
                }
                animator.SetLayerWeight(aimingLayerID, 0);

                zombies.Remove(target);
                target = null;
                StartCoroutine(IMove());

                break;
            }

            yield return new WaitForSecondsRealtime(1 / shotsPerSec);

            Instantiate(soldierBulletPrefab, bulletSpawnPoint.position, Quaternion.identity)
                   .GetComponent<SoldierBullet>()
                   .Init(target.transform.position - transform.position);
        }
    }

    private Zombie GetNearestZombie()
    {
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
