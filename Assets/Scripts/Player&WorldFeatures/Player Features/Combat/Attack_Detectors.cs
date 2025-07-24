using System.Collections.Generic;
using UnityEngine;

public class Attack_Detectors : MonoBehaviour
{
    List<IHittableObject> enemyHit;
    List<GameObject> enemyObj;

    private void Start()
    {
        enemyObj = new List<GameObject>();
        enemyHit = new List<IHittableObject>();
    }

    public (List<IHittableObject>, List<GameObject>) SendEnemyCollision()
    {
        if (enemyObj != null && enemyHit != null)
        {
            return (enemyHit, enemyObj);
        }

        return (null, null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            enemyHit.Add(collision.GetComponent<IHittableObject>());
            enemyObj.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            enemyHit.Remove(collision.GetComponent<IHittableObject>());
            enemyObj.Remove(collision.gameObject);
        }
    }
}
