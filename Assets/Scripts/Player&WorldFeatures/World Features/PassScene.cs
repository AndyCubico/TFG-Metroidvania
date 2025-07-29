using System.Collections;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PassScene : MonoBehaviour
{
    public SceneField m_SceneToLoad;

    public bool preLoadScene;
    public bool loadScene;
    public int spawnNumber;

    private bool m_onlyOnce = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SceneExists(m_SceneToLoad.SceneName))
        {
            if (collision.CompareTag("Player") && !m_onlyOnce)
            {
                m_onlyOnce = true;

                if (preLoadScene)
                {
                    LoadSceneManager.ePreLoadScene?.Invoke(m_SceneToLoad.SceneName);
                }
                else if (loadScene)
                {
                    StartCoroutine(LoadNewScene());
                }
            }
        }
        else
        {
            Debug.LogWarning("The scene to load called: " + m_SceneToLoad.SceneName + " does not exist in the build settings");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (SceneExists(m_SceneToLoad.SceneName))
        {
            if (collision.CompareTag("Player") && m_onlyOnce)
            {
                m_onlyOnce = false;

                if (preLoadScene)
                {
                    LoadSceneManager.eUnLoadScene?.Invoke(m_SceneToLoad.SceneName);
                }
            }
        }
        else
        {
            Debug.LogWarning("The scene to unload called: " + m_SceneToLoad.SceneName + " does not exist in the build settings");
        }
    }

    public static bool SceneExists(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);

            if (string.Equals(name, sceneName, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private IEnumerator LoadNewScene()
    {
        FadeToBlackEvents.eFadeToBlackAction?.Invoke(0.02f, 1.5f);

        yield return new WaitForSeconds(0.03f);

        LoadSceneManager.eLoadScene?.Invoke(m_SceneToLoad.SceneName);
        GameManagerEvents.eStartPlayerPosition?.Invoke(spawnNumber);
    }
}
