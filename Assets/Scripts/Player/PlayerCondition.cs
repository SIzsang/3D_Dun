using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakePhysicalDamage(int damage);
}

public class PlayerCondition : MonoBehaviour, IDamageable
{
    public UICondition uicondition;

    Condition Health { get { return uicondition.health; } }
    Condition Hunger { get { return uicondition.hunger; } }
    Condition Stamina { get { return uicondition.stamina; } }

    public float noHungerHealthDecay;

    public event Action OnTakeDamage;

    void Update()
    {
        Hunger.Subtract(Hunger.passiveValue * Time.deltaTime);
        Stamina.Add(Stamina.passiveValue * Time.deltaTime);
        
        if(Hunger.curValue == 0f)
        {
            Health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        if(Health.curValue == 0f)
        {
            Die();
        }
    }
    public void Heal(float amount)
    {
        Health.Add(amount);
    }

    public void Eat(float amount)
    {
        Hunger.Add(amount);
    }

    private void Die()
    {
        Debug.Log("Die");
    }

    public void TakePhysicalDamage(int damage)
    {
        Health.Subtract(damage);
        OnTakeDamage?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        if(Stamina.curValue - amount < 0f)
        {
            return false;
        }
        Stamina.Subtract(amount);
        return true;
    }
}
