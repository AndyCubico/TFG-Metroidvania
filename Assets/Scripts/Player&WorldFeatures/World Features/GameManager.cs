using System;
using System.Collections;
using UnityEngine;
public static class GameManagerEvents
{
    public static Action<int> eStartPlayerPosition;
}

public class GameManager : MonoBehaviour
{
    private GameObject m_player;
    private GameObject m_startPlayerPositionObj = null;

    [HideInInspector] public int spawnNumber = 1;

    private void OnEnable()
    {
        GameManagerEvents.eStartPlayerPosition += StartPlayerPositionRequest;
    }

    private void OnDisable()
    {
        GameManagerEvents.eStartPlayerPosition -= StartPlayerPositionRequest;
    }

    void Start()
    {
        m_player = GameObject.Find("Player").gameObject;
        StartCoroutine(StartPlayerPosition(spawnNumber));
    }

    void Update()
    {
        
    }

    private void StartPlayerPositionRequest(int spawnNumber)
    {
        StartCoroutine(StartPlayerPosition(spawnNumber));
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
}
