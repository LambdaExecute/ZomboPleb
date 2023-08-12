using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierBullet : Bullet
{
    public override void Init(Vector3 toTarget)
    {
        base.Init(toTarget);
        rigidbody.velocity = toTarget * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Zombie zombie))
        {
            zombie.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
