using System;
using UnityEngine;

//Here is where the respective classes are created, put the necessary variables to be stores in each case
public class player_SL //SL -> Save&Load
{
    public Vector3 playerPosition;
}

public class Save_And_Load_Game_Handler : MonoBehaviour
{
    //Here the different bools for which data will be stored
    [Header("Selected Savings")]
    public bool savePlayer;

    //The events to subscribe
    public event Action SaveAction;
    public event Action LoadAction;

    void Start()
    {
        //Create the Save folder in case if is not already been created
        SaveLoad_Manager.Save_And_Load_Manager.Init();
    }

    void Update()
    {
        //Input keys to save or load (debuging purposes)
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl))
        {
            SaveAction?.Invoke();
        }

        if (Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl))
        {
            LoadAction?.Invoke();
        }
    }

    //This function sends the data to save in a .txt folder with the data in a Json string format and the name of the file
    public void Save(string json, string filename)
    {
        SaveLoad_Manager.Save_And_Load_Manager.Save(json, filename);
    }

    //This function with the name of the file will load the information previously saved in /Saves/filename.txt
    public string Load(string filename)
    {
        string loadJson = SaveLoad_Manager.Save_And_Load_Manager.Load(filename);
        return loadJson;
    }
}
