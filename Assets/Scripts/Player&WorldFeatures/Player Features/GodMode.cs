using PlayerController;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
public class GlobalGodMode
{
    public static bool m_IsGodMode;

    public static bool IsGodModeOn()
    {
        return m_IsGodMode;
    }
}

public class GodMode : MonoBehaviour
{
    private CharacterPlayerController m_CharacterController;
    private BoxCollider2D m_BoxCollider;
    private CircleCollider2D m_CircleCollider;
    private Rigidbody2D m_Rb;
    private PlayerHealth m_PlayerHealth;

    private float gravityScale;

    Vector2 move;

    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference movement;
    [Space(5)]

    [Header("God Speed Movement")]
    [Space(5)]
    public float speed;
    public float maxSpeedX;
    public float maxSpeedY;
    public float speedReduction;


    void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_CharacterController = GetComponent<CharacterPlayerController>();
        m_BoxCollider = GetComponent<BoxCollider2D>();
        m_CircleCollider = GetComponent<CircleCollider2D>();
        m_PlayerHealth = GetComponent<PlayerHealth>();

        gravityScale = m_Rb.gravityScale;
        GlobalGodMode.m_IsGodMode = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) || (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.G) || Input.GetKey(KeyCode.Period) && Input.GetKeyDown(KeyCode.G)))
        {
            GlobalGodMode.m_IsGodMode = !GlobalGodMode.m_IsGodMode;

            ChangeGodMode();
        }

        move.x = movement.action.ReadValue<Vector2>().x;
        move.y = movement.action.ReadValue<Vector2>().y;

        if (GlobalGodMode.m_IsGodMode)
        {
            if (Mathf.Abs(m_Rb.linearVelocity.x) < maxSpeedX)
            {
                m_Rb.linearVelocity += new Vector2(move.x * speed * Time.deltaTime, 0);
            }

            if (Mathf.Abs(m_Rb.linearVelocity.y) < maxSpeedY)
            {
                m_Rb.linearVelocity += new Vector2(0, move.y * speed * Time.deltaTime);
            }

            if (move.x == 0) //Movement Reduction (fictional reduction)
            {
                if (m_Rb.linearVelocity.x > 1f)
                {
                    m_Rb.linearVelocity -= new Vector2(speed / speedReduction * Time.deltaTime, 0);
                }
                else if (m_Rb.linearVelocity.x < -1f)
                {
                    m_Rb.linearVelocity += new Vector2(speed / speedReduction * Time.deltaTime, 0);
                }
                else
                {
                    m_Rb.linearVelocity = new Vector2(0, m_Rb.linearVelocity.y);
                }
            }

            if(move.y == 0)
            {
                if (m_Rb.linearVelocity.y > 1f)
                {
                    m_Rb.linearVelocity -= new Vector2(0, speed / speedReduction * Time.deltaTime);
                }
                else if (m_Rb.linearVelocity.y < -1f)
                {
                    m_Rb.linearVelocity += new Vector2(0, speed / speedReduction * Time.deltaTime);
                }
                else
                {
                    m_Rb.linearVelocity = new Vector2(m_Rb.linearVelocity.x, 0);
                }
            }
        }
    }

    private void ChangeGodMode()
    {
        if(GlobalGodMode.m_IsGodMode)
        {
            m_CharacterController.enabled = false;
            m_Rb.gravityScale = 0f;
            m_BoxCollider.enabled = false;
            m_CircleCollider.enabled = false;

            m_PlayerHealth.RestoreHealth();

            m_PlayerHealth.enabled = false;
        }
        else
        {
            m_CharacterController.enabled = true;
            m_Rb.gravityScale = gravityScale;
            m_BoxCollider.enabled = true;
            m_CircleCollider.enabled = true;
            m_PlayerHealth.enabled = true;
        }
    }
}
