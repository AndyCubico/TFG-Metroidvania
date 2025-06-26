using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Detectors : MonoBehaviour
{
    List<IHittableObject> enemyObj;

    private void Start()
    {
        enemyObj = new List<IHittableObject>();
    }

    public List<IHittableObject> SendEnemyCollision()
    {
        if(enemyObj != null)
        {
            return enemyObj;
        }

        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            enemyObj.Add(collision.GetComponent<IHittableObject>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            enemyObj.Remove(collision.GetComponent<IHittableObject>());
        }
    }
}
