using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : HittableObject
{
    [Header("Life Variables")]
    public float life;

    override public void ReceiveDamage(float damage)
    {
        life -= damage;

        if (life <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
