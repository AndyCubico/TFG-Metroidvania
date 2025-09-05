using PlayerController;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WhiteIceBehaviour : MonoBehaviour, IHittableObject
{
    // On collision destroy parameters
    [SerializeField] private GameObject m_objectToDestroy; 
    bool m_isDestroyDueCollision = true;
    bool m_hasCollided;
    [TagDropdown] public string[] collisionTag = new string[] { };


    [SerializeField] private bool m_isRespawnOverTime;

    [Header("Timing")]
    [ShowIf("m_isRespawnOverTime", true)] public float timeToRespawn;
    public float timeToDestroy;

    // Weather system elements
    public Color[] iceColors = new Color[2];
    float m_frozenTime = 4;
    bool m_isFrozen;
    public AttackFlagType freezeMask = AttackFlagType.SNOW_ATTACK;

    private void Start()
    {
        iceColors = new Color[] { new Color(0.8235294f, 0.9411765f, 1, 0.9411765f), new Color(0.6367924f, 0.8887472f, 1, 0.7843137f) };
        if (WeatherManager.Instance.GetExteriorState()) // If is an interior area weather doesn't affect.
        {
            UpdateIce(WeatherManager.Instance.climate);
        }
    }

    private void OnEnable()
    {
        WeatherManager.ChangeWeather += UpdateIce;
    }

    private void OnDisable()
    {
        WeatherManager.ChangeWeather -= UpdateIce;
    }

    public void UpdateIce(CLIMATES c)
    {
        if (WeatherManager.Instance.GetExteriorState()) // If is an interior area weather doesn't affect.
        {
            switch (c)
            {
                case CLIMATES.NEUTRAL: // If object was destroy due to sun, 

                    if (m_isRespawnOverTime && m_isDestroyDueCollision)
                    {
                        m_objectToDestroy.SetActive(true);
                    }

                    break;
                case CLIMATES.SUN: // If object not frozen destroy it

                    if (!m_isFrozen) 
                    {
                        m_objectToDestroy.SetActive(false);
                    }

                    break;
                case CLIMATES.SNOW:

                    if (m_isRespawnOverTime && m_isDestroyDueCollision)
                    {
                        m_objectToDestroy.SetActive(true);
                    }

                    break;
                case CLIMATES.NONE:
                    break;
            }
        }
    }

    

    public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        Debug.Log("AttackCollided");
        if ((freezeMask & flag) != 0)
        {
            if (!m_isFrozen && m_objectToDestroy.activeInHierarchy)
            {
                StartCoroutine(TemporalyReinforceIce());
            }
        }
    }

    IEnumerator TemporalyReinforceIce()
    {
        m_isFrozen = true;

        for (int i = 0; i < m_objectToDestroy.transform.childCount; ++i) 
        {
            Transform t = m_objectToDestroy.transform.GetChild(i); 

            if (t.childCount > 0 && t.GetChild(0).name == "Pivot") // If gameObject is a platform
            {
                t.GetChild(0).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().color = iceColors[1]; //Change Visuals sprite renderer
            }  
        }

        yield return new WaitForSeconds(m_frozenTime); // Make frail again after 4 seconds passed, coldown of ice spell / ice special attack is 4.5 seconds,.

        m_isFrozen = false;

        for (int i = 0; i < m_objectToDestroy.transform.childCount; ++i)
        {
            Transform t = m_objectToDestroy.transform.GetChild(i);

            if (t.childCount > 0 && t.GetChild(0).name == "Pivot") // If gameObject is a platform
            {
                t.GetChild(0).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().color = iceColors[0]; //Change Visuals sprite renderer
            }
        }

        if (WeatherManager.Instance.GetExteriorState()) // If is an interior area weather doesn't affect.
        {
            UpdateIce(WeatherManager.Instance.climate);
        }
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collisionTag.Contains(collision.gameObject.tag) && collision.gameObject.layer == LayerMask.NameToLayer("Player") && !m_isFrozen && m_objectToDestroy.activeInHierarchy)
        {
            Debug.Log("Collision with: " + collision.gameObject.tag);
            // Collision detected with corresponding tag

            if (!WeatherManager.Instance.GetExteriorState() || (WeatherManager.Instance.climate != CLIMATES.SUN) && !m_hasCollided) 
            {
                m_hasCollided = true;
                StartCoroutine(DeactivateGameObject(m_objectToDestroy));
            }
            

        }
    }

    private IEnumerator ActivateGameObject(GameObject go)
    {
        // Trigger animation if any 
        gameObject.GetComponent<Collider2D>().enabled = false;
        gameObject.transform.position += new Vector3(0, 0.25f, 0);
        yield return new WaitForSeconds(timeToRespawn);

        // Activate GameObject
        gameObject.GetComponent<Collider2D>().enabled = true; // Active and deactivate the colider to reset the OnTriggerEnter without the need of using an OnTriggerStay
        go.SetActive(true);
        m_isDestroyDueCollision = true;
        m_hasCollided = false;
    }

    private IEnumerator DeactivateGameObject(GameObject go)
    {
        // Trigger animation if any
        if (gameObject.GetComponent<AudioSource>() != null) 
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
        
        gameObject.transform.position -= new Vector3(0, 0.25f, 0);
        yield return new WaitForSeconds(timeToDestroy);

        // Deactivate GameObject
        go.SetActive(false);
        m_isDestroyDueCollision = false;
        gameObject.GetComponent<AudioSource>().Stop();

        if (m_isRespawnOverTime)
        {
            StartCoroutine(ActivateGameObject(m_objectToDestroy));
        }
    }
}
