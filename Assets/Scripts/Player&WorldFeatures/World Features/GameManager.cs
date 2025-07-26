using System;
using System.Collections;
using UnityEngine;
public static class GameManagerEvents
{
    public static Action eStartPlayerPosition;
}

public class GameManager : MonoBehaviour
{
    private GameObject m_player;
    GameObject startPlayerPositionObj = null;

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
        StartCoroutine(StartPlayerPosition());
    }

    void Update()
    {
        
    }

    private void StartPlayerPositionRequest()
    {
        StartCoroutine(StartPlayerPosition());
    }

    public IEnumerator StartPlayerPosition()
    {
        if(startPlayerPositionObj != null)
        {
            Destroy(startPlayerPositionObj);
            startPlayerPositionObj = null;
        }

        while (startPlayerPositionObj == null)
        {
            startPlayerPositionObj = GameObject.Find("PlayerStart").gameObject;

            yield return null;
        }

        m_player.gameObject.transform.position = startPlayerPositionObj.transform.position;
    }
}
