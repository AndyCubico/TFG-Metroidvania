using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Chase Melee", menuName = "Enemy Logic/Chase/Chase Melee")]
public class ChaseMelee : ChaseSOBase
{
    //[SerializeField] float m_ChaseSpeed;

    [SerializeField] private float m_PathCooldown = 0.5f;
    [SerializeField] private float m_LostOfSightTime = 5f;
    [SerializeField] private float m_DistanceLimitToPlayer = 10f;

    private float m_SightTimer = 0f;
    private float m_PathTimer = 0f;
    private Vector3 m_LastTargetPos;
    private bool m_playerInSight = false;

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);

        m_SightTimer = 0f;
        m_PathTimer = 0f;
    }

    public override void DoEnter()
    {
        base.DoEnter();

        m_SightTimer = 0f;
        m_PathTimer = 0f;

        int2 currentPos = new int2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        int2 targetPos = new int2(Mathf.FloorToInt(playerTransform.position.x), Mathf.FloorToInt(playerTransform.position.y));

        enemy.pathfollowing.SetPath(currentPos, targetPos);
    }

    public override void DoExit()
    {
        base.DoExit();

        enemy.pathfollowing.FinishPath();
    }

    public override void DoUpdate()
    {
        base.DoUpdate();

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        m_playerInSight = false;

        if (distanceToPlayer <= m_DistanceLimitToPlayer)
        {
            // Only raycast if within the sight range
            LayerMask visionMask = LayerMask.GetMask("Player", "Ground");
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, visionMask);

            Debug.DrawRay(transform.position, directionToPlayer * distanceToPlayer, Color.green);

            if (hit.collider != null && hit.collider.transform.root.CompareTag("Player"))
            {
                m_playerInSight = true;
            }
        }
        else
        {
            // Debug ray if beyond range
            Debug.DrawRay(transform.position, directionToPlayer * distanceToPlayer, Color.red);
        }

        // If player is not in sight OR too far, start the countdown
        if (!m_playerInSight || distanceToPlayer > m_DistanceLimitToPlayer)
        {
            m_SightTimer += Time.deltaTime;

            if (m_SightTimer >= m_LostOfSightTime)
            {
                enemy.stateMachine.Transition(enemy.idleState);
                enemy.SetTransitionAnimation("Idle");
                Debug.Log("CHASE --> IDLE");
            }
        }
        else
        {
            // Player is visible and close enough, reset timer
            m_SightTimer = 0f;
        }
    }


    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();

        m_PathTimer += Time.fixedDeltaTime;

        if (m_PathTimer >= m_PathCooldown && !enemy.pathfollowing.isJumping)
        {
            m_PathTimer = 0f;

            int2 currentPos = new int2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
            int2 targetPos = new int2(Mathf.FloorToInt(playerTransform.position.x), Mathf.FloorToInt(playerTransform.position.y));

            enemy.pathfollowing.SetPath(currentPos, targetPos);

            if (!enemy.pathfollowing.isPathValid)
            {
                int2 fallbackTarget = enemy.pathfollowing.FindNearestWalkableTile(targetPos);
                enemy.pathfollowing.SetPath(currentPos, fallbackTarget);
            }
        }
    }

    public override void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType)
    {
        base.DoAnimationTrigger(triggerType);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}
