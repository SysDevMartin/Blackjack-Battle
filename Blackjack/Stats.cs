using Spectre.Console;

namespace Blackjack;

public class Stats
{
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public int DamageMultiplier { get; private set; }
    public bool IsAlive => CurrentHealth > 0;
    public bool IsDoubleDownActive { get; set; }

    public Stats(int maxHealth, int damageMultiplier)
    {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
        DamageMultiplier = damageMultiplier;
    }

    public void Reset(int maxHealth, int damageMultiplier)
    {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
        DamageMultiplier = damageMultiplier;
    }

    public int GetDamage(int cardSum)
    {
        return cardSum * DamageMultiplier;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Math.Clamp(CurrentHealth, 0, MaxHealth);
    }

    public void IncreaseDamageMultiplier()
    {
        DamageMultiplier++;
    }
}