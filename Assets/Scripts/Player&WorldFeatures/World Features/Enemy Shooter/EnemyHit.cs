using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    public float damage;
    public bool canBeParried;
    public bool hasHittedPlayer;

    public Enemy enemy;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }
}
