using UnityEngine;
using Player_Controller;
using UnityEngine.Splines.ExtrusionShapes;

public class Template_Save_Load : MonoBehaviour
{
    private Save_And_Load_Game_Handler saveLoad;

    private void OnEnable()
    {
        //Search for the GameManager object that manages the script Save_And_Load_Game_Handler
        saveLoad = GameObject.Find("GameManager")?.GetComponent<Save_And_Load_Game_Handler>();

        if (saveLoad != null)
        {
            //Save&Load subsciptions
            saveLoad.SaveAction += Save;
            saveLoad.LoadAction += Load;
        }
    }

    private void OnDisable()
    {
        if (saveLoad != null)
        {
            //Save&Load desubsciptions
            saveLoad.SaveAction -= Save;
            saveLoad.LoadAction -= Load;
        }
    }

    void Save()
    {
        //if (saveLoad.savePlayer/*bool name to know if has to be saved, in this case is the savePlayer as an example change it with the one you want*/)
        //{
        //    //Class selection, it will depend on what you want to save, this class is before created in script Save_And_Load_Game_Handler, this example is with player_SL, change it with the one you want
        //    player_SL saveObject = new player_SL
        //    {
        //        playerPosition = this.gameObject.transform.position
        //    };

        //    string json = JsonUtility.ToJson(saveObject);

        //    saveLoad.Save(json, "The name of your file");
        //}
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
