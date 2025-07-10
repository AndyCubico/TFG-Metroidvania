using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using PlayerController;
using Spine.Unity;

[System.Flags]
public enum AttackFlagType
{
    None = 0,
    BasicAttack = 1 << 0,   // 1
    ImpactHit = 1 << 1,    // 2
    HeavyAttack = 1 << 2,  // 4
    SnowAttack = 1 << 3     // 8
}

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
    public float returnToAttackAfterFinishComboTime;
    public float inputBufferTime;
    private float m_inputBufferCounter;

    [Header("________________________ ANIMATOR ________________________")]
    //Animator
    public Animator animator;

    [Header("________________________ SPRITE RENDERER ________________________")]
    //Animator
    public SkeletonMecanim skeleton;
    public bool actualSprite;

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
    List<IHittableObject> nextEnemyHealth = new List<IHittableObject>();
    List<IHittableObject> enemyHealth = new List<IHittableObject>();

    //Checkers
    bool m_basicAttack;
    bool m_downAttack;

    //Combo
    [HideInInspector]public int comboCounter;
    float m_comboTimer;
    float m_finishComboTimer;
    bool m_isOnCombo;
    bool m_canHitCombo;

    //Rigidbody
    public Rigidbody2D rb;

    //Checkers
    //bool cooldown;
    //bool comboReset;
    bool isDamaging;
    [HideInInspector]public bool isAttacking;

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
        //cooldown = false;
        m_finishComboTimer = 0;
        actualSprite = skeleton.Skeleton.FlipX;

        m_canHitCombo = true;
        isDamaging = false;
        isAttacking = false;

        gravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (skeleton.Skeleton.FlipX != actualSprite && m_isOnCombo)
        {
            ResetCombo();
        }

        if(m_finishComboTimer > 0)
        {
            m_finishComboTimer -= Time.deltaTime;
        }
        else
        {
            m_finishComboTimer = 0;
        }

        if (BasicAttackAction.action.WasPressedThisFrame())
        {
            basicAttackDown = true;
            m_inputBufferCounter = inputBufferTime;
        }
        //else
        //{
        //    basicAttackDown = false;
        //}

        if(m_inputBufferCounter <= 0)
        {
            basicAttackDown = false;
        }
        else
        {
            m_inputBufferCounter -= Time.deltaTime;
        }

        //_Basic Attack + Combo
        if (basicAttackDown && !isAttacking && !characterController.isInWater)
        {
            if (m_canHitCombo && m_finishComboTimer == 0)
            {
                if (ComboAttack())
                {
                    characterController.blockFlip = true;

                    m_canHitCombo = false;
                    isAttacking = true;
                    actualSprite = skeleton.Skeleton.FlipX;
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

        if (m_isOnCombo) //Here the combo counter counts
        {
            if (m_canHitCombo)
            {
                m_comboTimer += Time.deltaTime;
            }

            if (m_comboTimer > comboResetTime) //Auto Reset if passes certain time
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
        m_basicAttack = Physics2D.OverlapAreaAll(attackCollider.bounds.min, attackCollider.bounds.max, enemyMask).Length > 0;

        if (m_basicAttack)
        {
            if (!characterController.isGrounded && !characterController.moveStopper)
            {
                StopCoroutine(AirAttack());
                StartCoroutine(AirAttack());
            }

            nextEnemyHealth = new List<IHittableObject>(attackDetector.SendEnemyCollision());
            List<IHittableObject> newEnemiesList = new List<IHittableObject>();

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

            enemyHealth = new List<IHittableObject>(nextEnemyHealth);

            if (newEnemiesList.Count > 0)
            {
                attackFlagType = AttackFlagType.BasicAttack;
                HitEnemy(attackType, newEnemiesList, attackFlagType);
            }
        }
    }

    bool ComboAttack()
    {
        comboCounter++;
        m_isOnCombo = true;

        if (comboCounter > 0 && comboCounter <= 3) //Combo counter
        {
            //Debug.Log("Attack " + comboCounter);
            m_comboTimer = 0f; //Resets the combo
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
        m_comboTimer = 0;
        m_isOnCombo = false;
        m_canHitCombo = true;

        isDamaging = false;
        isAttacking = false;
        characterController.blockFlip = false;

        nextEnemyHealth.Clear();
        enemyHealth.Clear();

        animator.SetFloat("Combo", comboCounter);

        animator.SetBool("Attack_Sides1_Left", false);
        animator.SetBool("Attack_Sides2_Left", false);
        animator.SetBool("Attack_Sides3_Left", false);

        animator.SetBool("Attack_Sides1_Right", false);
        animator.SetBool("Attack_Sides2_Right", false);
        animator.SetBool("Attack_Sides3_Right", false);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void ImpactHit()
    {
        //Check if is there is something at LeftAttack
        m_downAttack = Physics2D.OverlapAreaAll(downHit.bounds.min, downHit.bounds.max, enemyMask).Length > 0;
        isAttacking = false;

        if (m_downAttack)
        {
            enemyHealth = impactHitDetector.SendEnemyCollision();

            attackFlagType = AttackFlagType.ImpactHit;
            HitEnemy(ATTACK_TYPE.MID_ATTACK, enemyHealth, attackFlagType);
        }
    }

    void HitEnemy(ATTACK_TYPE attackType, List<IHittableObject> enemyHealth, AttackFlagType flag)
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

        CameraEvents.eCameraShake?.Invoke(0.05f, 0.08f);
        SlowMotionEffect.eSlowMotion?.Invoke(0.15f, 0.02f);

        Debug.Log("Enemy Hit with: " + damage);

        for (int i = 0; i < enemyHealth.Count; i++)
        {
            enemyHealth[i].ReceiveDamage(damage, flag);
        }
    }

    public void StartAttacking()
    {
        if(!isDamaging)
        {
            isDamaging = true;
        }
    }

    public void AnimationHasFinished()
    {
        m_canHitCombo = true;
        isDamaging = false;
        isAttacking = false;
        //characterController.blockFlip = false;
        //Debug.Log("UnBlockFlip");
        nextEnemyHealth.Clear();
        enemyHealth.Clear();

        if(comboCounter == 3)
        {
            ResetCombo();
            m_finishComboTimer = returnToAttackAfterFinishComboTime;
        }

        //rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public IEnumerator AirAttack()
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        characterController.activateFallMultiplier = false;
        characterController.moveStopper = true;

        yield return new WaitUntil(() => m_canHitCombo);

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
                    animator.SetBool("Attack_Sides1_Left", true); //Say the animator to do the side attack
                }
                else
                {
                    animator.SetBool("Attack_Sides1_Right", true); //Say the animator to do the side attack
                }
                break;
            case ATTACK_TYPE.MID_ATTACK:
                if (characterController.flipAnimation) //Se what direction is facing the player
                {
                    animator.SetBool("Attack_Sides2_Left", true); //Say the animator to do the side attack
                }
                else
                {
                    animator.SetBool("Attack_Sides2_Right", true); //Say the animator to do the side attack
                }
                break;
            case ATTACK_TYPE.HARD_ATTACK:
                if (characterController.flipAnimation) //Se what direction is facing the player
                {
                    animator.SetBool("Attack_Sides3_Left", true); //Say the animator to do the side attack
                }
                else
                {
                    animator.SetBool("Attack_Sides3_Right", true); //Say the animator to do the side attack
                }
                break;
        }
    }
}

