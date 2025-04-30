using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Block_And_Parry : MonoBehaviour
{
    public float maxParryTime;
    float parryCounter;

    bool enemyIsAttacking;

    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference BlockAndParryAction;

    bool isBlocking;

    private void OnEnable()
    {
        BlockAndParryAction.action.started += BlockAndParryEvent;
    }

    private void OnDisable()
    {
        BlockAndParryAction.action.started -= BlockAndParryEvent;
    }

    public void BlockAndParryEvent(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isBlocking = true;
        }

        if (context.canceled)
        {
            isBlocking = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isBlocking)
        {
            parryCounter += Time.deltaTime;
        }
        else
        {
            parryCounter = 0;
        }

        if (enemyIsAttacking)
        {
            if (parryCounter > 0 && parryCounter <= maxParryTime)
            {
                ReciveAttackParryWindow();
            }
            else
            {
                ReciveAttackBlockWindow();
            }
        }
    }

    void ReciveAttackParryWindow()
    {
        //Send that the parry has been done correcltly
        Debug.Log("Parried");
    }

    void ReciveAttackBlockWindow()
    {
        //The attack has been blocked
        Debug.Log("Blocked");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            enemyIsAttacking = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack"))
        {
            enemyIsAttacking = false;
        }
    }
}
