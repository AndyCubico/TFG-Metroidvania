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
    Transform originalPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        referenceHealthScript = GameObject.Find("Player").GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        
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
