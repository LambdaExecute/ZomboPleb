using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticles : MonoBehaviour
{
    private ParticleSystem particleSystem;

    public void Init(int countOfParticles) 
    {
        StartCoroutine(IInit(countOfParticles));
    }

    private IEnumerator IInit(int countOfParticles)
    {
        particleSystem = GetComponent<ParticleSystem>();
        ParticleSystem.Burst burst = particleSystem.emission.GetBurst(0);
        burst.count = countOfParticles;
        particleSystem.emission.SetBurst(0, burst);
        particleSystem.Play();
        yield return new WaitForSecondsRealtime(particleSystem.main.duration);
        Destroy(gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
    }
}
