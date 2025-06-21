using System;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class DoNotDestroyOnLoadList : MonoBehaviour
{
    [Header("Objects List To NOT Destroy")]
    public List<GameObject> objectsToNotDestroy;

    void Start()
    {
        for (int i = 0; i < objectsToNotDestroy.Count; i++)
        {
            DontDestroyOnLoad(objectsToNotDestroy[i]);
        }
    }

    void Update()
    {
        
    }

    public void AddToList(GameObject obj)
    {
        objectsToNotDestroy.Add(obj);

        DontDestroyOnLoad(obj);
    }
}
