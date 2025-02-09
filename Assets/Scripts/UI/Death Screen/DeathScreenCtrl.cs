using UnityEngine;

public class DeathScreenCtrl : MonoBehaviour
{
    private float _offsetY = 500;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localPosition = new Vector3(0, _offsetY, 0);
    }

    // Update is called once per frame
    void Update()
    {
        _offsetY -= _offsetY * Time.deltaTime * 5;
        transform.localPosition = new Vector3(0, _offsetY, 0);
    }
}