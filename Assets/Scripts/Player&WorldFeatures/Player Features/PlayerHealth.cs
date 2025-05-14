using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float playerHealth;

    private void OnEnable()
    {
        HealthEvents.TakingDamage += ReceiveAnAttack;
    }

    private void OnDisable()
    {
        HealthEvents.TakingDamage -= ReceiveAnAttack;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            HealthEvents.TakingDamage?.Invoke(20);
        }
    }

    void ReceiveAnAttack(float damage)
    {
        playerHealth -= damage;
        Debug.Log("Player Has been hit with: " + damage);

        if (playerHealth <= 0) 
        {
            Debug.Log("Player Has died");
        }
    }
}
