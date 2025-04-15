using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerCombat : MonoBehaviour
{
    enum AttackType
    {
        NONE,
        SOFT_ATTACK,
        MID_ATTACK,
        HARD_ATTACK
    }

    [Header("Collider Detectors")]
    [Space(5)]

    //Detection Colliders
    public BoxCollider2D LeftHit;
    public BoxCollider2D RightHit;
    public BoxCollider2D DownHit;
    [Space(10)]

    [Header("________________________ DAMAGES ________________________")]
    [Space(10)]

    [Header("Combo Attacks")]
    //Animator
    public int softDamage;
    public int midDamage;
    public int hardDamage;
    [Space(10)]

    [Header("________________________ ANIMATOR ________________________")]
    //Animator
    Animator animator;

    [Header("________________________ ATTACK DETECTORS ________________________")]
    //Animator
    public AttackDetectors leftDetector;
    public AttackDetectors rightDetector;
    public AttackDetectors impactHitDetector;

    [Header("Asigned Layers")]
    [Space(5)]

    //Layers
    public LayerMask enemyMask;
    [Space(10)]

    //Character Controller
    CharacterPlayerController characterController;

    //Checkers
    bool leftAttack;
    bool rightAttack;
    bool downAttack;

    //Combo
    int comboCounter;
    float comboTimer;
    bool isOnCombo;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterPlayerController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ComboAttack();
        }

        if (isOnCombo) //Here the combo counter counts
        {
            comboTimer += Time.deltaTime;

            if (comboTimer > 0.5f) //Auto Reset if passes certain time
            {
                ResetCombo();
            }
        }
    }

    void BasicAttack(int attackType)
    {
        EnemyHealth enemyHealth = new EnemyHealth();

        if (attackType == 1)
        {
            animator.SetTrigger("Attactk_Sides"); //Say the animator to do the side attack
        }

        if (characterController.flipAnimation) //Se what direction is facing the player
        {
            //Check if is there is something at LeftAttack
            leftAttack = Physics2D.OverlapAreaAll(LeftHit.bounds.min, LeftHit.bounds.max, enemyMask).Length > 0;

            if (leftAttack)
            {
                enemyHealth = leftDetector.SendEnemyCollision();

                HitEnemy((AttackType)attackType, enemyHealth);
            }
        }
        else
        {
            //Check if is there is something at RightAttack
            rightAttack = Physics2D.OverlapAreaAll(RightHit.bounds.min, RightHit.bounds.max, enemyMask).Length > 0;

            if (rightAttack)
            {
                enemyHealth = rightDetector.SendEnemyCollision();

                HitEnemy((AttackType)attackType, enemyHealth);
            }
        }
    }

    void ComboAttack()
    {
        comboCounter++;
        isOnCombo = true;

        if (comboCounter > 0 && comboCounter <= 3) //Basic Attack
        {
            //Debug.Log("Attack " + comboCounter);
            BasicAttack(comboCounter); //Exectue the attack
            comboTimer = 0f; //Resets the combo
        }
    }

    void ResetCombo() //This function restarts the timer
    {
        comboCounter = 0;
        comboTimer = 0;
        isOnCombo = false;
    }

    public void ImpactHit()
    {
        EnemyHealth enemyHealth = new EnemyHealth();

        //Check if is there is something at LeftAttack
        downAttack = Physics2D.OverlapAreaAll(DownHit.bounds.min, DownHit.bounds.max, enemyMask).Length > 0;

        if (downAttack)
        {
            enemyHealth = impactHitDetector.SendEnemyCollision();

            HitEnemy(AttackType.MID_ATTACK, enemyHealth);
        }
    }

    void HitEnemy(AttackType attackType, EnemyHealth enemyHealth)
    {
        float damage = 0;

        switch (attackType)
        {
            case AttackType.SOFT_ATTACK:
                damage = softDamage;
                break;
            case AttackType.MID_ATTACK:
                damage = midDamage;
                break;
            case AttackType.HARD_ATTACK:
                damage = hardDamage;
                break;
            default:
                break;
        }

        Debug.Log("Enemy Hitted with: " + damage);
        enemyHealth.RecieveDamage(damage);
    }
}
