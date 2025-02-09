using System.Collections;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public float health;
    public GameObject destroyEffect;
    public float shakeAmount;
    public float shakeSpeed;
    public float shakeTime;
    public GameObject itemDrop;
    Vector3 rootPosition;
    bool shaking;
    float timer;
    private BasicCamFollow _camControl = null;
    private GameObject _cam = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rootPosition = transform.position;
        _cam = GameObject.Find("Main Camera");
        _camControl = _cam.GetComponent<BasicCamFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shaking)
        {
            timer += Time.deltaTime;
            transform.position = new Vector3(rootPosition.x + Mathf.Sin(timer * shakeSpeed) * shakeAmount / timer,
                rootPosition.y + Mathf.Sin(timer * shakeSpeed) * shakeAmount / timer, rootPosition.z);
            if (timer >= shakeTime + 0.5f)
            {
                shaking = false;
                transform.position = rootPosition;
            }
        }
    }

    public void Damage()
    {
        health--;
        if (health <= 0)
        {
            var count = Random.Range(1, 3);
            for (int i = 0; i < count; i++)
            {
                Instantiate(itemDrop, transform.position + Vector3.back, Quaternion.identity);
            }
            Instantiate(destroyEffect, transform.position + Vector3.back, Quaternion.identity);
            if (_cam != null && _camControl != null)
            {
                var dtVec = _cam.transform.position - transform.position;
                var angle = Mathf.Atan2(dtVec.y, dtVec.x) * Mathf.Rad2Deg + 90.0f;
                for (int i = 0; i < 4; i++)
                {
                    _camControl.ApplyShake(angle + Random.value * 15, strength: 0.5f,  magnitude: 10f,  fade: 0.1f + Random.value * 0.15f, ttl: 0.33f);
                }
            }

            Destroy(gameObject);
        }
        else
        {
            shaking = true;
            timer = 0.5f;
        }
    }
}