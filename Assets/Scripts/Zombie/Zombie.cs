using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zombie : MonoBehaviour, IMoneyCollectable
{
    public new Transform transform { get; private set; }

    private float health = 100;
    private float minScale = 1.75f;
    private float maxScale = 2.25f;
    private float price = 1f;
    private Animator zombieAnimator;

    private float currentAnimationSpeed;
    private int currentZombieIndex;

    private string pathToCurrentZombieData;
    
    private Vector3 currentZombieScale;

    private RuntimeAnimatorController currentAnimation;
    private GameObject zombieModel;

    public bool isDead => health == 0;

    public LODGroup lodGroup { get; private set; }
    public PhysicalZombie physicalBody { get; private set; }

    public void Init(List<GameObject> zombiesModels, int modelID)
    {
        pathToCurrentZombieData = $"Prefabs/Zombies/Zombie{modelID}/";
        currentZombieIndex = modelID;
        transform = GetComponent<Transform>();
        List<RuntimeAnimatorController> zombieAnimations = Resources.LoadAll<RuntimeAnimatorController>("Prefabs/ZombieAnimations").ToList();
        zombieAnimations.Remove(zombieAnimations.Find(za => za.name == "Zombie@Damages"));

        zombieModel = Instantiate(zombiesModels[modelID], transform.position, Quaternion.identity);
        
        zombieAnimator = zombieModel.GetComponent<Animator>();
        zombieAnimator.runtimeAnimatorController = zombieAnimations[Random.Range(0, zombieAnimations.Count)];
        currentAnimation = zombieAnimator.runtimeAnimatorController;

        StartCoroutine(IWaitForStartAnimation());
        
        float randomScale = Random.Range(minScale, maxScale);

        zombieModel.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        zombieModel.transform.parent = transform;
        zombieModel.transform.localPosition = Vector3.zero;
        currentZombieScale = zombieModel.transform.localScale;
        lodGroup = zombieAnimator.gameObject.GetComponent<LODGroup>();
        
        if(Random.Range(0, 100) <= 50)
        {
            LOD[] currentLODS = lodGroup.GetLODs();
            List<LOD> lodsList = currentLODS.ToList();
            LOD newSecondLOD = lodsList[lodsList.Count - 1];
            newSecondLOD.screenRelativeTransitionHeight = 0.01f;
            lodsList[lodsList.Count - 1] = newSecondLOD;
            
            lodGroup.SetLODs(lodsList.ToArray());
        }
        StartCoroutine(ISetAnimator());
    }

    private IEnumerator ISetAnimator()
    {
        while(!isDead)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            if(zombieAnimator != null && lodGroup != null)
                zombieAnimator.enabled = lodGroup.GetLODs()[0].renderers[0].isVisible;
        }
    }

    private IEnumerator IWaitForStartAnimation()
    {
        float delay = Random.Range(0.5f, 1.5f);
        yield return new WaitForSecondsRealtime(delay);
        zombieAnimator.SetTrigger("Animate");
        zombieAnimator.speed = Random.Range(0.25f, 2);
        currentAnimationSpeed = zombieAnimator.speed;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0)
            health = 0;

        if (health == 0)
            Kill();
    }

    public void Kill(bool isInfected = false)
    {
        Wallet.instance.Collect(GetPrice());

        health = 0;
        
        Destroy(zombieModel);
        
        physicalBody = Instantiate(Resources.Load<GameObject>("Prefabs/Zombies/Zombie0/ZombiePhysical"), transform.position, transform.localRotation).GetComponent<PhysicalZombie>();
        physicalBody.Init();

        physicalBody.transform.parent = transform;
        physicalBody.transform.localPosition = Vector3.zero;
        physicalBody.transform.localScale = currentZombieScale;

        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<Rigidbody>());

        if (isInfected)
            VisualInfect(physicalBody.gameObject);
    }

    public void KillByParticleHit()
    {
        StartCoroutine(IKillByParticleHit());
    }

    private IEnumerator IKillByParticleHit()
    {
        VisualInfect(gameObject);
        zombieAnimator.speed = 1;
        zombieAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Prefabs/ZombieAnimations/Zombie@Damages");
        yield return new WaitForSeconds(2f);
        Kill(true);
    }

    public void VisualInfect(GameObject target)
    {
        SkinnedMeshRenderer meshRenderer = target.GetComponentInChildren<SkinnedMeshRenderer>();
        Material[] materials = new Material[2];
        materials[0] = meshRenderer.materials[0];
        materials[1] = Resources.Load<Material>("Prefabs/ZombieInfectedMaterial");
        meshRenderer.sharedMaterials = materials;

        if(!isDead && lodGroup.GetLODs()[1].renderers[0].isVisible)
        {
            BillboardRenderer currentBillboard = GetComponentInChildren<BillboardRenderer>();
            if (currentBillboard != null)
            {
                GameObject infectedBillboard = Instantiate(Resources.Load<GameObject>(pathToCurrentZombieData + "ZombieInfectedBillboard"),
                                                           currentBillboard.gameObject.transform.position,
                                                           currentBillboard.gameObject.transform.localRotation);

                infectedBillboard.transform.localScale = currentBillboard.transform.localScale;
                infectedBillboard.transform.parent = zombieModel.transform; 
                
                Destroy(currentBillboard.gameObject);
                
                LOD[] lods = lodGroup.GetLODs();
                lods[1].renderers[0] = infectedBillboard.GetComponent<BillboardRenderer>();
                
                lodGroup.SetLODs(lods);
                lodGroup.RecalculateBounds();
            }
        }
    }

    public float GetPrice() => price;
}
