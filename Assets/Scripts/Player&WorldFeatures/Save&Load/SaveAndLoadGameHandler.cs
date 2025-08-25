using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

//Here is where the respective classes are created, put the necessary variables to be stores in each case
public class player_SL //SL -> Save&Load
{
    public Vector3 playerPosition;
    public int charges;
    public bool snowAbilityUnlock;
}

public class object_SL //SL -> Save&Load
{
    public string objectName;
    public int objectID;
    public enum ObjectType 
    {
        HIT_LEVER,
    }
    public ObjectType objectType;
}
public class room_SL //SL -> Save&Load
{
    public string roomName;
    public List<object_SL> listObjects = new List<object_SL>();
}

public class world_SL //SL -> Save&Load
{
    public List<room_SL> listRooms = new List<room_SL>();
}

public static class SaveAndLoadEvents
{
    //The events to subscribe
    public static Action eSaveAction;
    public static Action eLoadAction;
    public static Action eRoomSaveAction;
    public static Action eRoomLoadAction;
}

    public class SaveAndLoadGameHandler : MonoBehaviour
{
    //Here the different bools for which data will be stored
    [Header("Selected Savings")]
    public bool savePlayer;
    public bool saveWorld;

    void Start()
    {
        //Create the Save folder in case if is not already been created
        SaveLoadManager.SaveAndLoadManager.Init();
    }

    void Update()
    {
        //Input keys to save or load (debuging purposes)
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl))
        {
            SaveAndLoadEvents.eSaveAction?.Invoke();
        }

        if (Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl))
        {
            SaveAndLoadEvents.eLoadAction?.Invoke();
        }
    }

    //This function sends the data to save in a .txt folder with the data in a Json string format and the name of the file
    public void Save(string json, string filename)
    {
        SaveLoadManager.SaveAndLoadManager.Save(json, filename);
    }

    //This function with the name of the file will load the information previously saved in /Saves/filename.txt
    public string Load(string filename)
    {
        string loadJson = SaveLoadManager.SaveAndLoadManager.Load(filename);
        return loadJson;
    }
}
