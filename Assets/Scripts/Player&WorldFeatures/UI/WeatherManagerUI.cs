using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WeatherManagerUI : MonoBehaviour
{
    [Header("Opacity")]
    public CanvasGroup canvasGroup;
    [SerializeField] float m_ExteriorOpacity;
    [SerializeField] float m_InteriorOpacity; 

    [Header("Weather")]
    [SerializeField] List<Sprite> m_WeatherIcons;
    [SerializeField] UnityEngine.UI.Image m_CurrentWeatherIcon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InteriorExteriorChangeLogic();
        ChangeWeatherIcon(WeatherManager.Instance.climate);
    }

    private void OnEnable()
    {
        WeatherManager.NotifyExteriorChange += InteriorExteriorChangeLogic;
        WeatherManager.ChangeWeather += ChangeWeatherIcon;
    }

    private void OnDisable()
    {
        WeatherManager.NotifyExteriorChange -= InteriorExteriorChangeLogic;
        WeatherManager.ChangeWeather -= ChangeWeatherIcon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InteriorExteriorChangeLogic() 
    {
        WeatherManager.Instance.GetExteriorState();
        ChangeOpacity((WeatherManager.Instance.GetExteriorState()) ? m_ExteriorOpacity : m_InteriorOpacity);
    }

    void ChangeWeatherIcon(CLIMATES W) 
    {
        m_CurrentWeatherIcon.sprite = m_WeatherIcons[(int)W];
    }

    void ChangeOpacity(float a) 
    { 
        canvasGroup.alpha = a;
    }
}
