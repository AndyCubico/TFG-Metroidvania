using UnityEngine;

public class IdleSOBase : ScriptableObject
{
    protected Enemy enemy;
    protected Transform transform;
    protected GameObject gameObject;

    protected Transform playerTransform;

    public virtual void Initialize(GameObject gameObject, Enemy enemy)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.enemy = enemy;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public virtual void DoEnter() { }
    public virtual void DoExit() { ResetValues(); }
    public virtual void DoUpdate()
    {
        if (enemy.isAggro)
        {
            enemy.stateMachine.Transition(enemy.chaseState);
        }
    }
    public virtual void DoFixedUpdate() { }
    public virtual void DoAnimationTrigger(Enemy.ANIMATION_TRIGGER triggerType) { }
    public virtual void ResetValues() { }

    public virtual bool IsPlayerVisible()
    {
        if (playerTransform == null)
            return false;

        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Check if player is within view distance
        if (distanceToPlayer > enemy.viewDistance)
            return false;

        float angleToPlayer = Vector2.Angle(
            enemy.pathfollowing.isFacingRight ? Vector2.right : Vector2.left,
            directionToPlayer
        );

        // Check if player is within view angle
        if (angleToPlayer < enemy.viewAngle / 2f)
        {
            // Raycast to check for obstacles between AI and player
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, enemy.obstacleLayer | enemy.playerLayer);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }
}
