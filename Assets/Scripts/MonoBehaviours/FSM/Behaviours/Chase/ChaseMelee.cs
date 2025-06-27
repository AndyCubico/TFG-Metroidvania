using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Chase Melee", menuName = "Enemy Logic/Chase/Chase Melee")]
public class ChaseMelee : ChaseSOBase
{
    //[SerializeField] float m_ChaseSpeed;

    [SerializeField] private float m_PathCooldown = 0.5f;
    [SerializeField] private float m_PredictionTime = 0.5f;

    private float m_Timer = 0f;
    private Vector3 m_LastTargetPos;

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void DoEnter()
    {
        base.DoEnter();

        int2 currentPos = new int2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        int2 targetPos = new int2(Mathf.FloorToInt(playerTransform.position.x), Mathf.FloorToInt(playerTransform.position.y));

        enemy.pathfollowing.SetPath(currentPos, targetPos);
    }

    public override void DoExit()
    {
        base.DoExit();
    }

    public override void DoUpdate()
    {
        base.DoUpdate();
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();

        m_Timer += Time.fixedDeltaTime;

        if (m_Timer >= m_PathCooldown && !enemy.pathfollowing.isJumping)
        {
            m_Timer = 0f;

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
