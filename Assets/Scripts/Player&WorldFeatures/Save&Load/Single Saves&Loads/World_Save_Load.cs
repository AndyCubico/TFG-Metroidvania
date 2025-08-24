using PlayerController;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines.ExtrusionShapes;

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
        world_SL saveObject = new world_SL();
    }

    void Save()
    {
        if (saveLoad.savePlayer/*bool name to know if has to be saved, in this case is the savePlayer as an example change it with the one you want*/)
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
                
                saveObject.listRooms.Add
            }
            string json = JsonUtility.ToJson(saveObject);

            saveLoad.Save(json, "WorldSave");
        }
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
}
