using PlayerController;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health")]
    public float playerHealth;
    float maxPlayerHealth;
    [Space(10)]

    //[Header("UI Elements")]
    Image healthBar;
    TextMeshProUGUI healthText;
    TextMeshProUGUI maxHealthText;

    TextMeshProUGUI healPotionText;
    //[Space(10)]

    [Header("Heal Potions")]
    int healPotions;
    public int maxHealPotions;
    public float healthQuantity;

    public float timeToHeal;
    [Space(10)]

    [Header("Input Actions")]
    public InputActionReference HealPotionAction;
    bool healPotionInput;

    [Header("Particle System")]
    public ParticleSystem healingParticles;

    [HideInInspector] public bool isHealing;

    private Coroutine healthCoroutine;

    BlockPlayer blockPlayer;

    Rigidbody2D playerRb;

    //Scripts
    private SpecialAbilities m_SpecialAbilities;
    private CharacterPlayerController m_CharacterPlayerController;
    private PlayerCombatV2 m_PlayerCombat;
    private HeavyAttack m_HeavyAttack;

    private void OnEnable()
    {
        HealthEvents.TakingDamage += ReceiveAnAttack;
    }

    private void OnDisable()
    {
        HealthEvents.TakingDamage -= ReceiveAnAttack;
    }

    void Start()
    {
        healthBar = GameObject.Find("LifeBar").GetComponent<Image>();
        healthText = GameObject.Find("LifeText").GetComponent<TextMeshProUGUI>();
        maxHealthText = GameObject.Find("MaxHealthText").GetComponent<TextMeshProUGUI>();
        healPotionText = GameObject.Find("PotionsText").GetComponent<TextMeshProUGUI>();
        m_SpecialAbilities = GameObject.Find("SpecialAttacks").GetComponent<SpecialAbilities>();
        m_PlayerCombat = GameObject.Find("Combat").GetComponent<PlayerCombatV2>();
        m_HeavyAttack = GameObject.Find("HeavyAttack").GetComponent<HeavyAttack>();
        m_CharacterPlayerController = gameObject.GetComponent<CharacterPlayerController>();

        isHealing = false;

        maxPlayerHealth = playerHealth;
        healthBar.fillAmount = playerHealth / maxPlayerHealth;
        healthText.text = playerHealth.ToString();
        maxHealthText.text = playerHealth.ToString();

        healPotions = maxHealPotions;
        healPotionText.text = healPotions.ToString();

        blockPlayer = GetComponent<BlockPlayer>();
        playerRb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (HealPotionAction.action.WasPressedThisFrame() && !m_SpecialAbilities.specialHabilitiesTrigger && !m_PlayerCombat.isAttacking && m_CharacterPlayerController.playerState == CharacterPlayerController.PLAYER_STATUS.GROUND)
        {
            healPotionInput = true;
        }
        else
        {
            healPotionInput = false;
        }

        //if(Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    HealthEvents.TakingDamage?.Invoke(20);
        //}

        if (healPotions > 0 && healPotionInput && playerHealth < maxPlayerHealth && !isHealing)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.constraints = RigidbodyConstraints2D.FreezePositionX;
            healthCoroutine = StartCoroutine(Heal(healthQuantity));
        }
    }

    void ReceiveAnAttack(float damage)
    {
        playerHealth -= damage;

        if (healthCoroutine != null)
        {
            StopCoroutine(healthCoroutine);

            playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;

            blockPlayer.EnableMovement();
            blockPlayer.EnableCombat();

            isHealing = false;
        }

        SlowMotionEffect.eSlowMotion?.Invoke(1f, 0.05f);

        if (playerHealth > 0)
        {
            Debug.Log("Player Has been hit with: " + damage);

            healthBar.fillAmount = playerHealth / maxPlayerHealth;
            healthText.text = playerHealth.ToString();

            if (playerHealth <= 0)
            {
                Debug.Log("Player Has died");
            }
        }
        else
        {
            playerHealth = 100;
            healPotions = 3;

            healthBar.fillAmount = playerHealth / maxPlayerHealth;
            healthText.text = playerHealth.ToString();

            CheckpointEvents.FastCheckpointEvent?.Invoke();
        }
    }

    public IEnumerator Heal(float quantity)
    {
        healingParticles.Play();
        blockPlayer.BlockMovement();
        blockPlayer.BlockCombat();
        isHealing = true;

        yield return new WaitForSeconds(timeToHeal); // Wait to the preparation time

        playerHealth += quantity;

        if (playerHealth > 100)
        {
            playerHealth = 100;
        }

        healthBar.fillAmount = playerHealth / maxPlayerHealth;
        healthText.text = playerHealth.ToString();

        blockPlayer.EnableMovement();
        blockPlayer.EnableCombat();

        healPotions--;
        healPotionText.text = healPotions.ToString();

        healingParticles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        isHealing = false;
    }

    public void RestoreHealthPotions()
    {
        healPotions = maxHealPotions;
    }

    public void AddPotions(int potions)
    {
        healPotions += potions;
    }
}
