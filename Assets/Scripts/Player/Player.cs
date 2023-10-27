using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public UnityEvent onDead = new UnityEvent();
    public UnityEvent onWin = new UnityEvent();

    public new Transform transform { get; private set; }

    public bool isSizeCritical { get => playerMass * 2 < bulletMinMass; }

    private int distanceToTarget = 8;

    private float bulletMinMass = 20;
    private float playerDefaultMass = 50;
    private int possibleShotsCount { get => Mathf.FloorToInt(playerMass / bulletMinMass); }

    private float playerMass;

    private GameObject bullet;
    private Transform target;
    private PlayerUI playerUI;

    private bool isDead = false;
    private bool isDown = false;

    private PlayerBullet playerBullet;
    private Rigidbody rigidbody;

    [Min(0)]
    [SerializeField] private int playerAcceleration = 4;
    [SerializeField] private GameObject bulletSpawnPoint;
    [SerializeField] private List<Transform> _soldiersPositions;

    public List<Transform> soldiersPositions => _soldiersPositions;

    public void Init(int playerMass, PlayerUI playerUI, Transform target, GameObject bullet)
    {
        foreach (Transform soldierPos in _soldiersPositions)
            soldierPos.parent = null;

        this.playerMass = playerMass;
        this.target = target;
        this.bullet = bullet;
        this.playerUI = playerUI;

        rigidbody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();

        rigidbody.mass = playerMass;
        UpdateVizualSize();
    }

    public void Lose()
    {
        if(isDead) return;

        isDead = true;
        StopAllCoroutines();
        onDead.Invoke();
        Destroy(gameObject);
    }

    public void StartMoving()
    {
        StartCoroutine(IMoving());
    }

    private IEnumerator IMoving()
    {
        yield return null;
        float finalBulletMass = bulletMinMass;
        float moment = 0;

        playerUI.UpdateScale(0, transform.localScale.x);
        playerUI.UpdatePossibleCountOfShots(possibleShotsCount);

        while(Vector3.Distance(transform.position, target.position) >= distanceToTarget)
        {
            transform.position += Vector3.forward * playerAcceleration * Time.deltaTime;

            if (playerBullet != null)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    playerBullet.Explode(true);
                }
            }
            if (playerBullet == null)
            {
                if (Input.GetMouseButtonDown(0))
                    isDown = true;

                if (isDown)
                {
                    if (Input.GetMouseButton(0))
                    {
                        moment += Time.deltaTime / 1.1f;
                        if (moment >= 0.25f)
                        {
                            finalBulletMass += Time.deltaTime * 100;
                            playerUI.UpdateScale(finalBulletMass, playerMass);
                        }
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        isDown = false;
                        if (finalBulletMass > playerMass)
                            Lose();

                        Fire(finalBulletMass > bulletMinMass ? finalBulletMass : bulletMinMass);
                        finalBulletMass = bulletMinMass;
                        moment = 0;

                        playerUI.UpdateScale(0, transform.localScale.x);
                        playerUI.UpdatePossibleCountOfShots(possibleShotsCount);
                    }
                }
            }
            
            if (isSizeCritical)
                Lose();

            yield return null;
        }

        onWin.Invoke();
    }

    private void Fire(float bulletMass)
    {
        playerMass -= bulletMass;
        rigidbody.mass = playerMass;
        UpdateVizualSize();
        playerBullet = Instantiate(bullet, bulletSpawnPoint.transform.position, Quaternion.identity).GetComponent<PlayerBullet>();
        playerBullet.Init(bulletMass);
    }

    private void UpdateVizualSize()
    {
        float size = playerMass / playerDefaultMass * 0.7f;
        transform.localScale = new Vector3(size, size, size);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.TryGetComponent(out DeadZone zombie) && !isDead)
            Lose();
    }
}
