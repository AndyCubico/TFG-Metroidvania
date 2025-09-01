using UnityEngine;

public class PowerUps : MonoBehaviour
{
    public enum TypeUpgrade
    {
        HEALTH_UPGRADE,
        NUMBER_POTIONS,
        HEALING_POTIONS,
    }

    public TypeUpgrade typeUpgrade;

    PlayerHealth referenceHealthScript;
    public float numValue;
    Vector3 m_originalPosition;
    bool m_increaseValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_originalPosition = transform.position;
        referenceHealthScript = GameObject.Find("Player").GetComponent<PlayerHealth>();
        
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
                default:
                    break;
            }
            // Destroy this
            Destroy(this.gameObject);
        }
    }

    private void OnValidate()
    {
        switch (typeUpgrade)
        {
            case TypeUpgrade.HEALTH_UPGRADE:
                this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0.504717F, 0.504717F,1.0F);
                break;
            case TypeUpgrade.NUMBER_POTIONS:
                this.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.953f, 0.922f, 0.545f, 1.0f);
                break;
            case TypeUpgrade.HEALING_POTIONS:
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                break;
            default:
                break;
        }
        
    }
}
