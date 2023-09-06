using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticles : MonoBehaviour
{
    private ParticleSystem particleSystem;

    public void Init(int countOfParticles, float particleRadius) 
    {
        StartCoroutine(IInit(countOfParticles, particleRadius));
    }

    private IEnumerator IInit(int countOfParticles, float particleRadius)
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.startSize = particleRadius;
        ParticleSystem.Burst burst = particleSystem.emission.GetBurst(0);
        burst.count = countOfParticles;
        particleSystem.emission.SetBurst(0, burst);
        particleSystem.Play();
        yield return new WaitForSecondsRealtime(particleSystem.main.startLifetime.constant / 10);
        Destroy(gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if(zombie == null)
        {
            zombie = GetComponentInParent<Zombie>();
            if (zombie == null) return;
        }

        if (!zombie.isDead)
            zombie.KillByParticleHit();
        else
            zombie.VisualInfect(zombie.gameObject);
    }
}
