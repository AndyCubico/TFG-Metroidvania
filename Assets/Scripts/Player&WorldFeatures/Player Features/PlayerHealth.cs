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
            healthCoroutine = StartCoroutine(Heal(healthQuantity));
            healPotions--;
            healPotionText.text = healPotions.ToString();
        }
    }

    void ReceiveAnAttack(float damage)
    {
        playerHealth -= damage;

        if(healthCoroutine != null)
        {
            StopCoroutine(healthCoroutine);
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

            healthBar.fillAmount = playerHealth / maxPlayerHealth;
            healthText.text = playerHealth.ToString();

            CheckpointEvents.FastCheckpointEvent?.Invoke();
        }
    }

    public IEnumerator Heal(float quantity)
    {
        isHealing = true;

        yield return new WaitForSeconds(timeToHeal); // Wait to the preparation time

        playerHealth += quantity;

        if(playerHealth > 100)
        {
            playerHealth = 100;
        }

        healthBar.fillAmount = playerHealth / maxPlayerHealth;
        healthText.text = playerHealth.ToString();

        isHealing = false;
    }
}
