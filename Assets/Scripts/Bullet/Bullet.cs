using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int lifeTime = 1;
    private float speed = 50;

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
        if(collision.gameObject.TryGetComponent(out Barrier barrier))
        {
            barrier.Infect();
            Destroy(gameObject);
        }
    }
}
