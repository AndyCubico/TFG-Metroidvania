using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDemo : MonoBehaviour
{
    [Header("Spawn To")]
    public SceneField sceneToLoad;

    [SerializeField] List<GameObject> gameObjectsToDestroy = new List<GameObject>();

    private void Start()
    {
       gameObjectsToDestroy = new List<GameObject>();

        List<GameObject> rootGameObjectsExceptDontDestroyOnLoad = new List<GameObject>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            rootGameObjectsExceptDontDestroyOnLoad.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
        }

        List<GameObject> rootGameObjects = new List<GameObject>();
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        for (int i = 0; i < allTransforms.Length; i++)
        {
            Transform root = allTransforms[i].root;
            if (root.hideFlags == HideFlags.None && !rootGameObjects.Contains(root.gameObject))
            {
                rootGameObjects.Add(root.gameObject);
            }
        }

        for (int i = 0; i < rootGameObjects.Count; i++)
        {
            if (!rootGameObjectsExceptDontDestroyOnLoad.Contains(rootGameObjects[i]))
                gameObjectsToDestroy.Add(rootGameObjects[i]);
        }

        GameObject toRemove = null;
        foreach (GameObject item in gameObjectsToDestroy)
        {
            if(item.name == "[Debug Updater]") 
            {
                toRemove = item;
            }
        }
        gameObjectsToDestroy.Remove(toRemove);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (GameObject item in gameObjectsToDestroy)
        {
            item.SetActive(false);
        }

        //foreach (GameObject item in gameObjectsToDestroy)
        //{
        //    Destroy(item);
        //}

        SceneManager.LoadScene(sceneToLoad.SceneName);
    }
}
