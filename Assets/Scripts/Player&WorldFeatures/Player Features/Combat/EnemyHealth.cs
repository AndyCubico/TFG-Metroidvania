using UnityEngine;

public class EnemyHealth : MonoBehaviour, IHittableObject
{
    [Header("Life Variables")]
    public float life;

    public AttackFlagType flagMask;

    public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        if((flag & flagMask) != 0)
        {
            life -= damage;

            if (life <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
