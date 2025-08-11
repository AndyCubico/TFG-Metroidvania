using UnityEngine;

public class ProjectileAutoDestroy : MonoBehaviour
{
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.activeSelf)
        //{
        //    // Check recursively up the parent chain for "Enemy" tag
        //    Transform current = collision.transform;
        //    bool isEnemy = false;
        //    while (current != null)
        //    {
        //        if (current.CompareTag("Enemy"))
        //        {
        //            isEnemy = true;
        //            break;
        //        }
        //        current = current.parent;
        //    }

        //    if (!isEnemy)
        //    {
        //        Destroy(gameObject);
        //    }
        //}

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("NormalFloor"))
        {
            Destroy(gameObject);
        }
    }
}