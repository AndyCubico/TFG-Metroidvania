using UnityEngine;

public interface IHittableObject
{
    public virtual void ReceiveDamage(float damage, AttackFlagType flag) { }
    
    //Ejemplo de como funcionan las flags
    /*
     * if (Input.GetKey(KeyCode.Space))
    {
        flag |= AttackFlagType.Saltando; // Añade el estado Saltando
    }

    if ((flag & AttackFlagType.Saltando) != 0)
    {
        Debug.Log("El jugador está saltando.");
    }

    if (Input.GetKeyUp(KeyCode.Space))
    {
        flag &= ~AttackFlagType.Saltando; // Quita el estado Saltando
    }
    */
}
