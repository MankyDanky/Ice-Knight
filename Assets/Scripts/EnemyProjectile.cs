using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyProjectile : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject destroyEffect;
    public List<GameObject> ignoreList = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.angularVelocity = (Random.value > 0.5 ? 1 : -1) * (0.25f + Random.value * 0.25f) * 180 * 5;
        rb.angularDamping = 0;
    }


    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool shouldDestroy = false;
        if (ignoreList.Contains(other.gameObject) || other.GetComponent<RoomTrigger>() != null)
        {
            return;
        }

        if (other.gameObject.CompareTag("Crystal"))
        {
            other.gameObject.GetComponent<Crystal>().Damage();
            shouldDestroy = true;
        }
        else
        {
            var entity = other.gameObject.GetComponent<Entity>();
            if (entity != null)
            {
                if (entity.gameObject.CompareTag("Player"))
                {
                    entity.Damage(1, rb.linearVelocity.normalized);
                    shouldDestroy = true;
                }
            }
            else
            {
                shouldDestroy = true;
            }
        }

        if (shouldDestroy)
        {
            GameObject effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}