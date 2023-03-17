using System.Collections.Generic;
using UnityEngine;

public class BarrierGenerator : MonoBehaviour
{
    private int barrierLevelsCount = 10;

    private float getRandXForSingle { get => Random.Range(-1.1f, 1.1f); }
    private float getRandXForPair { get => Random.Range(0.65f, 1.7f); }

    private float startPosZ = 20;
    
    private List<bool> barrierTwoness = new List<bool>();
    private List<GameObject> barriers = new List<GameObject>();

    private GameObject barrierPrefab;

    public void Init(GameObject barrierPrefab)
    {
        this.barrierPrefab = barrierPrefab;
        GenerateBarriers();
    }

    public void ClearBarriers()
    {
        while (barrierTwoness.Count != 0)
            barrierTwoness.RemoveAt(0);

        while (barriers.Count != 0)
        {
            Destroy(barriers[0]);
            barriers.RemoveAt(0);
        }
    }

    private void GenerateBarriers()
    {
        for (int i = 0; i < barrierLevelsCount; i++)
        {
            int rand = Random.Range(1, i + Random.Range(1, 4));
            barrierTwoness.Add(!(i % rand == 0));
        }

        for (int i = 0; i < barrierTwoness.Count; i++)
        {
            float rand = Random.Range(15, 20);
            float finalPosZ = i == 0 ? startPosZ : 10 * i + rand;
            
            Vector3 firstPos = new Vector3(barrierTwoness[i] ? getRandXForPair : getRandXForSingle, 1.5f, finalPosZ);
            GameObject firstBarrier = Instantiate(barrierPrefab, firstPos, Quaternion.identity);
            barriers.Add(firstBarrier);
            if (barrierTwoness[i])
            {
                Vector3 secondPos = firstPos;
                secondPos.x *= -1;
                GameObject secondBarrier = Instantiate(barrierPrefab, secondPos, Quaternion.identity);
                barriers.Add(secondBarrier);
            }
        }

        foreach (GameObject barrier in barriers)
            Destroy(barrier.GetComponent<Rigidbody>());
    }
}
