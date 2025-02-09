using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class EnemyAI : MonoBehaviour
{
    class AIAction
    {
        private float _delayTimer;
        private float _durationTimer;
        private readonly Vector2 _move;
        public readonly Transform attackingPlayer;
        public readonly Transform self;

        public AIAction(Transform self, float delay, float duration, float dx, float dy,
            Transform attackingPlayer = null)
        {
            _delayTimer = delay;
            this.self = self;
            this.attackingPlayer = attackingPlayer;
            _durationTimer = duration;
            _move = new Vector2(dx, dy);
        }

        public void Perform(EnemyAI enemyAI)
        {
            if (attackingPlayer != null)
            {
                if ((attackingPlayer.position - self.position).sqrMagnitude < 1.5f)
                {
                    enemyAI.entity.Move(0, 0);
                    return;
                }
            }

            if (_delayTimer > 0)
            {
                enemyAI.entity.Move(0, 0);
                _delayTimer -= Time.fixedDeltaTime;
                if (_delayTimer < 0) _delayTimer = 0; // Prevents going negative
            }
            else if (_durationTimer > 0)
            {
                _durationTimer -= Time.fixedDeltaTime;
                if (_durationTimer < 0) _durationTimer = 0; // Prevents going negative
                enemyAI.entity.Move(_move.x, _move.y);
            }
        }

        public bool IsDone()
        {
            return _delayTimer <= 0 && _durationTimer <= 0;
        }

        public bool IsCloseEnoughToMelee()
        {
            if (attackingPlayer != null)
            {
                if ((attackingPlayer.position - self.position).sqrMagnitude < 1.8f)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public float viewDistance = 6;
    public Entity entity;
    private AIAction _currentAction; // Changed to Queue for better processing
    [SerializeField] public Entity player;
    [SerializeField] public Sprite deadBodyImage;
    [SerializeField] public Transform meleePrefab;
    private SpriteRenderer _spriteRenderer;
    public GameObject itemDrop;
    public int maxDropCount = 1;

    private float _meleeTimer = 0;
    private float _projectileTimer = 0;

    [SerializeField] public float attackCooldown = 1.25f;
    [SerializeField] public float actionCooldownMultiplier = 1;

    [SerializeField] public Transform projectile;
    [SerializeField] public float projectileFireRate = 0.5f;
    [SerializeField] public float projectileSpeed = 10f;

    [FormerlySerializedAs("canInstantAttack")] [SerializeField]
    public bool canImmediatelyAttack = false;

    void Start()
    {
        entity = GetComponent<Entity>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (entity.locked)
        {
            return;
        }

        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null)
                player = obj.GetComponent<Entity>();
        }

        if (entity != null && entity.IsDead())
        {
            if (deadBodyImage != null)
            {
                GameObject body = new GameObject();
                body.name = name + "(Dead)";
                body.transform.localScale = transform.localScale;
                body.transform.position = transform.position;
                body.AddComponent<DeadBodyDisappear>();
                var renderer = body.AddComponent<SpriteRenderer>();
                renderer.sprite = deadBodyImage;
                renderer.flipX = _spriteRenderer.flipX;
                var rigidBody = body.AddComponent<Rigidbody2D>();
                rigidBody.linearVelocity = GetComponent<Rigidbody2D>().linearVelocity;
                rigidBody.gravityScale = 0;
                rigidBody.linearDamping = 2f;
            }

            for (int i = 0; i < maxDropCount; i++)
            {
                if (Random.value > 0.25f && itemDrop != null)
                {
                    entity.SummonGhost();
                    Instantiate(itemDrop, transform.position + Vector3.back, Quaternion.identity);
                }
            }


            Destroy(gameObject);
        }

        bool shouldApproachPlayer = false;
        bool playerVisible = false;
        if (player != null)
        {
            playerVisible = (player.transform.position - transform.position).sqrMagnitude <
                            (viewDistance * viewDistance);
            if (playerVisible && projectile == null)
            {
                shouldApproachPlayer = true;
            }
        }

        if (canImmediatelyAttack)
        {
            _meleeTimer += Time.fixedDeltaTime;
        }

        if (_currentAction != null && _currentAction.attackingPlayer != null)
        {
            if (projectile != null)
            {
                _projectileTimer += Time.fixedDeltaTime;
                if (_projectileTimer > 1 / projectileFireRate)
                {
                    _projectileTimer = Random.value * 0.25f - 0.125f;

                    var diff = (player.transform.position + new Vector3(0, 1, 0)) - (transform.position + new Vector3(0, 0.65f, 0));
                    diff = diff.normalized + new Vector3(Random.value * 0.1f - 0.05f, Random.value * 0.1f - 0.05f, 0);
                    diff = diff.normalized;
                    _spriteRenderer.flipX = player.transform.position.x < transform.position.x;
                    var obj = Instantiate(projectile, transform.position + new Vector3(0, 0.65f, 0) + diff * 0.65f, Quaternion.identity);
                    obj.GetComponent<Rigidbody2D>().linearVelocity = diff * projectileSpeed;
                    obj.GetComponent<EnemyProjectile>().ignoreList.Add(gameObject);
                }
            }

            if (_currentAction.IsCloseEnoughToMelee())
            {
                if (!canImmediatelyAttack)
                {
                    _meleeTimer += Time.fixedDeltaTime;
                }

                if (_meleeTimer >= attackCooldown)
                {
                    if (meleePrefab != null)
                    {
                        _spriteRenderer.flipX = _currentAction.attackingPlayer.position.x < transform.position.x;
                        Transform obj = Instantiate(meleePrefab,
                            transform.position + new Vector3(_spriteRenderer.flipX ? -0.25f : 0.25f, 0, 0),
                            Quaternion.identity);
                        obj.GetComponent<SpriteRenderer>().flipX = _spriteRenderer.flipX;
                        MeleeAttack projectile = obj.GetComponent<MeleeAttack>();
                        projectile.firedByPlayer = false;
                        projectile.knockbackForce = new Vector2(_spriteRenderer.flipX ? -1 : 1, 0);
                    }
                    else
                    {
                        Debug.LogWarning("Melee Prefab not assigned!");
                    }

                    _meleeTimer = 0;
                }
            }
        }


        if (_currentAction == null || _currentAction.IsDone() ||
            (shouldApproachPlayer && _currentAction.attackingPlayer == null))
        {
            float extraDuration = 0;
            if (Random.value > 0.8f)
            {
                extraDuration = Random.Range(1.5f, 3.5f);
            }

            if (shouldApproachPlayer)
            {
                Vector3 diff = player.transform.position - transform.position;
                diff /= diff.magnitude;
                diff *= (0.5f + Random.value + 0.5f);
                diff = new Vector3(diff.x + Random.value * 0.5f - 0.25f, diff.y + Random.value * 0.5f - 0.25f, 0);
                diff /= diff.magnitude;
                _currentAction = new AIAction(
                    this.transform,
                    actionCooldownMultiplier * (Random.value * 0.5f + 0.5f),
                    Random.value * 0.2f + 0.25f + extraDuration,
                    diff.x + (diff.x > 0 ? -0.25f : 0.25f),
                    diff.y,
                    player.transform
                );
            }
            else
            {
                _currentAction = new AIAction(transform, Random.value * 3.5f + 0.5f,
                    Random.value * 0.5f + 0.25f + extraDuration,
                    Random.value * 2.0f - 1,
                    Random.value * 2.0f - 1,
                    playerVisible ? player.transform : null
                );
            }
        }

        _currentAction.Perform(this);
    }
}