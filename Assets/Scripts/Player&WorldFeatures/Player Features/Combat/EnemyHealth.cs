using UnityEngine;

public class EnemyHealth : MonoBehaviour, IHittableObject
{
    [Header("Life Variables")]
    public float life;

    public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        life -= damage;

        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }
}
