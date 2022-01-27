using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public HealthBar healthBar;

    public Tile tile;

    private int maxHealth;
    private int currentHealth;
    private int damage;

    private void Start()
    {
        damage = GameValues.playerDamage;
        maxHealth = GameValues.maxHealth;
        currentHealth = maxHealth;

        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        healthBar.SetHealth(currentHealth);
        GameValues.damageTakenTotal += dmg;
        if (currentHealth <= 0)
        {
            tile.tileMap.PlayerDeath();
        }
    }

    public void Heal(int hp)
    {
        int healAmount = Mathf.FloorToInt(maxHealth / 100) * hp;
        currentHealth += healAmount;
        GameValues.healedTotal += healAmount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        healthBar.SetHealth(currentHealth);
    }

    public void IncreaseMaxHealth(int amount)
    {
        GameValues.maxHealth += amount;
        maxHealth = GameValues.maxHealth;
        //currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
    }

    public void IncreaseDamage(int increaseBy)
    {
        damage += increaseBy;
        GameValues.playerDamage = damage;
    }

    public int ReturnDamage()
    {
        return damage;
    }

    public int ReturnHealth()
    {
        return currentHealth;
    }

    public int ReturnMaxHealth()
    {
        return maxHealth;
    }
}
