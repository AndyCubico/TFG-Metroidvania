using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class GameManagerEvents
{
    public static Action<int> eStartPlayerPosition;
    public static Action<int, string> eSearchStartPlayerPosition;
    public static Action<Vector3, float> eSpawnDamageText;
}

public class GameManager : MonoBehaviour
{
    private GameObject m_player;
    private GameObject m_startPlayerPositionObj = null;

    public GameObject hitTextObj;

    [HideInInspector] public int spawnNumber = 1;

    private void OnEnable()
    {
        GameManagerEvents.eStartPlayerPosition += StartPlayerPositionRequest;
        GameManagerEvents.eSearchStartPlayerPosition += SearchStartPlayerPositionRequest;
        GameManagerEvents.eSpawnDamageText += RequestHitSpawnText;
    }

    private void OnDisable()
    {
        GameManagerEvents.eStartPlayerPosition -= StartPlayerPositionRequest;
        GameManagerEvents.eSearchStartPlayerPosition -= SearchStartPlayerPositionRequest;
        GameManagerEvents.eSpawnDamageText -= RequestHitSpawnText;
    }

    void Start()
    {
        m_player = GameObject.Find("Player").gameObject;
        StartCoroutine(StartPlayerPosition(spawnNumber));
        SaveAndLoadEvents.eSaveAction?.Invoke();
    }

    void Update()
    {
        
    }

    private void StartPlayerPositionRequest(int spawnNumber)
    {
        StartCoroutine(StartPlayerPosition(spawnNumber));
    }
    
    private void SearchStartPlayerPositionRequest(int spawnNumber, string nameScene)
    {
        StartCoroutine(SearchForNewStartPlayerPosition(spawnNumber, nameScene));
    }

    public IEnumerator StartPlayerPosition(int spawnNumber)
    {
        float timeToDiscard = 0f;

        if(m_startPlayerPositionObj != null)
        {
            Destroy(m_startPlayerPositionObj);
            m_startPlayerPositionObj = null;
        }

        while (m_startPlayerPositionObj == null)
        {
            timeToDiscard += Time.deltaTime;

            m_startPlayerPositionObj = GameObject.Find("PlayerStart_" + spawnNumber)?.gameObject;

            if(timeToDiscard >= 3f)
            {
                Debug.LogWarning("Time Out to search new spawn number, is posible that the number introduced or the pivot name is incorrect, pivot number is: " + spawnNumber);
                break;
            }

            yield return null;
        }

        if(m_startPlayerPositionObj != null)
        {
            m_player.gameObject.transform.position = m_startPlayerPositionObj.transform.position;
        }
    }

    public IEnumerator SearchForNewStartPlayerPosition(int spawnNumber, string sceneName)
    {
        while(SceneManager.GetActiveScene().name != sceneName)
        {
            yield return null;
        }

        var scene = SceneManager.GetActiveScene();

        foreach(var go in scene.GetRootGameObjects())
        {
            if (go.name == "Player" || go.name == "GameManager")
            {
                Destroy(go.gameObject);
            }
        }

        m_startPlayerPositionObj = GameObject.Find("PlayerStart_" + spawnNumber).gameObject;

        if (m_startPlayerPositionObj != null)
        {
            m_player.gameObject.transform.position = m_startPlayerPositionObj.transform.position;
        }
        else
        {
            Debug.LogWarning("Not found new spawn number, is posible that the number introduced or the pivot name is incorrect, pivot number is: " + spawnNumber);
        }
    }

    void RequestHitSpawnText(Vector3 spawnPosition, float damage)
    {
        StartCoroutine(SpawnDamageText(spawnPosition, damage));
    }

    IEnumerator SpawnDamageText(Vector3 spawnPosition, float damage)
    {
        float speedY = 2f;
        float disapearSpeed = 1.5f;

        GameObject obj = Instantiate(hitTextObj, spawnPosition, Quaternion.identity);

        TextMeshPro text = obj.GetComponent<TextMeshPro>();
        text.text = damage.ToString();

        while (true)
        {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y + (speedY * Time.deltaTime), obj.transform.position.z);

            if (text.color.a > 0)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - disapearSpeed * Time.deltaTime);
                yield return false;
            }
            else
            {
                Destroy(obj);
                break;
            }
        }
    }
}
