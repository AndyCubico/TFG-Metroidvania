using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum CLIMATES
    {
        NEUTRAL,
        SUN,
        SNOW,
        NONE
    }

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; } // Make this script a singelton

    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference sunAction;
    public InputActionReference snowAction;
    [Space(10)]

    [SerializeField] bool m_IsExterior;

    //Bool keys
    //bool snowKey;

    //Controllers
    public CLIMATES climate = CLIMATES.NEUTRAL;

    //Objects
    List<WaterBehaviour> waters;

    SpecialAbilities specialAbilitiesScript;

    public static Action<CLIMATES> ChangeWeather;
    public static Action NotifyExteriorChange;

    private GameObject m_WeatherWheel;
    private bool m_IsWheelActive;
    private Vector3 m_MousePos;
    private Vector3 m_MiddleScreen;
    private Image m_actualSprite;

    private void Awake()
    {
        // This makes sure that there is only one instance of the singlenton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        waters = new List<WaterBehaviour>();

        GameObject waterContainer = GameObject.Find("Waters");

        if( waterContainer != null)
        {
            for (int i = 0; i < waterContainer.transform.childCount; i++)
            {
                waters.Add(waterContainer.transform.GetChild(i).GetComponent<WaterBehaviour>());
            }
        }

        specialAbilitiesScript = GameObject.Find("SpecialAttacks").GetComponent<SpecialAbilities>();
        m_WeatherWheel = GameObject.Find("WeatherWheel").gameObject;
        m_WeatherWheel.SetActive(false);
    }

    void Update()
    {
        //// Sun Inputs
        if (sunAction.action.WasPressedThisFrame())
        {
            if (climate == CLIMATES.SUN)
            {
                climate = CLIMATES.NEUTRAL;
            }
            else
            {
                climate = CLIMATES.SUN;
            }

            ChangeClimateTo(climate);

            //snowKey = true;
        }
        else
        {
            //snowKey = false;
        }

        //Snow Inputs
        if (snowAction.action.WasPressedThisFrame())
        {
            if (climate == CLIMATES.SNOW)
            {
                climate = CLIMATES.NEUTRAL;
            }
            else
            {
                climate = CLIMATES.SNOW;
            }

            ChangeClimateTo(climate);

            //snowKey = true;
        }
        else
        {
            //snowKey = false;
        }

        WeatherWheel();
    }

    private void WeatherWheel()
    {
        Vector3 mouseToMidScreen = new Vector3();
        m_MiddleScreen = new Vector2(Screen.width / 2f, Screen.height / 2f);

        if (m_IsWheelActive) // Show the actual sprite selected in the climate wheel
        {
            m_MousePos = Input.mousePosition;

            mouseToMidScreen = m_MousePos - m_MiddleScreen;

            if(m_actualSprite != null)
            {
                if (Vector3.Distance(m_MousePos, m_MiddleScreen) >= 160f)
                {
                    if (mouseToMidScreen.x > 0 && mouseToMidScreen.y > 0)
                    {
                        if (m_actualSprite.gameObject.name != "SnowWeather")
                        {
                            m_actualSprite.color = new Color(m_actualSprite.color.r, m_actualSprite.color.g, m_actualSprite.color.b, 1f);
                            m_actualSprite = GameObject.Find("SnowWeather").GetComponent<Image>();
                            m_actualSprite.color = new Color(m_actualSprite.color.r, m_actualSprite.color.g, m_actualSprite.color.b, m_actualSprite.color.a / 2f);
                        }
                    }
                    else if (mouseToMidScreen.x < 0 && mouseToMidScreen.y > 0)
                    {
                        if (m_actualSprite.gameObject.name != "SunWeather")
                        {
                            m_actualSprite.color = new Color(m_actualSprite.color.r, m_actualSprite.color.g, m_actualSprite.color.b, 1f);
                            m_actualSprite = GameObject.Find("SunWeather").GetComponent<Image>();
                            m_actualSprite.color = new Color(m_actualSprite.color.r, m_actualSprite.color.g, m_actualSprite.color.b, m_actualSprite.color.a / 2f);
                        }
                    }
                }
                else
                {
                    if (m_actualSprite.gameObject.name != "NeutralWeather")
                    {
                        m_actualSprite.color = new Color(m_actualSprite.color.r, m_actualSprite.color.g, m_actualSprite.color.b, 1f);
                        m_actualSprite = GameObject.Find("NeutralWeather").GetComponent<Image>();
                        m_actualSprite.color = new Color(m_actualSprite.color.r, m_actualSprite.color.g, m_actualSprite.color.b, m_actualSprite.color.a / 2f);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(2)) // Press the wheel button, show the wheel, and set the current sprite selected, also start slowMotion
        {
            m_IsWheelActive = true;
            m_WeatherWheel.SetActive(true);

            m_actualSprite = GameObject.Find("NeutralWeather").GetComponent<Image>();
            m_actualSprite.color = new Color(m_actualSprite.color.r, m_actualSprite.color.g, m_actualSprite.color.b, m_actualSprite.color.a / 2f);

            Mouse.current.WarpCursorPosition(m_MiddleScreen);

            SlowMotionEffect.eSlowMotionOn?.Invoke(0.02f, 0.5f);
        }
        else if (Input.GetMouseButtonUp(2)) // Release the wheel button, hide the wheel, no SlowMotion and select climate
        {
            SlowMotionEffect.eSlowMotionOff?.Invoke();

            if (m_IsWheelActive)
            {
                if (Vector3.Distance(m_MousePos, m_MiddleScreen) >= 160f)
                {
                    if (mouseToMidScreen.x > 0 && mouseToMidScreen.y > 0)
                    {
                        climate = CLIMATES.SNOW;
                    }
                    else if (mouseToMidScreen.x < 0 && mouseToMidScreen.y > 0)
                    {
                        climate = CLIMATES.SUN;
                    }
                }
                else
                {
                    climate = CLIMATES.NEUTRAL;
                }
            }

            m_actualSprite.color = new Color(m_actualSprite.color.r, m_actualSprite.color.g, m_actualSprite.color.b, 1f);
            m_IsWheelActive = false;
            m_WeatherWheel.SetActive(false);
            ChangeClimateTo(climate);
        }
    }

    public void ChangeClimateTo(CLIMATES newClima)
    {
        ChangeWeather.Invoke(newClima);
    }

    public bool GetExteriorState() 
    {
        return m_IsExterior;
    }

    public void  SetExteriorState(bool isExterior) 
    {
        m_IsExterior = isExterior;
    }
}
