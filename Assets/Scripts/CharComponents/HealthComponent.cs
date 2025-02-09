using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private int MaxHealth = 5;
    [SerializeField] private int MaxShield = 3;
    [SerializeField] private float ShieldRegenRate = 1;
    private int CurrentHealth;
    private int CurrentShield;
    private bool IsRegen;

    private void Start()
    {
        CurrentHealth = MaxHealth;
        CurrentShield = MaxShield;
    }
    public void DamageCharacter(int damage)
    {
        print("health before " + CurrentHealth +", shield: " + CurrentShield + ", damage: " + damage);
        if (damage < CurrentShield)
        {
            CurrentShield -= damage;
        }
        else if (damage > CurrentShield)
        {
            CurrentHealth = CurrentHealth + CurrentShield - damage;
            if (CurrentHealth <= 0)
            {
                Die();
            }
            CurrentShield = 0; // if character is not dead:
        }
        if (!IsRegen)
        {
            IsRegen = true;
            InvokeRepeating("RegenerateShield", ShieldRegenRate, ShieldRegenRate);
        }
    }


    private void RegenerateShield()
    {
        CurrentShield += 1;

        if (CurrentShield >= MaxShield)
        {
            IsRegen = false;
            CancelInvoke("RegenerateShield");
        }
    }
    private void Die()
    {
        // add death logic here
        print("i am dead");

        Destroy(gameObject);
    }

}
