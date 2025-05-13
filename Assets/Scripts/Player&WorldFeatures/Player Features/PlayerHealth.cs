using System;
using UnityEngine;

public static class HealthEvents
{
    public static Action TakingDamage;
}

public class PlayerHealth : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0)) 
        {
            HealthEvents.TakingDamage?.Invoke();
        }
    }
}
