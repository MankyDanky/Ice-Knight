using UnityEngine;

public partial class AttackInstance : MonoBehaviour
{
    [SerializeField] private int Damage;
    [SerializeField] private float Duration = 1; // melee attack should be small, projectile attack should be big

    private void Start()
    {
        Destroy(gameObject, Duration);
    }


    void OnTriggerEnter2D(Collider2D Target)
    {
        print(Target.name + " entered my range");
        DealDamage(Target);
    }
    public virtual void DealDamage(Collider2D Target)
    {
        HealthComponent TargetHealthComponent = Target.GetComponent<HealthComponent>();
        print("i am dealing " + Damage + " to the Target");
        TargetHealthComponent?.DamageCharacter(Damage);
    }
}
