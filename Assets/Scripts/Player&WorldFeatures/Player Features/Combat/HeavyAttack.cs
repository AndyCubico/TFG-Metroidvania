using PlayerController;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HeavyAttack : MonoBehaviour
{
    enum ATTACK_TYPE
    {
        NONE,
        SOFT_ATTACK,
        MID_ATTACK,
        HEAVY_ATTACK,
        MAX
    }

    [Header("________________________ DAMAGES ________________________")]
    [Space(10)]

    [Header("Combo Attacks")]
    //Animator
    public int attackDamage;
    private int m_midDamage;
    private int m_hardDamage;
    [Space(10)]

    public Animator animator;
    [Space(5)]

    public CharacterPlayerController characterPlayerController;
    public PlayerCombatV2 playerCombat;
    [Space(5)]

    [Header("Input Actions")]
    public InputActionReference HeavyAttackAction;
    bool heavyAttackInput;
    [Space(5)]

    [Header("Rigidbody")]
    [Space(5)]
    public Rigidbody2D rb;
    [Space(5)]

    //Enemy layer mask
    public LayerMask enemyMask;

    bool canPerformAttack;
    bool hasHit;
    [HideInInspector]public bool isDamaging;

    BoxCollider2D attackCollider; 
    Attack_Detectors attackDetector;

    //Enemy list
    List<IHittableObject> nextEnemyHealth = new List<IHittableObject>();
    List<IHittableObject> enemyHealth = new List<IHittableObject>();

    //Charges list
    List<Image> chargesUI = new List<Image>();

    int heavyCharges;

    AttackFlagType attackFlagType = AttackFlagType.None;

    void Start()
    {
        attackCollider = GetComponent<BoxCollider2D>();
        attackDetector = attackCollider.GetComponent<Attack_Detectors>();

        heavyCharges = 0;
        heavyAttackInput = false;
        canPerformAttack = true;
        playerCombat.isAttacking = false;

        for(int i = 0; i < 3; i++)
        {
            chargesUI.Add(GameObject.Find("Charges").transform.GetChild(i).GetComponent<Image>());
        }

        UpdateCharges();
    }

    void Update()
    {
        if (HeavyAttackAction.action.WasPressedThisFrame())
        {
            heavyAttackInput = true;
        }
        else
        {
            heavyAttackInput = false;
        }

        if (heavyAttackInput && !playerCombat.isAttacking && !characterPlayerController.isInWater)
        {
            if (canPerformAttack && heavyCharges > 0)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                rb.linearVelocity = Vector2.zero;
                canPerformAttack = false;
                playerCombat.isAttacking = true;

                if (characterPlayerController.flipAnimation) //Se what direction is facing the player
                {
                    animator.SetTrigger("Heavy_Attack_Left"); //Say the animator to do the heavy attack right
                }
                else
                {
                    animator.SetTrigger("Heavy_Attack_Right"); //Say the animator to do the heavy attack left
                }
            }
        }

        if (characterPlayerController != null)
        {
            if(characterPlayerController.playerFaceDir == CharacterPlayerController.PLAYER_FACE_DIRECTION.RIGHT)
            {
                attackCollider.transform.localPosition = new Vector3(Mathf.Abs(attackCollider.transform.localPosition.x), attackCollider.transform.localPosition.y, attackCollider.transform.localPosition.z);
            }
            else if(characterPlayerController.playerFaceDir == CharacterPlayerController.PLAYER_FACE_DIRECTION.LEFT)
            {
                attackCollider.transform.localPosition = new Vector3(Mathf.Abs(attackCollider.transform.localPosition.x) * -1, attackCollider.transform.localPosition.y, attackCollider.transform.localPosition.z);
            }
        }

        if(isDamaging)
        {
            Hit();
        }
    }

    public void AnimationHasFinished()
    {
        canPerformAttack = true;
        playerCombat.isAttacking = false;
        isDamaging = false;
        characterPlayerController.blockFlip = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        animator.SetBool("Heavy_Attack_Right", false);
        animator.SetBool("Heavy_Attack_Left", false);
    }

    public void Hit()
    {
        //ATTACK_TYPE attackType = (ATTACK_TYPE)heavyCharges;
        ATTACK_TYPE attackType = ATTACK_TYPE.SOFT_ATTACK;
        heavyCharges = 0;

        UpdateCharges();

        hasHit = Physics2D.OverlapAreaAll(attackCollider.bounds.min, attackCollider.bounds.max, enemyMask).Length > 0;

        if (hasHit)
        {
            if (!characterPlayerController.isGrounded && !characterPlayerController.moveStopper)
            {
                StopCoroutine(playerCombat.AirAttack());
                StartCoroutine(playerCombat.AirAttack());
            }

            (List<IHittableObject> enemyList, List<GameObject> enemyObjects) = attackDetector.SendEnemyCollision();
            nextEnemyHealth = new List<IHittableObject>(enemyList);

            List<IHittableObject> newEnemiesList = new List<IHittableObject>();

            for (int j = 0; j < nextEnemyHealth.Count; j++)
            {
                if (!enemyHealth.Contains(nextEnemyHealth[j]) && !newEnemiesList.Contains(nextEnemyHealth[j]))
                {
                    newEnemiesList.Add(nextEnemyHealth[j]);
                }
            }

            if (nextEnemyHealth.Count > 0)
            {
                for (int i = 0; i < nextEnemyHealth.Count; i++)
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
                HitEnemy(attackType, newEnemiesList);
            }
        }
    }

    void HitEnemy(ATTACK_TYPE attackType, List<IHittableObject> enemyHealth)
    {
        float damage = 0;

        switch (attackType)
        {
            case ATTACK_TYPE.SOFT_ATTACK:
                damage = attackDamage;
                break;
            case ATTACK_TYPE.MID_ATTACK:
                damage = m_midDamage;
                break;
            case ATTACK_TYPE.HEAVY_ATTACK:
                damage = m_hardDamage;
                break;
            default:
                break;
        }

        Debug.Log("Enemy Hit with: " + damage);

        for (int i = 0; i < enemyHealth.Count; i++)
        {
            attackFlagType = AttackFlagType.HeavyAttack;
            enemyHealth[i].ReceiveDamage(damage, attackFlagType);
        }
    }

    void UpdateCharges()
    {
        for(int i = 0; i < chargesUI.Count; i++)
        {
            if (i <= heavyCharges - 1)
            {
                chargesUI[i].enabled = true;
            }
            else
            {
                chargesUI[i].enabled = false;
            }
        }
    }

    public void AddCharges(int charges)
    {
        //heavyCharges += charges;
        heavyCharges = 1;

        UpdateCharges();
    }

    public void StartAttacking()
    {
        if (!isDamaging)
        {
            characterPlayerController.blockFlip = true;

            isDamaging = true;
        }
    }
}
