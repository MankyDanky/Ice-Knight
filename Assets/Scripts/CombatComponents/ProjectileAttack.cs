using UnityEngine;

public class ProjectileAttack : AttackInstance
{
    [SerializeField] private GameObject Projectile;
    [SerializeField] private float Speed;

    private void FixedUpdate()
    {
        transform.Translate(Vector2.right * Speed * Time.fixedDeltaTime);
    }

        public override void DealDamage(Collider2D Target)
    {
        base.DealDamage(Target);
        Destroy(gameObject);
    }

}
