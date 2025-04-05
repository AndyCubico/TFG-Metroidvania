using Unity.Entities;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public float fadeInTime;
    public float fadeOutTime;

    public float timer;
    public float weatherDuration = 10f;

    public EntityManager entityManager;
    public Entity weatherEntity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = weatherDuration;
        //entityManager = Helper.GetEntityManager();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            //entityManager.SetComponentEnabled<weather.RainComponent>(weatherEntity, true);
            //entityManager.SetComponentEnabled<weather.RainComponent>(weatherEntity, false);
        }
    }
}
