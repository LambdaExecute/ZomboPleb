using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalZombie : MonoBehaviour
{
    public new Transform transform { get; private set; }
    public Rigidbody spine { get; private set; }

    private Rigidbody[] allRigidbodies;

    public void Init()
    {
        transform = GetComponent<Transform>();
        allRigidbodies = GetComponentsInChildren<Rigidbody>(); 
        spine = GetComponentInChildren<ZombieSpine>().GetComponent<Rigidbody>();

        foreach (Rigidbody rigidbody in allRigidbodies)
        {
            rigidbody.gameObject.layer = 8;
        }
        SetPhysical(true);
    }

    private void SetPhysical(bool isKinematic)
    {
        foreach (Rigidbody rigidbody in allRigidbodies)
        {
            rigidbody.isKinematic = !isKinematic;
        }
    }

    private void DestroyPhysics()
    {
        CharacterJoint[] joints = GetComponentsInChildren<CharacterJoint>();
        Collider[] colliders = GetComponentsInChildren<Collider>();
        
        foreach (CharacterJoint joint in joints)
            Destroy(joint);
        foreach(Rigidbody rigidbody in allRigidbodies)
            Destroy(rigidbody);
        foreach(Collider collider in colliders)
            Destroy(collider);
    }

    public void DestroyPhysicsWithDelay() => StartCoroutine(IWaitForDestroyPhysics());

    private IEnumerator IWaitForDestroyPhysics()
    {
        yield return new WaitForSecondsRealtime(4);
        yield return new WaitUntil(() => transform.position.y <= 0.5f);
        SetPhysical(false);
        DestroyPhysics();
    }
}
