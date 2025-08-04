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



public class ChangeClimate : MonoBehaviour
{
    public static ChangeClimate Instance { get; private set; } // Make this code a singelton

    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference snowAction;
    [Space(10)]

    [SerializeField] bool m_IsExterior;

    //Bool keys
    bool snowKey;

    //Controllers
    public CLIMATES climate = CLIMATES.NEUTRAL;

    //Objects
    List<WaterBehaviour> waters;

    SpecialAbilities specialAbilitiesScript;

    public static Action<CLIMATES> ChangeWeather;

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
        if(specialAbilitiesScript.snowAbilityUnlocked) // If is unlocked the snow special attack
        {
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

                snowKey = true;
            }
            else
            {
                snowKey = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha9) && climate != CLIMATES.SUN) // Testing
        {
            climate = CLIMATES.SUN;
            ChangeClimateTo(climate);
        }
    }

    public void ChangeClimateTo(CLIMATES newClima)
    {
        ChangeWeather.Invoke(newClima);

        switch (newClima)
        {
            case CLIMATES.NEUTRAL:
                ChangeClimateToNeutral();
                break;
            case CLIMATES.SUN:
                break;
            case CLIMATES.SNOW:
                ChangeClimateToSnow();
                break;
        }
    }

    void ChangeClimateToSnow()
    {
        for (int i = 0; i < waters.Count; i++)
        {
            waters[i].FreezeWater();
        }
    }

    void ChangeClimateToNeutral()
    {
        //Snow
        for (int i = 0; i < waters.Count; i++)
        {
            waters[i].UnFreezeWater();
        }
    }

    public bool GetExteriorState() 
    {
        return m_IsExterior;
    }

    public void  SetExteriorState(bool isExterior) 
    {
        m_IsExterior = true;
    }
}
