using PlayerController;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class WaterBehaviour : MonoBehaviour
{
    bool m_IsFrozen = false;
    bool playerIsInsde = false;
    CharacterPlayerController playerController;

    // Sun and Rain mechanics
    public float waterChange = 0.0f;
    Vector3 originalScale;
    [SerializeField] GameObject m_SwimingCollider;

    private void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<CharacterPlayerController>();

        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        ChangeClimate.ChangeWeather += UpdateWater;
    }

    private void OnDisable()
    {
        ChangeClimate.ChangeWeather -= UpdateWater;
    }

    public void UpdateWater(CLIMATES c) 
    {
        switch (c)
        {
            case CLIMATES.NEUTRAL:
                break;
            case CLIMATES.SUN:

                UnFreezeWater();
                ChangeWaterLevel(1/waterChange);

                break;
            case CLIMATES.SNOW:
                break;
            case CLIMATES.NONE:
                break;
        }
    }

    public void FreezeWater()
    {
        m_IsFrozen = true;
        playerController.isInWater = false;

        if (playerIsInsde && playerController.playerState != CharacterPlayerController.PLAYER_STATUS.WALL && playerController.playerState != CharacterPlayerController.PLAYER_STATUS.HANGED)
        {
            playerController.transform.position = new Vector3(playerController.transform.position.x, playerController.transform.position.y + (this.transform.localScale.y / 2), playerController.transform.position.z);

            playerIsInsde = false;
        }

        this.GetComponent<BoxCollider2D>().isTrigger = false;
        gameObject.layer = 7;
    }

    public void UnFreezeWater()
    {
        m_IsFrozen = false;
        this.GetComponent<BoxCollider2D>().isTrigger = true;
        gameObject.layer = 0;
    }

    public void ChangeWaterLevel(float value) 
    {
        Vector3 nScale = transform.localScale;
        nScale.y = value;
        gameObject.transform.localScale = nScale;
        gameObject.transform.position -= new Vector3 (0, (originalScale.y-nScale.y), 0);
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
}
