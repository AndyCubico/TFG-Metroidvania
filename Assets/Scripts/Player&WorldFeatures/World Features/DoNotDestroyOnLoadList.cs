using System;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class DoNotDestroyOnLoadList : MonoBehaviour
{
    [Header("Objects List To NOT Destroy")]
    public List<String> objectsToNotDestroy;

    void Start()
    {
        for (int i = 0; i < objectsToNotDestroy.Count; i++)
        {
            DontDestroyOnLoad(GameObject.Find(objectsToNotDestroy[i]));
        }
    }

    void Update()
    {
        
    }

    public void AddToList(string name)
    {
        objectsToNotDestroy.Add(name);

        DontDestroyOnLoad(GameObject.Find(objectsToNotDestroy[objectsToNotDestroy.Count - 1]));
    }
}
