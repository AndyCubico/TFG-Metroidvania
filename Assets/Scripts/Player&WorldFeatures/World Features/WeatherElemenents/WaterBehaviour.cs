using PlayerController;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class WaterBehaviour : MonoBehaviour, IHittableObject
{
    bool m_IsFrozen = false;
    float m_frozenTime = 4.0f;
    bool playerIsInsde = false;
    public Color[] waterColors = new Color[2];
    CharacterPlayerController playerController;

    // Waterfall mechanics (default water is horizontal not vertical)
    [SerializeField] bool m_IsWaterfall;
    //[ShowIf("m_IsWaterfall",true)]

    // Sun and Rain mechanics
    public float waterChange = 1.0f;
    Vector3 originalScale;
    [SerializeField] GameObject m_SwimingCollider;

    public AttackFlagType freezeMask;

    private void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<CharacterPlayerController>();

        originalScale = transform.localScale;

        if (WeatherManager.Instance.GetExteriorState()) // If is an interior area weather doesn't affect.
        {
            UpdateWater(WeatherManager.Instance.climate);
        }
    }

    private void OnEnable()
    {
        WeatherManager.ChangeWeather += UpdateWater;
    }

    private void OnDisable()
    {
        WeatherManager.ChangeWeather -= UpdateWater;
    }

    public void UpdateWater(CLIMATES c) 
    {
        if (WeatherManager.Instance.GetExteriorState()) // If is an interior area weather doesn't affect.
        {
            switch (c)
            {
                case CLIMATES.NEUTRAL:

                    UnFreezeWater();
                    ResetWaterLevel();

                    break;
                case CLIMATES.SUN:

                    UnFreezeWater();
                    if(originalScale == transform.localScale && !m_IsWaterfall) 
                    {
                        ChangeWaterLevel(1 / waterChange);
                    }
                    

                    break;
                case CLIMATES.SNOW:

                    FreezeWater();

                    break;
                case CLIMATES.NONE:
                    break;
            }
        }
    }

    public void FreezeWater()
    {
        this.gameObject.GetComponent<SpriteRenderer>().color = waterColors[1];
        m_IsFrozen = true;
        playerController.isInWater = false;

        if (playerIsInsde && playerController.playerState != CharacterPlayerController.PLAYER_STATUS.WALL && playerController.playerState != CharacterPlayerController.PLAYER_STATUS.HANGED)
        {
            playerController.transform.position = new Vector3(playerController.transform.position.x, playerController.transform.position.y + (this.transform.localScale.y / 2), playerController.transform.position.z);

            playerIsInsde = false;
        }

        this.GetComponent<BoxCollider2D>().isTrigger = false;

        if (m_IsWaterfall) 
        {
            gameObject.layer = 9; // Wall layer
        }
        else 
        {
            gameObject.layer = 7; // Ground
        }
            
    }

    public void UnFreezeWater()
    {
        this.gameObject.GetComponent<SpriteRenderer>().color = waterColors[0];
        m_IsFrozen = false;
        this.GetComponent<BoxCollider2D>().isTrigger = true;
        gameObject.layer = 11; // Enemy layer
    }

    public void ChangeWaterLevel(float value) 
    {
        Vector3 nScale = transform.localScale;
        nScale.y = nScale.y *  value;
        gameObject.transform.localScale = nScale;
        Debug.Log(originalScale.y - nScale.y);
        gameObject.transform.position += new Vector3 (0, (nScale.y-originalScale.y), 0);
    }

    public void ResetWaterLevel() 
    {
        Vector3 nScale = transform.localScale;
        gameObject.transform.localScale = originalScale;
        gameObject.transform.position -= new Vector3(0, (nScale.y - originalScale.y), 0);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !m_IsFrozen && !playerController.isInWater && playerController.playerState != CharacterPlayerController.PLAYER_STATUS.WALL && playerController.playerState != CharacterPlayerController.PLAYER_STATUS.HANGED)
        {
            playerController.isInWater = true;
            playerIsInsde = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !m_IsFrozen)
        {
            playerController.isInWater = false;
            playerIsInsde = false;
        }
    }

    public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        if ((freezeMask & flag) != 0) 
        {
            if (!m_IsFrozen) 
            {
                StartCoroutine(FreezeWaterTemporaly());
            }
        }
    }

    IEnumerator FreezeWaterTemporaly() 
    {
        FreezeWater();
        yield return new WaitForSeconds(m_frozenTime); // Unfreze water after 4 seconds passed, coldown of ice spell / ice special attack is 4.5 seconds,.

        if (WeatherManager.Instance.GetExteriorState()) // If is an interior area weather doesn't affect.
        {
            UpdateWater(WeatherManager.Instance.climate);
        }
    }
}
