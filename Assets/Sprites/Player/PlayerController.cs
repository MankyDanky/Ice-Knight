using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Entity entity;
    [SerializeField] public Image healthBar;
    [SerializeField] public Image iceBar;
    [SerializeField] public TMP_Text healthText;
    [SerializeField] public TMP_Text iceText;
    [SerializeField] int maxHealthBarWidth;
    [SerializeField] int maxIceBarWidth;
    [SerializeField] public Transform weapon;
    [SerializeField] public SpriteRenderer weaponRenderer;
    [SerializeField] SpriteRenderer playerRenderer;
    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;
    public bool isPrimaryWeapon = true;
    Vector3 weaponOffset;
    [SerializeField] public Transform melee;
    public int ice;
    public int maxIce;
    private List<Transform> _meleeObjects = new();
    [SerializeField] public Sprite deadBodyImage;
    [SerializeField] public Transform deathScreen;
    [SerializeField] public Transform hurtScreen;
    public Image[] weaponIcons;
    [SerializeField] public AudioSource itemCollectSound;
    [SerializeField] AudioSource meleeSound;

    private int _healthCache = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRenderer = GetComponent<SpriteRenderer>();
        entity = GetComponent<Entity>();
        entity.autoLook = false;
        healthText = GameObject.FindWithTag("HealthbarText").GetComponent<TMP_Text>();
        iceText = GameObject.FindWithTag("IcebarText").GetComponent<TMP_Text>();
        healthBar = GameObject.FindWithTag("Healthbar").GetComponent<Image>();
        iceBar = GameObject.FindWithTag("Icebar").GetComponent<Image>();
        maxHealthBarWidth = (int)healthBar.rectTransform.sizeDelta.x;
        maxIceBarWidth = (int)iceBar.rectTransform.sizeDelta.x;
        weapon = GameObject.FindWithTag("Weapon")?.transform;
        if (weapon != null)
        {
            weaponRenderer = weapon.GetComponentInChildren<SpriteRenderer>();
            weaponOffset = weapon.localPosition;
        }
        else
        {
            Debug.LogWarning("No weapon was assigned to player!");
        }

        entity = GetComponent<Entity>();
        if (entity == null)
        {
            gameObject.AddComponent<Entity>();
            Debug.LogWarning("No BaseEntity attached to Player Object: " + gameObject.name + " (Auto-attached)");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (entity.health < _healthCache)
        {
            if (hurtScreen != null)
            {
                Instantiate(hurtScreen);
            }
        }

        _healthCache = entity.health;

        double dx = 0;
        if (Input.GetKey(KeyCode.D))
        {
            dx = 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            dx = -1;
        }

        double dy = 0;
        if (Input.GetKey(KeyCode.W))
        {
            dy = 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            dy = -1;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (melee != null)
            {

                meleeSound.Play();
                Transform obj = Instantiate(melee, transform.position, Quaternion.identity);
                obj.GetComponent<SpriteRenderer>().flipX = playerRenderer.flipX;
                MeleeAttack projectile = obj.GetComponent<MeleeAttack>();
                projectile.firedByPlayer = true;
                projectile.knockbackForce = new Vector2(playerRenderer.flipX ? -1 : 1, 0);

                _meleeObjects.Add(obj);
            }
            else
            {
                Debug.LogWarning("Melee Prefab not assigned!");
            }
        }

        _meleeObjects.RemoveAll(transform1 => transform1 == null || transform1.IsDestroyed());
        foreach (var obj in _meleeObjects)
        {
            obj.position = transform.position +
                           new Vector3(obj.GetComponent<SpriteRenderer>().flipX ? -0.5f : 0.5f, 0, 0);
        }

        entity.Move(dx, dy);

        // Point weapon towards mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 lookDir = mousePos - transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        if (weapon != null)
        {
            if (angle > 90 || angle < -90)
            {
                weapon.rotation = Quaternion.Euler(0, -180, 180 - angle);
                playerRenderer.flipX = true;
                weapon.transform.localPosition = new Vector3(-0.0196f, 0.0529f, -2f);
            }
            else
            {
                weapon.rotation = Quaternion.Euler(0, 0, angle);
                playerRenderer.flipX = false;
                weapon.transform.localPosition = new Vector3(0.0196f, 0.0529f, -2f);
            }
        }


        // Update health and ice bar
        healthText.text = entity.health + "/" + entity.maxHealth;
        iceText.text = ice + "/" + maxIce;
        if (Math.Abs(maxHealthBarWidth * entity.health / entity.maxHealth - healthBar.rectTransform.sizeDelta.x) >
            0.01 ||
            Math.Abs(maxIceBarWidth * ice / maxIce - iceBar.rectTransform.sizeDelta.x) > 0.01)
        {
            healthBar.rectTransform.sizeDelta = new Vector2(
                (maxHealthBarWidth * entity.health / entity.maxHealth - healthBar.rectTransform.sizeDelta.x) *
                Time.deltaTime * 3f + healthBar.rectTransform.sizeDelta.x, healthBar.rectTransform.sizeDelta.y);
            iceBar.rectTransform.sizeDelta = new Vector2(
                (maxIceBarWidth * ice / maxIce - iceBar.rectTransform.sizeDelta.x) *
                Time.deltaTime * 3f + iceBar.rectTransform.sizeDelta.x, iceBar.rectTransform.sizeDelta.y);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchWeapon();
        }
    }

    private void FixedUpdate()
    {
        if (entity.IsDead())
        {
            GameObject body = new GameObject();
            body.name = name + "(Dead)";
            body.transform.localScale = transform.localScale;
            body.transform.position = transform.position;
            var renderer = body.AddComponent<SpriteRenderer>();
            renderer.sprite = deadBodyImage;
            renderer.flipX = transform.rotation.x < 0;
            var rigidBody = body.AddComponent<Rigidbody2D>();
            rigidBody.linearVelocity = GetComponent<Rigidbody2D>().linearVelocity;
            rigidBody.gravityScale = 0;
            rigidBody.linearDamping = 2f;
            var light = body.AddComponent<Light2D>();
            light.color = Color.white;
            light.intensity = 0.2f;
            light.falloffIntensity = 0;
            light.pointLightOuterRadius = 100f;
            light.color = new Color(1, 0.3f, 0.21f, 1);

            if (deathScreen != null)
            {
                Instantiate(deathScreen);
            }
            else
            {
                Debug.LogWarning("No death screen was set!");
            }

            Destroy(gameObject);
        }
    }

    public void SwitchWeapon()
    {
        foreach (Image weaponIcon in weaponIcons)
        {
            weaponIcon.sprite = weapon.GetComponent<Weapon>().weaponSprite;
            weaponIcon.rectTransform.sizeDelta = new Vector2(weaponIcon.sprite.rect.width, weaponIcon.sprite.rect.height);
        }
        Destroy(weapon.gameObject);
        weapon = Instantiate(isPrimaryWeapon ? secondaryWeapon : primaryWeapon, transform).transform;
        weaponRenderer = weapon.GetComponentInChildren<SpriteRenderer>();
        isPrimaryWeapon = !isPrimaryWeapon;
    }
}