using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField] protected int lifeTime;
    [SerializeField] protected float speed;
    [SerializeField] protected int explousionMultiplier;
    [SerializeField] protected float damage;

    private float bulletDefaultMass = 20;
    public virtual float radius => transform.localScale.x * 0.5f;
    protected virtual float fragmentRadius => radius >= 0.5f ? 0.3f : radius / 2;
    
    protected new Rigidbody rigidbody;
    protected new Transform transform;
    protected TrailRenderer trailRenderer;

    public virtual void Init(float mass)
    {
        rigidbody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        trailRenderer = GetComponent<TrailRenderer>();
        
        UpdateVizualSize(mass);
        trailRenderer.startWidth = transform.localScale.x;

        Destroy(gameObject, lifeTime);
    }

    public virtual void Init(Vector3 toTarget)
    {
        rigidbody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        trailRenderer = GetComponent<TrailRenderer>();

        Destroy(gameObject, lifeTime);
    }

    private void UpdateVizualSize(float mass)
    {
        float size = (mass / bulletDefaultMass) * 0.6f;
        transform.localScale = new Vector3(size, size, size);
    }
}
