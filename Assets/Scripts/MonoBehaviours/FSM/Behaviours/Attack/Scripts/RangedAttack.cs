using UnityEngine;

[CreateAssetMenu(fileName = "Ranged attack", menuName = "Enemy Logic/Attack/Ranged attack")]
// This class is a placeholder for ranged attack logic.
public class RangedAttack : AttackSOBase
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float attackCooldown = 2f;
    private float m_attackTimer;

    [SerializeField] private float m_LostOfSightTime = 2f;
    [SerializeField] private float m_DistanceLimitToPlayer = 15f;
    private bool m_playerInSight = false;
    private float m_SightTimer = 0f;

    [SerializeField] private LayerMask m_VisionMask;

    public override void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType)
    {
        base.DoAnimationTrigger(triggerType);
    }

    public override void DoEnter()
    {
        base.DoEnter();
    }

    public override void DoExit()
    {
        base.DoExit();
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
    }

    public override void DoUpdate()
    {
        base.DoUpdate();

        CheckPlayerSightAndHandleState();

        if (m_playerInSight)
        {
            m_attackTimer += Time.deltaTime;

            if (m_attackTimer >= attackCooldown)
            {
                m_attackTimer = 0;

                enemy.SetTransitionAnimation("Attack");

                //Vector2 direction = (playerTransform.position - transform.position).normalized;
                //GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                //projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;
                //projectile.GetComponent<EnemyHit>().enemy = enemy;
            }
        }
    }

    public void CheckPlayerSightAndHandleState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        m_playerInSight = false;

        // Only raycast if within the sight range
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, m_VisionMask);

        Debug.DrawRay(transform.position, directionToPlayer * distanceToPlayer, Color.green);

        if (hit.collider != null && hit.collider.transform.root.CompareTag("Player"))
        {
            m_playerInSight = true;
        }

        // If player is not in sight OR too far, start the countdown
        //if (!m_playerInSight || distanceToPlayer > m_DistanceLimitToPlayer)
        //{
        //    m_SightTimer += Time.deltaTime;

        //    m_attackTimer = 0;

        //    if (m_SightTimer >= m_LostOfSightTime)
        //    {
        //        enemy.stateMachine.Transition(enemy.idleState);
        //        enemy.SetTransitionAnimation("Idle");
        //        Debug.Log("CHASE --> IDLE");
        //    }
        //}
        //else
        //{
        //    // Player is visible and close enough, reset timer
        //    m_SightTimer = 0f;
        //}
    }

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void OnParried()
    {
        base.OnParried();
    }

    public override void ResetValues()
    {
        base.ResetValues();
        m_attackTimer = 0;
    }

    // Method called in the animator to shoot the projectile.
    public override void PerformAttack()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed;
        projectile.GetComponent<EnemyHit>().enemy = enemy;
    }
}
