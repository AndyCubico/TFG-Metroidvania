using UnityEngine;
using PlayerController;
using UnityEngine.Splines.ExtrusionShapes;

public class PlayerSaveLoad : MonoBehaviour
{
    private SaveAndLoadGameHandler saveLoad;
    CharacterController characterController;

    private void OnEnable()
    {
        saveLoad = GameObject.Find("GameManager")?.GetComponent<SaveAndLoadGameHandler>();

        if (saveLoad != null)
        {
            //Save&Load
            saveLoad.SaveAction += Save;
            saveLoad.LoadAction += Load;
        }
    }

    private void OnDisable()
    {
        if (saveLoad != null)
        {
            //Save&Load
            saveLoad.SaveAction -= Save;
            saveLoad.LoadAction -= Load;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = this.gameObject.GetComponent<CharacterController>();
    }

    void Save()
    {
        if (saveLoad.savePlayer)
        {
            player_SL saveObject = new player_SL
            {
                playerPosition = this.gameObject.transform.position
            };

            string json = JsonUtility.ToJson(saveObject);

            saveLoad.Save(json, "PlayerSave");
        }
    }

    void Load()
    {
        if (saveLoad.savePlayer)
        {
            player_SL saveObject = JsonUtility.FromJson<player_SL>(saveLoad.Load("PlayerSave"));

            this.transform.position = saveObject.playerPosition;
        }
    }
}
