using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Life Variables")]
    public float life;

    private void Update()
    {
        if(life <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void RecieveDamage(float damage)
    {
        life -= damage;
    }
}
