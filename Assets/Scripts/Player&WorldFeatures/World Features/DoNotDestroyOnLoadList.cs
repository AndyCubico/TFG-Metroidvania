using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class DoNotDestroyOnLoadList : MonoBehaviour
{
    [Header("Objects List To NOT Destroy")]
    public List<GameObject> objectsToNotDestroy;

    void Start()
    {
        StartCoroutine(RequestDontDestroyOnLoad());
    }

    void Update()
    {
        
    }

    public void AddToList(GameObject obj)
    {
        objectsToNotDestroy.Add(obj);

        DontDestroyOnLoad(obj);
    }

    public IEnumerator RequestDontDestroyOnLoad()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < objectsToNotDestroy.Count; i++)
        {
            DontDestroyOnLoad(objectsToNotDestroy[i]);
        }
    }
}
