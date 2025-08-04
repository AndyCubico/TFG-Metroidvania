using UnityEngine;

public class WeatherManagerUI : MonoBehaviour
{
    [Header("Opacity")]
    public CanvasGroup canvasGroup;
    [SerializeField] float m_ExteriorOpacity;
    [SerializeField] float m_InteriorOpacity;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeClimate.Instance.GetExteriorState();
        ChangeOpacity((ChangeClimate.Instance.GetExteriorState()) ? m_ExteriorOpacity : m_InteriorOpacity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChangeOpacity(float a) 
    { 
        canvasGroup.alpha = a;
    }
}
