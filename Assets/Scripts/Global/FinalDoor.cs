using System.Collections;
using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    private new Transform transform;

    private void Start()
    {
        transform = GetComponent<Transform>();
    }

    public void Open() => StartCoroutine(IOpen());

    public void Close()
    {
        StopAllCoroutines();
        transform.localRotation = Quaternion.identity;
    }
    private IEnumerator IOpen()
    {
        for(float i = 0; i <= 1; i += Time.deltaTime)
        {
            transform.localRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0, -90, 0), i);
            yield return null;
        }
    }
}
