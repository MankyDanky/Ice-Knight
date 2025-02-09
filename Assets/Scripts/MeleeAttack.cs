using System;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] public bool firedByPlayer = false;
    [SerializeField] public int damage = 1;
    [SerializeField] public Vector2 knockbackForce = Vector2.zero;
    [SerializeField] public float timeToLive = -1f;
    [SerializeField] public List<Entity> damagedEntities = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (timeToLive != -1)
        {
            timeToLive -= Time.fixedDeltaTime;
            timeToLive = Mathf.Max(0, timeToLive);
            if (timeToLive == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Entity entity = other.gameObject.GetComponent<Entity>();
        if (entity != null && !damagedEntities.Contains(entity))
        {
            if (firedByPlayer)
            {
                if (other.gameObject.CompareTag("Player"))
                    return;
            }
            else
            {
                if (!other.gameObject.CompareTag("Player"))
                    return;
            }

            entity.Damage(damage, knockbackForce);
            damagedEntities.Add(entity);
        }
    }
}