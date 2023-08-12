using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private new Transform transform;
    private Bounds zombieSpawnField;

    public void Init(Bounds zombieSpawnField)
    {
        this.zombieSpawnField = zombieSpawnField;
        transform = GetComponent<Transform>();
        Vector3 position = zombieSpawnField.ClosestPoint(FindObjectOfType<Player>().transform.position);
        position.y = zombieSpawnField.min.y;
        transform.position = position;
        StartCoroutine(IMove());
    }

    private IEnumerator IMove()
    {
        while (zombieSpawnField.Contains(transform.position))
        {
            yield return null;
            transform.position += Vector3.forward * Time.deltaTime * 5;
        }
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Zombie zombie))
            StopAllCoroutines();
    }

    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(IMove());
    }
}
