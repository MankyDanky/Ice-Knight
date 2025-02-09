using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    [SerializeField] private float speed = 7.5f;
    [SerializeField] public int health = 5;
    [SerializeField] public int maxHealth = 5;
    [SerializeField] public bool autoLook = true;
    private int _damageFlashes = 0;
    private float _damageFlashTimer = 0;
    [SerializeField] public bool locked = false;
    [SerializeField] public Transform ghostEmoji;

    private Vector2 moveVel = Vector2.zero;
    private Rigidbody2D rigidBody;
    private Animator animator;
    private Vector2 _knockbackVel = new Vector2(0, 0);
    private SpriteRenderer spriteRenderer;
    [SerializeField] public AudioSource damageSound;
    [SerializeField] public AudioSource dieSound;

    private bool _dieSoundPlayed = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        rigidBody = GetComponent<Rigidbody2D>();
        if (rigidBody == null)
        {
            rigidBody = gameObject.AddComponent<Rigidbody2D>();
            rigidBody.mass = 0;
        }

        rigidBody.gravityScale = 0.0f;
        rigidBody.freezeRotation = true;
        rigidBody.interpolation = RigidbodyInterpolation2D.Interpolate;
        rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void SummonGhost()
    {
        Debug.Log("Attempted to summon ghost");
        if (ghostEmoji != null)
        {
            var obj = Instantiate(ghostEmoji);
            obj.transform.position = transform.position;
        }
        else
        {
            Debug.Log("Failed.");
        }
    }

    public void Move(double deltaX, double deltaY)
    {
        if (locked)
        {
            return;
        }

        deltaX = Math.Max(-1, Math.Min(1, deltaX));
        deltaY = Math.Max(-1, Math.Min(1, deltaY));


        var magnitude = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        if (magnitude > 1)
        {
            deltaX /= magnitude;
            deltaY /= magnitude;
        }

        if (magnitude < 0.02)
        {
            deltaX = 0;
            deltaY = 0;
        }

        moveVel = new Vector2((float)deltaX, (float)deltaY);
    }

    private void Update()
    {
        if (locked) return;
        _damageFlashTimer += Time.fixedDeltaTime;
        if (_damageFlashTimer >= 1 / 12.0f)
        {
            _damageFlashTimer = 0;
            if (_damageFlashes > 0)
            {
                _damageFlashes--;
                if (_damageFlashes % 2 == 0)
                {
                    spriteRenderer.color = Color.white;
                }
                else
                {
                    spriteRenderer.color = Color.red;
                }
            }
        }

        if (IsDead() && !_dieSoundPlayed && dieSound != null)
        {
            _dieSoundPlayed = true;
            dieSound.Play();
        }

        if (animator == null) animator = GetComponent<Animator>();
        if (animator != null)
        {
            var logWarning = animator.logWarnings;
            animator.logWarnings = false;
            animator.SetFloat("vel", moveVel.magnitude);
            animator.logWarnings = logWarning;
        }

        if (autoLook)
        {
            if (moveVel.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (moveVel.x < 0)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (locked)
        {
            rigidBody.simulated = false;
            return;
        }

        rigidBody.simulated = true;

        rigidBody.linearVelocity = moveVel * speed + _knockbackVel;
        _knockbackVel -= _knockbackVel * Time.fixedDeltaTime * 5;
    }

    public void Damage(int damage, Vector2 knockback = new Vector2())
    {
        if (locked) return;

        damageSound.Play();


        _damageFlashTimer = 0;
        _damageFlashes = 4;

        health -= damage;
        rigidBody.linearVelocity /= 2;
        _knockbackVel += knockback;
    }

    public bool IsDead()
    {
        return health <= 0;
    }
}