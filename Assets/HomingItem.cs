using System;
using Unity.VisualScripting;
using UnityEngine;

public class HomingItem : MonoBehaviour
{
    Rigidbody2D rb;
    Transform player;
    PlayerController playerController;
    public int ice;

    public int health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.IsDestroyed())
        {
            rb.AddForce((player.position - transform.position).normalized * 5f, ForceMode2D.Force);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerController.ice = Math.Min(playerController.ice + ice, playerController.maxIce);
            playerController.entity.health =
                Math.Min(playerController.entity.health + health, playerController.entity.maxHealth);
            playerController.itemCollectSound.Play();
            Destroy(gameObject);
        }
    }
}