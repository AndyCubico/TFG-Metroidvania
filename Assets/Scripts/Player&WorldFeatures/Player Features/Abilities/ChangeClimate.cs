using NUnit.Framework;
using System.Collections;
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
    [Header("Input Actions")]
    [Space(5)]
    public InputActionReference snowAction;
    [Space(10)]

    //Bool keys
    bool snowKey;

    //Controllers
    public CLIMATES climate = CLIMATES.NEUTRAL;

    //Objects
    List<WaterBehaviour> waters;

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
    }
    
    void Update()
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

    public void ChangeClimateTo(CLIMATES newClima)
    {
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
}
