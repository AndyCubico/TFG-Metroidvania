using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using PlayerController;

public class Player_Combat : MonoBehaviour
{
    enum ATTACK_TYPE
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

    [Header("________________________ TIMERS ________________________")]
    [Space(10)]
    //Basic Attack
    public float basicAttackCooldown;
    float basicAttackCooldownLocal;

    public float comboResetTime;

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
    public Attack_Detectors leftDetector;
    public Attack_Detectors rightDetector;
    public Attack_Detectors impactHitDetector;

    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference BasicAttackAction;

    bool basicAttackDown;

    [Header("Assigned Layers")]
    [Space(5)]
    //Layers
    public LayerMask enemyMask;

    //Character Controller
    CharacterPlayerController characterController;

    //Enemy list
    List<EnemyHealth> enemyHealth = new List<EnemyHealth>();

    //Checkers
    bool leftAttack;
    bool rightAttack;
    bool downAttack;

    //Combo
    int comboCounter;
    float comboTimer;
    bool isOnCombo;
    bool canHitCombo;

    //Rigidbody
    Rigidbody2D rb;

    private void OnEnable()
    {
        BasicAttackAction.action.started += BasicAttackEvent;
    }

    private void OnDisable()
    {
        BasicAttackAction.action.started -= BasicAttackEvent;
    }

    public void BasicAttackEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            basicAttackDown = true;
        }
        else
        {
            basicAttackDown = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterPlayerController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        basicAttackCooldownLocal = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //_Basic Attack + Combo
        if (basicAttackDown)
        {
            if(basicAttackCooldownLocal == 0)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                ComboAttack();
                canHitCombo = false;
            }

            basicAttackCooldownLocal += Time.deltaTime;
        }

        if(basicAttackCooldownLocal > 0) // Here the cooldown of attack is starting to sum up
        {
            basicAttackCooldownLocal += Time.deltaTime;

            if(basicAttackCooldownLocal > basicAttackCooldown) // If the cooldown is reached the combo timer will start to happen and the basic attack will be ready again
            {
                canHitCombo = true;
                basicAttackCooldownLocal = 0;
            }
        }


        if (isOnCombo) //Here the combo counter counts
        {
            if(canHitCombo)
            {
                comboTimer += Time.deltaTime;
            }

            if (comboTimer > comboResetTime) //Auto Reset if passes certain time
            {
                ResetCombo();
            }
        }
        //Basic Attack + Combo_

        if(characterController.playerState == CharacterPlayerController.PLAYER_STATUS.DASH)
        {
            Physics2D.IgnoreLayerCollision(6, 11, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(6, 11, false);
        }
    }

    void BasicAttack(ATTACK_TYPE attackType)
    {
        switch (attackType)
        {
            case ATTACK_TYPE.SOFT_ATTACK:
                    animator.SetTrigger("Attack_Sides"); //Say the animator to do the side attack
                break;
            case ATTACK_TYPE.MID_ATTACK:
                animator.SetTrigger("Attack_Sides"); //Say the animator to do the side attack
                break;
            case ATTACK_TYPE.HARD_ATTACK:
                animator.SetTrigger("Attack_Sides"); //Say the animator to do the side attack
                break;
        }

        if (characterController.flipAnimation) //Se what direction is facing the player
        {
            //Check if is there is something at LeftAttack
            leftAttack = Physics2D.OverlapAreaAll(LeftHit.bounds.min, LeftHit.bounds.max, enemyMask).Length > 0;

            if (leftAttack)
            {
                enemyHealth = leftDetector.SendEnemyCollision();

                HitEnemy(attackType, enemyHealth);
            }
        }
        else
        {
            //Check if is there is something at RightAttack
            rightAttack = Physics2D.OverlapAreaAll(RightHit.bounds.min, RightHit.bounds.max, enemyMask).Length > 0;

            if (rightAttack)
            {
                enemyHealth = rightDetector.SendEnemyCollision();

                HitEnemy(attackType, enemyHealth);
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
            BasicAttack((ATTACK_TYPE)comboCounter); //Exectue the attack
            comboTimer = 0f; //Resets the combo
        }
        else
        {
            ResetCombo();
        }
    }

    void ResetCombo() //This function restarts the timer
    {
        comboCounter = 0;
        comboTimer = 0;
        isOnCombo = false;
        canHitCombo = false;
    }

    public void ImpactHit()
    {
        //Check if is there is something at LeftAttack
        downAttack = Physics2D.OverlapAreaAll(DownHit.bounds.min, DownHit.bounds.max, enemyMask).Length > 0;

        if (downAttack)
        {
            enemyHealth = impactHitDetector.SendEnemyCollision();

            HitEnemy(ATTACK_TYPE.MID_ATTACK, enemyHealth);
        }
    }

    void HitEnemy(ATTACK_TYPE attackType, List<EnemyHealth> enemyHealth)
    {
        float damage = 0;

        switch (attackType)
        {
            case ATTACK_TYPE.SOFT_ATTACK:
                damage = softDamage;
                break;
            case ATTACK_TYPE.MID_ATTACK:
                damage = midDamage;
                break;
            case ATTACK_TYPE.HARD_ATTACK:
                damage = hardDamage;
                break;
            default:
                break;
        }

        Debug.Log("Enemy Hit with: " + damage);

        for (int i = 0; i < enemyHealth.Count; i++)
        {
            enemyHealth[i].ReceiveDamage(damage);
        }
    }

    public void AnimationHasFinished()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
