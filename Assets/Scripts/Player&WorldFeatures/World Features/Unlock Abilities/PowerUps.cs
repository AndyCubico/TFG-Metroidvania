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
        referenceHealthScript = GameObject.Find("Player").GetComponent<PlayerHealth>();
        m_originalPosition = transform.position;
    }

    private void Update()
    {
        if (m_increaseValue) 
        {
            transform.position += new Vector3(0,0.02f,0);
            m_increaseValue = (transform.position.y > (m_originalPosition.y + 1.0f));
        }
        else 
        {
            transform.position -= new Vector3(0, 0.02f, 0);
            m_increaseValue = (transform.position.y < (m_originalPosition.y - 1.0f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            // Aply upgrade
            switch (typeUpgrade) 
            {
                case TypeUpgrade.HEALTH_UPGRADE:
                    referenceHealthScript.IncreaseMaxHealth(numValue);
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
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
                break;
            case TypeUpgrade.NUMBER_POTIONS:
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            case TypeUpgrade.HEALING_POTIONS:
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                break;
            default:
                break;
        }
        
    }
}
