using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    [Space]
    [Header("Character Basics")]
    public new string name;

    [Space]
    [Header("Base Stats")]
    public int baseMaxHealth;
    public int baseArmor;
    public float baseMoveSpeed;
    public float baseRotationSpeed = 5f;

    [Space]
    [Header("Current Stats")]
    public int currentMaxHealth;
    public int currentHealth;
    public int currentArmor;
    public float currentMoveSpeed;
    public float currentRotationSpeed;

    //take damage from an attack and apply armor reduction to it
    public void TakeDamage(int damage)
    {
        damage -= currentArmor;
        damage = Mathf.Clamp(damage, 1, int.MaxValue);

        currentHealth -= damage;
        Debug.Log(name + " takes " + damage + " damage.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //kill the player
    private void Die()
    {
        Debug.Log(name + " died.");
    }

    
    public virtual void DebugMe(GameObject parent)
    {
        Debug.Log("Debug Call Activated : " + name);
    }
}
