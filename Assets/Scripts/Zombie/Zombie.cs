using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public new Transform transform { get; private set; }

    private float health = 100;
    private float minScale = 1.75f;
    private float maxScale = 2.25f;
    private Animator zombieAnimator;

    private float currentAnimationSpeed;
    private RuntimeAnimatorController currentAnimation;

    public bool isDead => health == 0;
    private Rigidbody[] allRigidbodies;
    public Rigidbody spine { get; private set; }

    public void Init(List<GameObject> zombiesModels, int modelID)
    {
        transform = GetComponent<Transform>();
        List<RuntimeAnimatorController> zombieAnimations = Resources.LoadAll<RuntimeAnimatorController>("Prefabs/ZombieAnimations").ToList();
        zombieAnimations.Remove(zombieAnimations.Find(za => za.name == "Zombie@Damages"));
        GameObject zombieModel = Instantiate(zombiesModels[modelID], transform.position, Quaternion.identity);
        allRigidbodies = zombieModel.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rigidbody in allRigidbodies)
        {
            rigidbody.gameObject.layer = 8;
        }

        zombieAnimator = zombieModel.GetComponent<Animator>();
        zombieAnimator.runtimeAnimatorController = zombieAnimations[Random.Range(0, zombieAnimations.Count)];
        StartCoroutine(IWaitForStartAnimation());
        SetPhysical(false);

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
        currentAnimationSpeed = zombieAnimator.speed;
    }

    private void SetPhysical(bool isKinematic)
    {
        if (isKinematic)
            Destroy(zombieAnimator);
        foreach(Rigidbody rigidbody in allRigidbodies)
        {
            rigidbody.isKinematic = !isKinematic;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0)
            health = 0;

        if (health == 0)
            Kill();
    }

    public void Kill()
    {
        health = 0;
        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<Rigidbody>());
        SetPhysical(true);
        StartCoroutine(IWaitForDestroyPhyisics());
    }

    private IEnumerator IWaitForDestroyPhyisics()
    {
        yield return new WaitForSecondsRealtime(4);
        yield return new WaitUntil(() => transform.position.y <= 0.5f);
        SetPhysical(false);
    }

    public void KillByParticleHit()
    {
        StartCoroutine(IKillByParticleHit());
    }

    private IEnumerator IKillByParticleHit()
    {
        VisualInfect();
        zombieAnimator.speed = 1;
        zombieAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Prefabs/ZombieAnimations/Zombie@Damages");
        yield return new WaitForSeconds(2f);
        Kill();
    }

    public void VisualInfect()
    {
        SkinnedMeshRenderer meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        Material[] materials = new Material[2];
        materials[0] = meshRenderer.materials[0];
        materials[1] = Resources.Load<Material>("Prefabs/ZombieInfectedMaterial");
        meshRenderer.sharedMaterials = materials;
    }
}
