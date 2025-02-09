using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject destroyEffect;
    public float speed;
    public AudioSource bulletSound;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
        rb.angularVelocity = (Random.value > 0.5 ? 1 : -1) * (0.25f + Random.value * 0.25f) * 180 * 5;
        rb.angularDamping = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (bulletSound) {
            bulletSound.Play();
        }
        GameObject effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
        if (other.gameObject.CompareTag("Crystal"))
        {
            other.gameObject.GetComponent<Crystal>().Damage();
        }
        else 
        {
            var entity = other.gameObject.GetComponent<Entity>();
            if (entity != null)
            {
                entity.Damage(1, rb.linearVelocity.normalized);
            }
        }

        Destroy(this.gameObject);
    }
}