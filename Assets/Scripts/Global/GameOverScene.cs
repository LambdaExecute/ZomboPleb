using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScene : MonoBehaviour
{
    [SerializeField] private GameObject attackingZombie;
    [SerializeField] private GameObject pleb;
    [Space]
    [SerializeField] private ParticleSystem plebParticles;
    [Space]
    [Min(0.5f)]
    [SerializeField] private float initDelay;

    private WindowsManager windowsManager;
    
    public void Init()
    {
        StartCoroutine(IInit());
    }

    private IEnumerator IInit()
    {
        windowsManager = FindObjectOfType<WindowsManager>();
        Animator attackZombieAnimator = attackingZombie.GetComponent<Animator>();
        yield return new WaitForSeconds(initDelay);
        attackZombieAnimator.SetTrigger("Attack");
        yield return new WaitForSecondsRealtime(1.35f);
        Destroy(pleb);
        plebParticles.Play();
        yield return new WaitForSecondsRealtime(0.5f);
        windowsManager.Open(WindowType.GameOver, new MessageBox("You lose!\nTry again."));
        yield return new WaitForSecondsRealtime(1.5f);
        Destroy(plebParticles.gameObject);
    }
}
