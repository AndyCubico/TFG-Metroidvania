using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
