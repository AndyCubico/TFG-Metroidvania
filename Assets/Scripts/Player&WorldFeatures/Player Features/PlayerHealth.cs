using System;
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

    [Header("UI Elements")]
    public Image healthBar;
    public TextMeshProUGUI healthText; 
    public TextMeshProUGUI maxHealthText;

    public TextMeshProUGUI healPotionText;
    [Space(10)]

    [Header("Heal Potions")]
    public int healPotions;
    public float healthQuantity;

    public float timeToHeal;
    [Space(10)]

    [Header("Input Actions")]
    public InputActionReference HealPotionAction;
    bool healPotionInput;

    bool isHealing;

    private Coroutine healthCoroutine;

    BlockPlayer blockPlayer;

    Rigidbody2D playerRb;

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
        isHealing = false;

        maxPlayerHealth = playerHealth;
        healthBar.fillAmount = playerHealth / maxPlayerHealth;
        healthText.text = playerHealth.ToString();
        maxHealthText.text = playerHealth.ToString();

        healPotionText.text = healPotions.ToString();

        blockPlayer = GetComponent<BlockPlayer>();
        playerRb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (HealPotionAction.action.WasPressedThisFrame())
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

        if (healPotions > 0 && healPotionInput && playerHealth < 100 && !isHealing)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.constraints = RigidbodyConstraints2D.FreezePositionX;
            healthCoroutine = StartCoroutine(Heal(healthQuantity));
        }
    }

    void ReceiveAnAttack(float damage)
    {
        playerHealth -= damage;

        if(healthCoroutine != null)
        {
            StopCoroutine(healthCoroutine);

            playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;

            blockPlayer.EnableMovement();
            blockPlayer.EnableCombat();

            isHealing = false;
        }

        if(playerHealth > 0)
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
        blockPlayer.BlockMovement();
        blockPlayer.BlockCombat();
        isHealing = true;

        yield return new WaitForSeconds(timeToHeal); // Wait to the preparation time

        playerHealth += quantity;

        if(playerHealth > 100)
        {
            playerHealth = 100;
        }

        healthBar.fillAmount = playerHealth / maxPlayerHealth;
        healthText.text = playerHealth.ToString();

        blockPlayer.EnableMovement();
        blockPlayer.EnableCombat();

        healPotions--;
        healPotionText.text = healPotions.ToString();

        isHealing = false;
    }
}
