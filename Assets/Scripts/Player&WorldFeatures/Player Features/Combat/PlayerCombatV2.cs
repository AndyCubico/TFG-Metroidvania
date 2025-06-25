using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using PlayerController;

public class PlayerCombatV2 : MonoBehaviour
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
    public BoxCollider2D attackCollider;
    public BoxCollider2D downHit;
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
    public Attack_Detectors attackDetector;
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
    List<HittableObject> nextEnemyHealth = new List<HittableObject>();
    List<HittableObject> enemyHealth = new List<HittableObject>();

    //Checkers
    bool basicAttack;
    bool downAttack;

    //Combo
    int comboCounter;
    float comboTimer;
    bool isOnCombo;
    bool canHitCombo;

    //Rigidbody
    public Rigidbody2D rb;

    //Checkers
    //bool cooldown;
    //bool comboReset;
    bool isDamaging;
    [HideInInspector]public bool isAttacking;

    //Values
    float gravityScale;

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
        //cooldown = false;
        canHitCombo = true;
        isDamaging = false;
        isAttacking = false;

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
            if (canHitCombo)
            {
                if (ComboAttack())
                {
                    canHitCombo = false;
                    isAttacking = true;
                    rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                    AnimationSelector((ATTACK_TYPE)comboCounter);
                }

                //cooldown = true;
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
            if (canHitCombo)
            {
                comboTimer += Time.deltaTime;
            }

            if (comboTimer > comboResetTime) //Auto Reset if passes certain time
            {
                ResetCombo();
            }
        }
        //Basic Attack + Combo_

        if (characterController.isDashing || characterController.isImpactHitting)
        {
            Physics2D.IgnoreLayerCollision(6, 11, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(6, 11, false);
        }

        if (isDamaging)
        {
            BasicAttack((ATTACK_TYPE)comboCounter); //Exectue the attack
        }
    }

    void BasicAttack(ATTACK_TYPE attackType)
    {
        //Check if is there is something at Basic Attack
        basicAttack = Physics2D.OverlapAreaAll(attackCollider.bounds.min, attackCollider.bounds.max, enemyMask).Length > 0;

        if (basicAttack)
        {
            if (!characterController.isGrounded && !characterController.moveStopper)
            {
                StopCoroutine(AirAttack());
                StartCoroutine(AirAttack());
            }

            nextEnemyHealth = new List<HittableObject>(attackDetector.SendEnemyCollision());
            List<HittableObject> newEnemiesList = new List<HittableObject>();

            for (int j = 0; j < nextEnemyHealth.Count; j++)
            {
                if (!enemyHealth.Contains(nextEnemyHealth[j]) && !newEnemiesList.Contains(nextEnemyHealth[j]))
                {
                    newEnemiesList.Add(nextEnemyHealth[j]);
                }
            }

            if(nextEnemyHealth.Count > 0)
            {
                for(int i = 0; i < nextEnemyHealth.Count; i++)
                {
                    if (!newEnemiesList.Contains(nextEnemyHealth[i]) && enemyHealth.Count == 0)
                    {
                        newEnemiesList.Add(nextEnemyHealth[i]);
                    }
                }
            }

            enemyHealth = new List<HittableObject>(nextEnemyHealth);

            if (newEnemiesList.Count > 0)
            {
                HitEnemy(attackType, newEnemiesList);
            }
        }
    }

    bool ComboAttack()
    {
        comboCounter++;
        isOnCombo = true;

        if (comboCounter > 0 && comboCounter <= 3) //Combo counter
        {
            //Debug.Log("Attack " + comboCounter);
            comboTimer = 0f; //Resets the combo
            return true;
        }
        else
        {
            ResetCombo();
            return false;
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
        downAttack = Physics2D.OverlapAreaAll(downHit.bounds.min, downHit.bounds.max, enemyMask).Length > 0;

        if (downAttack)
        {
            enemyHealth = impactHitDetector.SendEnemyCollision();

            HitEnemy(ATTACK_TYPE.MID_ATTACK, enemyHealth);

            //Pasar a Andreu los enemigos que han sido golpeados.
        }
    }

    void HitEnemy(ATTACK_TYPE attackType, List<HittableObject> enemyHealth)
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

    public void StartAttacking()
    {
        if(!isDamaging)
        {
            characterController.blockFlip = true;

            isDamaging = true;
        }
    }

    public void AnimationHasFinished()
    {
        canHitCombo = true;
        isDamaging = false;
        isAttacking = false;
        characterController.blockFlip = false;
        nextEnemyHealth.Clear();
        enemyHealth.Clear();

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

    void AnimationSelector(ATTACK_TYPE attackType)
    {
        switch (attackType)
        {
            case ATTACK_TYPE.SOFT_ATTACK:
                if (characterController.flipAnimation) //Se what direction is facing the player
                {
                    animator.SetTrigger("Attack_Sides_Left"); //Say the animator to do the side attack
                }
                else
                {
                    animator.SetTrigger("Attack_Sides_Right"); //Say the animator to do the side attack
                }
                break;
            case ATTACK_TYPE.MID_ATTACK:
                if (characterController.flipAnimation) //Se what direction is facing the player
                {
                    animator.SetTrigger("Attack_Sides_Left"); //Say the animator to do the side attack
                }
                else
                {
                    animator.SetTrigger("Attack_Sides_Right"); //Say the animator to do the side attack
                }
                break;
            case ATTACK_TYPE.HARD_ATTACK:
                if (characterController.flipAnimation) //Se what direction is facing the player
                {
                    animator.SetTrigger("Attack_Sides_Left"); //Say the animator to do the side attack
                }
                else
                {
                    animator.SetTrigger("Attack_Sides_Right"); //Say the animator to do the side attack
                }
                break;
        }
    }
}

