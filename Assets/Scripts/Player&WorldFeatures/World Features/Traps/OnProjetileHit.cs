using System.Linq;
using UnityEngine;

public class OnProjetileHit : MonoBehaviour
{
    public enum EffectOnCollision
    {
        BREAK, // On collision with the ground the object breaks
        STAY, // On collision with the ground the object stays, creating a platform
    }
    public EffectOnCollision effectOnCollision;

    public LayerMask layer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision with something");
        if ((layer.value & (1 << collision.gameObject.layer)) > 0)
        {
            Debug.Log("Collision layer was:" + collision.gameObject.layer);
            ManageCollision();
        }
    }

    void ManageCollision()
    {
        switch (effectOnCollision)
        {
            case EffectOnCollision.BREAK:
                this.gameObject.transform.parent.parent.gameObject.SetActive(false);
                break;
            case EffectOnCollision.STAY:
                break;
        }
    }
}
