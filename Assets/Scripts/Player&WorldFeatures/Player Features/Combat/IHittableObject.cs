using UnityEngine;

public interface IHittableObject
{
    public virtual void ReceiveDamage(float damage, AttackFlagType flag) { }
    
    //Ejemplo de como funcionan las flags
    /*
     * if (Input.GetKey(KeyCode.Space))
    {
        flag |= AttackFlagType.Saltando; // A�ade el estado Saltando
    }

    if ((flag & AttackFlagType.Saltando) != 0)
    {
        Debug.Log("El jugador est� saltando.");
    }

    if (Input.GetKeyUp(KeyCode.Space))
    {
        flag &= ~AttackFlagType.Saltando; // Quita el estado Saltando
    }
    */
}
