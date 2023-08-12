using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Soldier : MonoBehaviour
{
    //[SerializeField] private MultiAimConstraint multiAimConstraint;
    [Space]
    [Min(40)]
    [SerializeField] private float fov;
    [SerializeField] private float maxDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private float acceleration;
    [Header("Count of shots per second.")]
    [Min(1)]
    [SerializeField] private float ShotsPerSec = 1;
    [Space]
    [SerializeField] private Transform bulletSpawnPoint;

    private Animator animator;
    private List<Zombie> zombies = new List<Zombie>();
    private new Transform transform;
    private Zombie target;
    private Zombie[] nearestZombies;
    private RigBuilder rigBuilder;
    private GameObject soldierBulletPrefab;

    private bool isDead = false;
    private int aimingLayerID = 2;

    private bool isTargetInFov => Vector3.Angle(target.transform.position - transform.position, transform.forward) <= fov;

    private Coroutine movementCoroutine;
    private Coroutine shootMachineCoroutine;
    private Coroutine shootCoroutine;

    private void Start()
    {
        StartCoroutine(IStart());
    }

    private IEnumerator IStart()
    {
        yield return new WaitForSeconds(2f);
        soldierBulletPrefab = Resources.Load<GameObject>("Prefabs/SoldierBullet");

        rigBuilder = GetComponent<RigBuilder>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();

        zombies = FindObjectsOfType<Zombie>().ToList();

        shootMachineCoroutine = StartCoroutine(IShootMachine());
        movementCoroutine = StartCoroutine(IMove());
    }

    private IEnumerator IMove()
    {
        while (!isDead)
        {
            transform.rotation = Quaternion.identity;
            animator.SetTrigger("Run");
            while (true)
            {
                yield return null;
                transform.position += Vector3.forward * acceleration * Time.deltaTime;
                if(target != null && !target.isDead && Vector3.Distance(transform.position, target.transform.position) <= minDistance)
                    break;
            }
            transform.LookAt(target.transform.position);
            animator.SetTrigger("Idle");
            while(true)
            {
                yield return new WaitForSecondsRealtime(0.001f);
                if (target != null && Vector3.Distance(transform.position, target.transform.position) > minDistance)
                    break;
                if (nearestZombies.Length <= 0)
                    break;
            }
        }
    }

    private IEnumerator IShootMachine()
    {
        SetAnimationTarget(null);
        while (true)
        {
            int tryFindTargetCount = 0;
            while (target == null)
            {
                yield return new WaitForSecondsRealtime(0.001f);
                
                nearestZombies = zombies.FindAll(z => Vector3.Distance(z.transform.position, transform.position) <= maxDistance && !z.isDead).ToArray();
                
                if (nearestZombies.Length > 0)
                {
                    Zombie nearestZombie = nearestZombies.First();
                    float minDistance = maxDistance;
                    foreach(Zombie currentZombie in nearestZombies)
                    {
                        float currentDistance = Vector3.Distance(transform.position, currentZombie.transform.position);
                        if(currentDistance < minDistance)
                        {
                            nearestZombie = currentZombie;
                            minDistance = currentDistance;
                        }
                    }

                    SetAnimationTarget(nearestZombie.transform);

                    target = nearestZombie;
                    tryFindTargetCount++;

                    if(tryFindTargetCount > 5)
                    {
                        tryFindTargetCount = 0;
                        zombies.Remove(nearestZombie);
                    }
                    if (!isTargetInFov)
                    {
                        SetAnimationTarget(null);
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
            shootCoroutine = StartCoroutine(IShooting());
            yield return new WaitUntil(() => target == null);
            StopCoroutine(shootCoroutine);
        }
    }

    private IEnumerator IShooting()
    {
        while (target != null)
        {
            if (target.isDead || !isTargetInFov)
            {
                for (float i = 1; i >= 0; i -= Time.deltaTime * 4)
                {
                    yield return null;
                    animator.SetLayerWeight(aimingLayerID, i);
                }
                animator.SetLayerWeight(aimingLayerID, 0);

                SetAnimationTarget(null);

                zombies.Remove(target);
                target = null;
            }

            yield return new WaitForSecondsRealtime(1 / ShotsPerSec);

            Instantiate(soldierBulletPrefab, bulletSpawnPoint.position, Quaternion.identity)
                       .GetComponent<SoldierBullet>()
                       .Init(target.transform.position - transform.position);
        }
    }

    private void SetAnimationTarget(Transform target)
    {
        //WeightedTransformArray data = multiAimConstraint.data.sourceObjects;
        //
        //if (target != null)
        //    data.Add(new WeightedTransform(target, 1));
        //else
        //    data.Clear();
        //
        //multiAimConstraint.data.sourceObjects = data;
        //rigBuilder.Build();
    }
}
