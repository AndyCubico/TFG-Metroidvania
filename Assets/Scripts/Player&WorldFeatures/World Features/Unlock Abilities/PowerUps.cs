using UnityEngine;
using static BreakableDoor;

public class PowerUps : MonoBehaviour
{
    public enum TypeUpgrade
    {
        HEALTH_UPGRADE,
        NUMBER_POTIONS,
        HEALING_POTIONS,
        HEAVY_ATTACK,
    }

    public TypeUpgrade typeUpgrade;

    PlayerHealth referenceHealthScript;
    HeavyAttack referenceHeavyAttackScript;
    public float numValue;
    Vector3 m_originalPosition;
    bool m_increaseValue;

    Breakable_SL m_Save;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        World_Save_Load saveLoad = GameObject.Find("GameManager")?.GetComponent<World_Save_Load>();

        object_SL nameObj = new object_SL
        {
            // Generic objects attributes
            objectName = this.gameObject.name,
            objectID = this.gameObject.transform.GetSiblingIndex(),
            objectType = object_SL.ObjectType.BREAKABLE_OBJECT,
        };

        m_Save = (Breakable_SL)saveLoad.LoadObject(nameObj);

        if (m_Save != null)
        {
            if (m_Save.isBroken)
            {
                Destroy(gameObject);
            }
        }

        m_originalPosition = transform.position;
        referenceHealthScript = GameObject.Find("Player").GetComponent<PlayerHealth>();
        referenceHeavyAttackScript = GameObject.Find("HeavyAttack").GetComponent<HeavyAttack>();


    }

    private void Update()
    {
        if (m_increaseValue) 
        {
            transform.position += new Vector3(0,0.003f,0);
        }
        else 
        {
            transform.position -= new Vector3(0, 0.003f, 0);
        }

        if (Mathf.Abs(m_originalPosition.y-transform.position.y) >= 0.35f) 
        {
            m_increaseValue = !m_increaseValue;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Aply upgrade
            switch (typeUpgrade)
            {
                case TypeUpgrade.HEALTH_UPGRADE:
                    float ret = referenceHealthScript.IncreaseMaxHealth(numValue);
                    break;
                case TypeUpgrade.NUMBER_POTIONS:
                    referenceHealthScript.IncreaseMaxPotions((int)numValue);
                    break;
                case TypeUpgrade.HEALING_POTIONS:
                    referenceHealthScript.IncreasePotionHealing(numValue);
                    break;
                case TypeUpgrade.HEAVY_ATTACK:
                    referenceHeavyAttackScript.UnlockedCharges += (int)numValue;
                    referenceHeavyAttackScript.AddCharges((int)numValue);
                    break;
                default:
                    break;
            }

            World_Save_Load saveLoad = GameObject.Find("GameManager")?.GetComponent<World_Save_Load>();

            m_Save = new Breakable_SL
            {
                // Generic objects attributes
                objectName = this.gameObject.name,
                objectID = this.gameObject.transform.GetSiblingIndex(),
                objectType = object_SL.ObjectType.BREAKABLE_OBJECT,

                // Specific object atributes
                isBroken = true

            };
            saveLoad.SaveObject(m_Save);

            // Destroy this
            Destroy(this.gameObject);
        }
    }

    private void OnValidate()
    {
        switch (typeUpgrade)
        {
            case TypeUpgrade.HEALTH_UPGRADE:
                this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0.504717F, 0.504717F, 1.0F);
                break;
            case TypeUpgrade.NUMBER_POTIONS:
                this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.953f, 0.922f, 0.545f, 1.0f);
                break;
            case TypeUpgrade.HEALING_POTIONS:
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                break;
            case TypeUpgrade.HEAVY_ATTACK:
                this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.7122642F, 0.8954492F, 1.0F, 1.0f);
                break;
            default:
                break;
        }

    }
}
