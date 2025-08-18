using UnityEngine;

public class SnowBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject m_CheckIfDestroyed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable()
    {
        WeatherManager.ChangeWeather += UpdateSnow;
    }

    private void OnDisable()
    {
        WeatherManager.ChangeWeather -= UpdateSnow;
    }

    private void Update()
    {
        if(m_CheckIfDestroyed != null && (!WeatherManager.Instance.GetExteriorState() || WeatherManager.Instance.climate != CLIMATES.SUN)) // If exterior and sunny, stop checking
        {
            gameObject.GetComponent<Renderer>().enabled = m_CheckIfDestroyed.activeInHierarchy;  

        }
    }

    public void UpdateSnow(CLIMATES c)
    {
        if (WeatherManager.Instance.GetExteriorState()) // If is an interior area weather doesn't affect.
        {
            switch (c)
            {
                case CLIMATES.NEUTRAL:

                    gameObject.GetComponent<Renderer>().enabled = true;

                    break;
                case CLIMATES.SUN:

                    gameObject.GetComponent<Renderer>().enabled = false;

                    break;
                case CLIMATES.SNOW:

                    gameObject.GetComponent<Renderer>().enabled = true;

                    break;
                case CLIMATES.NONE:
                    break;
            }
        }
    }
}
