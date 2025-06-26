using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using PlayerController;

public class PlayerCombat : MonoBehaviour
{
    enum ATTACK_TYPE
    {
        NONE,
        SOFT_ATTACK,
        MID_ATTACK,
        HARD_ATTACK,
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
    //float basicAttackCooldownLocal;

    public float comboResetTime;

    [Header("________________________ DAMAGES ________________________")]
    [Space(10)]

    [Header("Combo Attacks")]
    //Animator
    public int softDamage;
    public int midDamage;
    public int hardDamage;
    [Space(10)]

    [Header("________________________ VARIABLES ________________________")]
    public float airTime;

    [Header("________________________ ANIMATOR ________________________")]
    //Animator
    public Animator animator;

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
    public CharacterPlayerController characterController;

    //Enemy list
    List<IHittableObject> enemyHealth = new List<IHittableObject>();

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
    public Rigidbody2D rb;

    //Checkers
    bool cooldown;
    bool comboReset;

    //Values
    float gravityScale;

    AttackFlagType attackFlagType = AttackFlagType.None;

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        //basicAttackCooldownLocal = 0;
        cooldown = false;
        canHitCombo = true;

        gravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (BasicAttackAction.action.WasPressedThisFrame())
        {
            basicAttackDown = true;
        }
        else
        {
            basicAttackDown = false;
        }

        //_Basic Attack + Combo
        if (basicAttackDown)
        {
            if(canHitCombo)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                canHitCombo = false;
                ComboAttack();
                cooldown = true;
            }
        }

        //if(cooldown) // Here the cooldown of attack is starting to sum up
        //{
        //    basicAttackCooldownLocal += Time.deltaTime;

        //    if(basicAttackCooldownLocal > basicAttackCooldown) // If the cooldown is reached the combo timer will start to happen and the basic attack will be ready again
        //    {
        //        canHitCombo = true;
        //        basicAttackCooldownLocal = 0;
        //        cooldown = false;
        //    }
        //}


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

        if(characterController.isDashing || characterController.isImpactHitting)
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
                if (!characterController.isGrounded)
                {
                    StopCoroutine(AirAttack());
                    StartCoroutine(AirAttack());
                }

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
                if (!characterController.isGrounded)
                {
                    StopCoroutine(AirAttack());
                    StartCoroutine(AirAttack());
                }

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
        canHitCombo = true;
    }

    public void ImpactHit()
    {
        //Check if is there is something at LeftAttack
        downAttack = Physics2D.OverlapAreaAll(DownHit.bounds.min, DownHit.bounds.max, enemyMask).Length > 0;

        if (downAttack)
        {
            enemyHealth = impactHitDetector.SendEnemyCollision();

            HitEnemy(ATTACK_TYPE.MID_ATTACK, enemyHealth);

            //Pasar a Andreu los enemigos que han sido golpeados.
        }
    }

    void HitEnemy(ATTACK_TYPE attackType, List<IHittableObject> enemyHealth)
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
            attackFlagType = AttackFlagType.BasicAttack;
            enemyHealth[i].ReceiveDamage(damage, attackFlagType);
        }
    }

    public void AnimationHasFinished()
    {
        canHitCombo = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public IEnumerator AirAttack()
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        characterController.activateFallMultiplier = false;
        characterController.moveStopper = true;

        yield return new WaitUntil(() => canHitCombo);

        yield return new WaitForSeconds(airTime);

        rb.gravityScale = gravityScale;
        characterController.activateFallMultiplier = true;
        characterController.moveStopper = false;
    }
}
