using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class HitLever : MonoBehaviour, IHittableObject, ILerpValueReturn
{
    public AttackFlagType flagMask;

    bool m_IsTriggered = false;
    [SerializeField] private float m_UntriggeredValue = 0.0f;
    [SerializeField] private float m_TriggeredValue = 1.0f;
    [SerializeField] private float m_CurrentValue; 
    [SerializeField] float m_timeFromFullToZero;

    [Header("Extra Properties")]
    [SerializeField] bool m_CanBeTurnedOff;
    [SerializeField] bool m_ChangesLerpProvider;
    [SerializeField] bool m_IsSave;
    [ShowIf("m_ChangesLerpProvider", true)] public MonoBehaviour lerpReciver; ILerpValueReciver m_LerpReciver;

    public class HitLever_SL : object_SL 
    {
        public bool isTriggered;
        public float currentValue;
    } 
    HitLever_SL m_Save;
    void Awake()
    {
        if (m_ChangesLerpProvider)
        {
            m_LerpReciver = lerpReciver as ILerpValueReciver;
            if (m_LerpReciver == null) { Debug.LogError("Assigned object does not implement ILerpValueReturn"); }
        }
    }

    private void Start()
    {
        if (m_IsSave) 
        {
            World_Save_Load saveLoad = GameObject.Find("GameManager")?.GetComponent<World_Save_Load>();

            object_SL nameObj = new object_SL
            {
                // Generic objects attributes
                objectName = this.gameObject.name,
                objectID = this.gameObject.transform.GetSiblingIndex(),
                objectType = object_SL.ObjectType.HIT_LEVER,
            };

            m_Save = (HitLever_SL)saveLoad.LoadObject(nameObj);

            if(m_Save != null) 
            {
                m_IsTriggered = m_Save.isTriggered;
                m_CurrentValue = m_Save.currentValue;

                if (m_IsTriggered) // If lever was triggered, trigger again with its saved value.
                {
                    if (m_ChangesLerpProvider)
                    {
                        m_CurrentValue = m_LerpReciver.ChangeLerpSource(this);
                    }

                    StartCoroutine(MoveLerp());
                }
                
            }
        }
    }

    public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        if ((flagMask & flag) != 0)
        {
            if (m_CanBeTurnedOff || !m_IsTriggered)
            {
                m_IsTriggered = !m_IsTriggered;

                if (m_ChangesLerpProvider)
                {
                    m_CurrentValue = m_LerpReciver.ChangeLerpSource(this);
                }

                StartCoroutine(MoveLerp());

                if (m_IsSave)
                {
                    World_Save_Load saveLoad = GameObject.Find("GameManager")?.GetComponent<World_Save_Load>();

                    m_Save = new HitLever_SL
                    {
                        // Generic objects attributes
                        objectName = this.gameObject.name,
                        objectID = this.gameObject.transform.GetSiblingIndex(),
                        objectType = object_SL.ObjectType.HIT_LEVER,

                        // Specific object atributes
                        isTriggered = m_IsTriggered,
                        currentValue = m_CurrentValue,

                    };
                    saveLoad.SaveObject(m_Save);
                }
            }
        }
    }

    IEnumerator MoveLerp()
    {
        if (m_IsTriggered)
        {
            float sign = Mathf.Sign(m_TriggeredValue - m_CurrentValue);
            while (sign == Mathf.Sign(m_TriggeredValue - m_CurrentValue))
            {
                m_CurrentValue += sign * Time.deltaTime / m_timeFromFullToZero;
                yield return null;
            }
            if (sign != Mathf.Sign(m_TriggeredValue - m_CurrentValue))
            {
                m_CurrentValue = m_TriggeredValue;
            }
        }
        else
        {
            float sign = Mathf.Sign(m_UntriggeredValue - m_CurrentValue);
            while (sign == Mathf.Sign(m_UntriggeredValue - m_CurrentValue))
            {
                m_CurrentValue += sign * Time.deltaTime / m_timeFromFullToZero;
                yield return null;
            }
            if (sign != Mathf.Sign(m_UntriggeredValue - m_CurrentValue))
            {
                m_CurrentValue = m_UntriggeredValue;
            }
        }

        
    }

    public float GetCurrentValue() 
    {
        return m_CurrentValue;
    }
}
