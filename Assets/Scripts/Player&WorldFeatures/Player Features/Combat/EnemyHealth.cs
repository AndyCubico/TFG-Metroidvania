using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : HittableObject
{
    [Header("Life Variables")]
    public float life;

    override public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        life -= damage;

        if (life <= 0)
        {
            Destroy(this.gameObject);
        }

        //Ejemplo de como funcionan las flags
        /*
         * if (Input.GetKey(KeyCode.Space))
        {
            flag |= AttackFlagType.Saltando; // Añade el estado Saltando
        }

        if ((flag & AttackFlagType.Saltando) != 0)
        {
            Debug.Log("El jugador está saltando.");
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            flag &= ~AttackFlagType.Saltando; // Quita el estado Saltando
        }
        */
    }
}
