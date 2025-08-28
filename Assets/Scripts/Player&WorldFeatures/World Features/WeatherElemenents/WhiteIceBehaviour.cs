using PlayerController;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WhiteIceBehaviour : MonoBehaviour, IHittableObject
{
    // On collision destroy parameters
    [SerializeField] private GameObject m_objectToDestroy; bool m_isDestroyDueCollision = false;
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

                    if (m_isRespawnOverTime && !m_isDestroyDueCollision)
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

                    if (m_isRespawnOverTime && !m_isDestroyDueCollision)
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

        yield return new WaitForSeconds(m_frozenTime); // Make frail again after 4 seconds passed, coldown of ice spell / ice special attack is 4.5 seconds,.

        m_isFrozen = false;

        if (WeatherManager.Instance.GetExteriorState()) // If is an interior area weather doesn't affect.
        {
            UpdateIce(WeatherManager.Instance.climate);
        }
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collisionTag.Contains(collision.gameObject.tag) && !m_isFrozen)
        {
            Debug.Log("Collision with: " + collision.gameObject.tag);
            // Collision detected with corresponding tag

            if (!WeatherManager.Instance.GetExteriorState() || (WeatherManager.Instance.climate != CLIMATES.SUN)) 
            {
                StartCoroutine(DeactivateGameObject(m_objectToDestroy));
            }
            

        }
    }

    private IEnumerator ActivateGameObject(GameObject go)
    {
        // Trigger animation if any

        yield return new WaitForSeconds(timeToRespawn);

        // Deactivate GameObject
        go.SetActive(true);
        m_isDestroyDueCollision = true;
    }

    private IEnumerator DeactivateGameObject(GameObject go)
    {
        // Trigger animation if any

        yield return new WaitForSeconds(timeToDestroy);

        // Deactivate GameObject
        go.SetActive(false);
        m_isDestroyDueCollision = false;

        if (m_isRespawnOverTime)
        {
            StartCoroutine(ActivateGameObject(m_objectToDestroy));
        }
    }
}
