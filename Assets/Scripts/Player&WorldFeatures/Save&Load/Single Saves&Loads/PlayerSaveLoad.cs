using PlayerController;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            SaveAndLoadEvents.eSaveAction += Save;
            SaveAndLoadEvents.eLoadAction += Load;
        }
    }

    private void OnDisable()
    {
        if (saveLoad != null)
        {
            //Save&Load
            SaveAndLoadEvents.eSaveAction -= Save;
            SaveAndLoadEvents.eLoadAction -= Load;
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
                playerPosition = this.gameObject.transform.position,
                charges = GameObject.Find("HeavyAttack").GetComponent<HeavyAttack>().heavyCharges,
                snowAbilityUnlock = GameObject.Find("SpecialAttacks").GetComponent<SpecialAbilities>().snowAbilityUnlocked,
                lastSavedScene = SceneManager.GetActiveScene().name,
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

            if (SceneManager.GetActiveScene().name != saveObject.lastSavedScene)
            {
                SceneManager.LoadScene(saveObject.lastSavedScene);
            }

            this.transform.position = saveObject.playerPosition;

            GameObject.Find("HeavyAttack").GetComponent<HeavyAttack>().heavyCharges = saveObject.charges;
            GameObject.Find("HeavyAttack").GetComponent<HeavyAttack>().UpdateCharges();

            GameObject.Find("SpecialAttacks").GetComponent<SpecialAbilities>().snowAbilityUnlocked = saveObject.snowAbilityUnlock;

            this.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            this.gameObject.GetComponent<CharacterPlayerController>().enabled = true;
        }
    }
}
