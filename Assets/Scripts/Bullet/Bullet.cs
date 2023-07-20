using System.Linq;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int lifeTime = 1;
    private float speed = 50;
    private int explousionMultiplier = 10;

    public float radius => transform.localScale.x * 0.5f;

    private new Rigidbody rigidbody;
    private new Transform transform;
    private TrailRenderer trailRenderer;

    public void Init(float size)
    {
        rigidbody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        trailRenderer = GetComponent<TrailRenderer>();
        
        transform.localScale = new Vector3(size, size, size);
        trailRenderer.startWidth = size;
        rigidbody.velocity = Vector3.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out Zombie targetZombie))
        {
            Collider[] nearestColliders = Physics.OverlapSphere(transform.localPosition, radius * explousionMultiplier).Where(c => c.GetComponent<Zombie>() != null).ToArray();
            Zombie[] nearestZombies = nearestColliders.ToList().Select(c => c.GetComponent<Zombie>()).ToArray();

            foreach(Zombie zombie in nearestZombies)
            {
                zombie.Kill();
                zombie.spine.AddExplosionForce(15000 + 25000 * radius, transform.position, radius * explousionMultiplier);
            }
            Destroy(gameObject);
        }
    }
}
