using Unity.Mathematics;
using UnityEngine;

public class PatrolSetup : MonoBehaviour
{
    [SerializeField] private Vector2Int patrolStart;
    [SerializeField] private Vector2Int patrolEnd;

    private void Awake()
    {
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.stateContext["PatrolStart"] = new int2(patrolStart.x, patrolStart.y);
            enemy.stateContext["PatrolEnd"] = new int2(patrolEnd.x, patrolEnd.y);
        }
    }
}
