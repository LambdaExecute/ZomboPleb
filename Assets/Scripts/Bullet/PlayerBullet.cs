using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBullet : Bullet
{
    private GameObject bulletParticles;

    public override void Init(float size)
    {
        base.Init(size);
        bulletParticles = Resources.Load<GameObject>("Prefabs/BulletParticles");
        rigidbody.velocity = Vector3.forward * speed;
    }

    public void Explode(bool isClicked = false)
    {
        Collider[] nearestColliders = Physics.OverlapSphere(transform.position, 2 + Mathf.CeilToInt(radius * Mathf.PI)).Where(c => c.GetComponent<Zombie>() != null).ToArray();
        Zombie[] nearestZombies = nearestColliders.ToList().Select(c => c.GetComponent<Zombie>()).ToArray();

        foreach (Zombie zombie in nearestZombies)
        {
            zombie.Kill();
            zombie.physicalBody.spine.AddExplosionForce(Mathf.Pow(4/3 * Mathf.PI * Mathf.Pow(radius, 3), 2) * 50 + 10000, transform.position, 10);
            zombie.physicalBody.DestroyPhysicsWithDelay();
        }
        
        float bulletSurfaceArea = 4 * Mathf.PI * Mathf.Pow(radius, 2);
        float bulletFragmentSurfaceArea = 4 * Mathf.PI * Mathf.Pow(fragmentRadius, 2);

        Instantiate(bulletParticles, transform.position, isClicked ? Quaternion.Euler(-40, 0, 0) : Quaternion.Euler(-90, 0, 0))
                .GetComponent<BulletParticles>()
                .Init(Mathf.RoundToInt(bulletSurfaceArea / bulletFragmentSurfaceArea), radius * 0.45f);

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Zombie zombie))
            Explode();
    }
}
