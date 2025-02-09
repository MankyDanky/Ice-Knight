using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    public Sprite weaponSprite;
    public int damage;
    public GameObject projectile;
    public GameObject fireEffect;
    public Vector2 fireOffset;
    SpriteRenderer weaponRenderer;
    SpriteRenderer flashRenderer;
    Animator weaponAnimator;
    bool canShoot;
    public float cooldown;
    public float flashDuration;
    public int projectileCount;
    public float spread;
    public bool iceWeapon;
    private Transform _camera;
    PlayerController playerController;
    public AudioSource shootSound;
    public AudioSource bulletSound;
    [SerializeField] AudioSource noAmmoSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        canShoot = true;
        weaponRenderer = GetComponentInChildren<SpriteRenderer>();
        weaponAnimator = GetComponentInChildren<Animator>();
        flashRenderer = weaponRenderer.transform.GetChild(0).GetComponent<SpriteRenderer>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot && (!iceWeapon || playerController.ice > 0))
        {
            if (iceWeapon) {
                playerController.ice -= 1;
            }
            if (weaponRenderer.flipY) {
                for (int i = 0; i < projectileCount; i++) {
                    GameObject p = Instantiate(projectile, transform.position + transform.right*fireOffset.x + -transform.up*fireOffset.y, transform.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-spread, spread)));
                    Projectile pp = p.GetComponent<Projectile>();
                    pp.bulletSound = bulletSound;
                }
            } else {
                for (int i = 0; i < projectileCount; i++) {
                    GameObject p =Instantiate(projectile, transform.position + transform.right*fireOffset.x + transform.up*fireOffset.y, transform.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-spread, spread)));
                    p.GetComponent<Projectile>().bulletSound = bulletSound;
                }
            }

            if (projectileCount > 3)
            {
                if (_camera == null)
                {
                    _camera = GameObject.Find("Main Camera").transform;
                }

                for (int i = 0; i < projectileCount; i++)
                {
                    _camera.GetComponent<BasicCamFollow>().ApplyShake(Random.value * 360);
                }
            }
            shootSound.Play();
            weaponAnimator.ResetTrigger("Shoot");
            weaponAnimator.SetTrigger("Shoot");
            StartCoroutine(Cooldown());
            StartCoroutine(Flash());
        } else if (Input.GetMouseButtonDown(0) && canShoot && iceWeapon && playerController.ice <= 0) {
            noAmmoSound.Play();
        }
    }

    IEnumerator Cooldown() {
        canShoot = false;
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    IEnumerator Flash() {
        flashRenderer.enabled = true;
        yield return new WaitForSeconds(flashDuration);
        flashRenderer.enabled = false;
    }
}
