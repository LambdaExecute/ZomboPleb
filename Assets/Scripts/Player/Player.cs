using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public UnityEvent onDead = new UnityEvent();
    public UnityEvent onWin = new UnityEvent();

    public new Transform transform { get; private set; }

    public bool isSizeCritical { get => transform.localScale.x <= criticalAxis; }

    private int distanceToTarget = 8;

    private float bulletMinSize = 0.4f;
    private float criticalAxis { get => bulletMinSize * 0.15f; }
    private int possibleShotsCount { get => Mathf.CeilToInt((transform.localScale.x - bulletMinSize) / criticalAxis) - 1; }

    private Vector3 criticalSize;

    private GameObject bullet;
    private Transform target;
    private PlayerUI playerUI;

    private bool isDown = false;

    private PlayerBullet playerBullet;

    [Min(1)]
    [SerializeField] private int playerAcceleration = 4;
    [SerializeField] private GameObject bulletSpawnPoint;

    public void Init(PlayerUI playerUI, Transform target, GameObject bullet)
    {
        this.target = target;
        this.bullet = bullet;
        this.playerUI = playerUI;

        criticalSize = new Vector3(criticalAxis, criticalAxis, criticalAxis);
        transform = GetComponent<Transform>();
    }

    public void Lose()
    {
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
        float finalBulletSize = bulletMinSize;
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
                            finalBulletSize += Time.deltaTime;
                            playerUI.UpdateScale(finalBulletSize, transform.localScale.x);
                        }
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        isDown = false;
                        if (finalBulletSize > (transform.localScale - criticalSize).x)
                            Lose();

                        Fire(finalBulletSize > bulletMinSize ? finalBulletSize : bulletMinSize);
                        finalBulletSize = bulletMinSize;
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

    private void Fire(float bulletSize)
    {
        float playerSizeKoef = GetKoefBy(bulletSize);
        
        transform.localScale -= new Vector3(playerSizeKoef, playerSizeKoef, playerSizeKoef);
        playerBullet = Instantiate(bullet, bulletSpawnPoint.transform.position, Quaternion.identity).GetComponent<PlayerBullet>();
        playerBullet.Init(bulletSize);
    }

    private float GetKoefBy(float bulletSize)
    {
        float result = bulletSize * 0.15f;

        if (transform.localScale.x > bulletMinSize * 2 && bulletSize > transform.localScale.x / 2)
            result = bulletSize * 0.5f;

        return result;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.TryGetComponent(out DeadZone zombie))
            Lose();
    }
}
