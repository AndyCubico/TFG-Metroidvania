using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Experimental.GraphView.GraphView;

public class HitMechanism : MonoBehaviour, IHittableObject, ILerpValueReturn
{
    public AttackFlagType flagMask;

    [Header("Accumulated hit amount")]
    public float currentCharges;
    [SerializeField] float m_MaxCharges;

    [Header("Countdown")]
    [SerializeField] float m_timeFromFullToZero;

    public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        if ((flagMask & flag) != 0)
        {
            if(currentCharges < m_MaxCharges) 
            {
                currentCharges++;
                currentCharges = Mathf.Min(m_MaxCharges, currentCharges);
            }
        }
    }

    private void Update()
    {
        if(currentCharges > 0) 
        {
            currentCharges -= Time.deltaTime*m_MaxCharges/(m_timeFromFullToZero);
        }
    }

    public float GetCurrentValue() 
    {
        return currentCharges;
    }
}
