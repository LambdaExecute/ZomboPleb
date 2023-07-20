using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public UnityEvent onDead;
    public UnityEvent onWin;

    public new Transform transform { get; private set; }
    public bool isSizeCritical { get => transform.localScale.x <= criticalAxis; }

    private int distanceToTarget = 8;
    private int playerAcceleration = 2;

    private float bulletMinSize = 0.4f;
    private float criticalAxis { get => bulletMinSize * 0.15f; }
    private int possibleShotsCount { get => Mathf.CeilToInt((transform.localScale.x - bulletMinSize) / criticalAxis) - 1; }

    private Vector3 criticalSize;

    private GameObject bullet;
    private Transform target;
    private PlayerUI playerUI;
    
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
            
            if(Input.GetMouseButton(0))
            {
                moment += Time.deltaTime / 1.1f;
                if (moment >= 0.25f)
                {
                    finalBulletSize += Time.deltaTime;
                    playerUI.UpdateScale(finalBulletSize, transform.localScale.x);
                }
            }

            if(Input.GetMouseButtonUp(0))
            {
                if (finalBulletSize > (transform.localScale - criticalSize).x)
                    Lose();

                Fire(finalBulletSize > bulletMinSize ? finalBulletSize : bulletMinSize);
                finalBulletSize = bulletMinSize;
                moment = 0;

                playerUI.UpdateScale(0, transform.localScale.x);
                playerUI.UpdatePossibleCountOfShots(possibleShotsCount);
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
        Instantiate(bullet, bulletSpawnPoint.transform.position, Quaternion.identity).GetComponent<Bullet>().Init(bulletSize);
    }

    private float GetKoefBy(float bulletSize)
    {
        float result = bulletSize * 0.15f;

        if (transform.localScale.x > bulletMinSize * 2 && bulletSize > transform.localScale.x / 2)
            result = bulletSize * 0.5f;

        return result;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Zombie zombie))
            Lose();
    }
}
