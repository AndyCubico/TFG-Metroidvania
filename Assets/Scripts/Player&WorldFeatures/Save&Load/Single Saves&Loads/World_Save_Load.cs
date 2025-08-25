using PlayerController;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines.ExtrusionShapes;
using static HitLever;

public class World_Save_Load : MonoBehaviour
{
    private SaveAndLoadGameHandler saveLoad;
    public world_SL saveObject;

    private void OnEnable()
    {
        //Search for the GameManager object that manages the script Save_And_Load_Game_Handler
        saveLoad = GameObject.Find("GameManager")?.GetComponent<SaveAndLoadGameHandler>();

        if (saveLoad != null)
        {
            //Save&Load subsciptions
            SaveAndLoadEvents.eRoomSaveAction += Save;
            SaveAndLoadEvents.eRoomLoadAction += Load;
        }
    }

    private void OnDisable()
    {
        if (saveLoad != null)
        {
            //Save&Load desubsciptions
            SaveAndLoadEvents.eRoomSaveAction -= Save;
            SaveAndLoadEvents.eRoomLoadAction -= Load;
        }
    }

    private void Start()
    {
        saveObject = new world_SL() { }; 
        saveObject.listRooms = new List<room_SL>();
    }

    void Save()
    {
        if (saveLoad.saveWorld)
        {
            //Class selection, it will depend on what you want to save, this class is before created in script Save_And_Load_Game_Handler, this example is with player_SL, change it with the one you want
            
            bool hasSceneAlready = false;
            for(int i = 0; i < saveObject.listRooms.Count; i++)
            {
                if(saveObject.listRooms[i].roomName == SceneManager.GetActiveScene().name) 
                {
                    hasSceneAlready = true;
                    break;
                }
            }

            if (!hasSceneAlready) 
            {
                room_SL saveRoom = new room_SL
                {
                    roomName = SceneManager.GetActiveScene().name,
                    listObjects = new List<object_SL>()
                };
                //saveObject.listRooms.Add
            }
            string json = JsonUtility.ToJson(saveObject);

            saveLoad.Save(json, "WorldSave");
        }
    }

    public void SaveObject(object_SL obj) 
    {
        bool hasSceneAlready = false;
        for (int i = 0; i < saveObject.listRooms.Count; i++)
        {
            if (saveObject.listRooms[i].roomName == SceneManager.GetActiveScene().name)
            {
                hasSceneAlready = true;
                bool objExists = false;
                for(int j = 0; j< saveObject.listRooms[i].listObjects.Count; j++) 
                {
                    if (saveObject.listRooms[i].listObjects[j].objectName == obj.objectName && saveObject.listRooms[i].listObjects[j].objectID == obj.objectID) 
                    {
                        objExists = true;
                        saveObject.listRooms[i].listObjects[j] = obj; //Substitute old object by new one
                        break;
                    }
                }
                if (!objExists)
                {
                    saveObject.listRooms[i].listObjects.Add(obj);
                }

                break;
            }
        }

        if (!hasSceneAlready)
        {
            room_SL saveRoom = new room_SL
            {
                roomName = SceneManager.GetActiveScene().name,
                listObjects = new List<object_SL>()
            };
            saveRoom.listObjects.Add(obj);

            saveObject.listRooms.Add(saveRoom);

        }
            string json = JsonUtility.ToJson(saveObject);

        saveLoad.Save(json, "WorldSave");
    }

    void Load()
    {
        //if (saveLoad.savePlayer/*bool name to know if has to be saved, in this case is the savePlayer as an example change it with the one you want*/)
        //{
        //    //Store the content of the json returned as the type of your class, in this case the player_SL is put as an example
        //    /*player_SL*/ saveObject = JsonUtility.FromJson</*player_SL*/>(saveLoad.Load("The name of your file"));

        //    //Apply the information as you want, here the player position is changed as an example
        //    this.transform.position = saveObject.playerPosition;
        //}
    }

    public object_SL LoadObject(object_SL obj) 
    {
        for (int i = 0; i < saveObject.listRooms.Count; i++) //Search for the scene in the save object 
        {
            if (saveObject.listRooms[i].roomName == SceneManager.GetActiveScene().name)
            {
                
                for (int j = 0; j < saveObject.listRooms[i].listObjects.Count; j++)
                {
                    if (saveObject.listRooms[i].listObjects[j].objectName == obj.objectName && saveObject.listRooms[i].listObjects[j].objectID == obj.objectID)
                    {
                        switch (obj.objectType)
                        {
                            case object_SL.ObjectType.HIT_LEVER:


                                HitLever_SL rObj = (HitLever_SL)saveObject.listRooms[i].listObjects[j];

                                return rObj;

                                break;
                            default:
                                break;
                        }
                        break;
                    }
                }
                break;
            }
        }
        return null;
    }
}
