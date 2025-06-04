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
    private int midDamage;
    private int hardDamage;
    [Space(10)]

    public Animator animator;
    [Space(5)]

    public CharacterPlayerController characterPlayerController;
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

    BoxCollider2D attackCollider; 
    Attack_Detectors attackDetector;

    //Enemy list
    List<EnemyHealth> enemyHealth = new List<EnemyHealth>();

    //Charges list
    List<Image> chargesUI = new List<Image>();

    int heavyCharges;

    void Start()
    {
        attackCollider = GetComponent<BoxCollider2D>();
        attackDetector = attackCollider.GetComponent<Attack_Detectors>();

        heavyCharges = 0;
        heavyAttackInput = false;
        canPerformAttack = true;

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

        if (heavyAttackInput)
        {
            if (canPerformAttack && heavyCharges > 0)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                canPerformAttack = false;

                animator.SetBool("Heavy_Attack", true);
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
    }

    public void AnimationHasFinished()
    {
        canPerformAttack = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        animator.SetBool("Heavy_Attack", false);
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
            enemyHealth = attackDetector.SendEnemyCollision();

            HitEnemy(attackType, enemyHealth);
        }
    }

    void HitEnemy(ATTACK_TYPE attackType, List<EnemyHealth> enemyHealth)
    {
        float damage = 0;

        switch (attackType)
        {
            case ATTACK_TYPE.SOFT_ATTACK:
                damage = attackDamage;
                break;
            case ATTACK_TYPE.MID_ATTACK:
                damage = midDamage;
                break;
            case ATTACK_TYPE.HEAVY_ATTACK:
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
}
