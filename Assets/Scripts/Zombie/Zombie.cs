using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public new Transform transform { get; private set; }

    private float minScale = 1.75f;
    private float maxScale = 2.25f;
    private Animator zombieAnimator;

    public bool isDead { get; private set; }
    private Rigidbody[] allRigidbodies;
    public Rigidbody spine { get; private set; }

    public void Init(List<GameObject> zombiesModels, int modelID)
    {
        List<RuntimeAnimatorController> zombieAnimations = Resources.LoadAll<RuntimeAnimatorController>("Prefabs/ZombieAnimations").ToList();
        transform = GetComponent<Transform>();
        GameObject zombieModel = Instantiate(zombiesModels[modelID], transform.position, Quaternion.identity);
        allRigidbodies = zombieModel.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rigidbody in allRigidbodies)
        {
            rigidbody.gameObject.layer = 8;
        }

        zombieAnimator = zombieModel.GetComponent<Animator>();
        zombieAnimator.runtimeAnimatorController = zombieAnimations[Random.Range(0, zombieAnimations.Count)];
        StartCoroutine(IWaitForStartAnimation());
        SetPhyisical(false);

        float randomScale = Random.Range(minScale, maxScale);

        zombieModel.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        zombieModel.transform.parent = transform;
        zombieModel.transform.localPosition = Vector3.zero;

        spine = GetComponentInChildren<ZombieSpine>().GetComponent<Rigidbody>();
    }

    private IEnumerator IWaitForStartAnimation()
    {
        float delay = Random.Range(0.5f, 1.5f);
        yield return new WaitForSecondsRealtime(delay);
        zombieAnimator.SetTrigger("Animate");
        zombieAnimator.speed = Random.Range(0.25f, 2);
    }

    private void SetPhyisical(bool isKinematic)
    {
        if (isKinematic)
            Destroy(zombieAnimator);
        foreach(Rigidbody rigidbody in allRigidbodies)
        {
            rigidbody.isKinematic = !isKinematic;
        }
    }

    public void Kill()
    {
        isDead = true;
        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<Rigidbody>());
        SetPhyisical(true);
        StartCoroutine(IWaitForDestroyPhyisics());
    }

    private IEnumerator IWaitForDestroyPhyisics()
    {
        yield return new WaitForSecondsRealtime(4);
        yield return new WaitUntil(() => transform.position.y <= 0.5f);
        SetPhyisical(false);
    }
}
