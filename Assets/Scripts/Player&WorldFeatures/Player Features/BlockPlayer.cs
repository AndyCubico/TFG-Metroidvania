using NUnit.Framework;
using UnityEngine;

public class BlockPlayer : MonoBehaviour
{
    public Component[] movementScripts;

    public Component[] combatScripts;

    public Component[] healthDamageScripts;

    Rigidbody2D rbPlayer;

    private void Start()
    {
        rbPlayer = GetComponent<Rigidbody2D>();
    }

    public void BlockAll()
    {
        DisableComponents(movementScripts);
        DisableComponents(combatScripts);
        DisableComponents(healthDamageScripts);
    }

    public void BlockMovement()
    {
        DisableComponents(movementScripts);
    }

    public void BlockCombat()
    {
        DisableComponents(combatScripts);
    }
    public void BlockDamageHealth()
    {
        DisableComponents(healthDamageScripts);
    }

    public void EnableAll()
    {
        EnableComponents(movementScripts);
        EnableComponents(combatScripts);
        EnableComponents(healthDamageScripts);
    }

    public void EnableMovement()
    {
        EnableComponents(movementScripts);
    }

    public void EnableCombat()
    {
        EnableComponents(combatScripts);
    }
    public void EnableDamageHealth()
    {
        EnableComponents(healthDamageScripts);
    }

    void DisableComponents(Component[] components)
    {
        foreach (var comp in components)
        {
            if (comp is Behaviour behaviour)
            {
                behaviour.enabled = false;
            }
        }
    }

    void EnableComponents(Component[] components)
    {
        rbPlayer.constraints = RigidbodyConstraints2D.FreezeRotation;

        foreach (var comp in components)
        {
            if (comp is Behaviour behaviour)
            {
                behaviour.enabled = true;
            }
        }
    }
}
