using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int lifeTime = 1;
    private float speed = 50;
    private int explousionMultiplier = 10;
    private int minParticlesCount = 5;

    public float radius => transform.localScale.x * 0.5f;
    
    private new Rigidbody rigidbody;
    private new Transform transform;
    private TrailRenderer trailRenderer;
    private GameObject bulletParticles;

    public void Init(float size)
    {
        rigidbody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        trailRenderer = GetComponent<TrailRenderer>();
        bulletParticles = Resources.Load<GameObject>("Prefabs/BulletParticles");

        transform.localScale = new Vector3(size, size, size);
        trailRenderer.startWidth = size;
        rigidbody.velocity = Vector3.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out Zombie targetZombie))
        {
            Collider[] nearestColliders = Physics.OverlapSphere(transform.localPosition, radius * explousionMultiplier * 2).Where(c => c.GetComponent<Zombie>() != null).ToArray();
            Zombie[] nearestZombies = nearestColliders.ToList().Select(c => c.GetComponent<Zombie>()).ToArray();

            foreach (Zombie zombie in nearestZombies)
            {
                zombie.Kill();
                zombie.spine.AddExplosionForce(5000 + 25000 * radius, transform.position, radius * explousionMultiplier);
            }
            Instantiate(bulletParticles, transform.position, Quaternion.identity).GetComponent<BulletParticles>().Init(minParticlesCount + Mathf.RoundToInt(radius * 2 * Random.Range(4, 10)));
            Destroy(gameObject);
        }
    }
}
