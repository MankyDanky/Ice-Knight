using System.Collections;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float time = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Destruct());
    }

    IEnumerator Destruct()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
