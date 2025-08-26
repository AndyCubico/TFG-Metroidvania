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
    [TagDropdown] public string[] collisionTag = new string[] { };


    [SerializeField] private bool m_isRespawnOverTime;

    [Header("Timing")]
    [ShowIf("m_isRespawnOverTime", true)] public float timeToRespawn;
    public float timeToDestroy;

    // Weather system elements
    public Color[] waterColors = new Color[2];
    float m_frozenTime = 4;

    public AttackFlagType freezeMask;

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
                case CLIMATES.NEUTRAL:

                    // m_objectToDestroy.enabled = true;

                    break;
                case CLIMATES.SUN:

                    // m_destroy.enabled = false;
                    m_objectToDestroy.SetActive(false);


                    break;
                case CLIMATES.SNOW:



                    break;
                case CLIMATES.NONE:
                    break;
            }
        }
    }

    

    public void ReceiveDamage(float damage, AttackFlagType flag)
    {
        if ((freezeMask & flag) != 0)
        {
            if (/*!m_IsFrozen*/ true)
            {
                StartCoroutine(FreezeWaterTemporaly());
            }
        }
    }

    IEnumerator FreezeWaterTemporaly()
    {
        yield return new WaitForSeconds(m_frozenTime); // Unfreze water after 4 seconds passed, coldown of ice spell / ice special attack is 4.5 seconds,.

        if (WeatherManager.Instance.GetExteriorState()) // If is an interior area weather doesn't affect.
        {
            UpdateIce(WeatherManager.Instance.climate);
        }
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected");
        if (collisionTag.Contains(collision.gameObject.tag))
        {
            Debug.Log("Collision with: " + collision.gameObject.tag);
            // Collision detected with corresponding tag
            StartCoroutine(DeactivateGameObject(m_objectToDestroy));

        }
    }

    private IEnumerator ActivateGameObject(GameObject go)
    {
        // Trigger animation if any

        yield return new WaitForSeconds(timeToRespawn);

        // Deactivate GameObject
        go.SetActive(true);
    }

    private IEnumerator DeactivateGameObject(GameObject go)
    {
        // Trigger animation if any

        yield return new WaitForSeconds(timeToDestroy);

        // Deactivate GameObject
        go.SetActive(false);

        if (m_isRespawnOverTime)
        {
            StartCoroutine(ActivateGameObject(m_objectToDestroy));
        }
    }
}
