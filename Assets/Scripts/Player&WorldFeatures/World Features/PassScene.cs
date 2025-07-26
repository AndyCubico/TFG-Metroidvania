using System.Collections;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PassScene : MonoBehaviour
{
    public string m_sceneToLoad;

    public bool m_preLoadScene;
    public bool m_loadScene;
    public int spawnNumber;

    private bool m_onlyOnce = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (SceneExists(m_sceneToLoad))
        {
            if (collision.CompareTag("Player") && !m_onlyOnce)
            {
                m_onlyOnce = true;

                if (m_preLoadScene)
                {
                    LoadSceneManager.ePreLoadScene?.Invoke(m_sceneToLoad);
                }
                else if (m_loadScene)
                {
                    StartCoroutine(LoadNewScene());
                }
            }
        }
        else
        {
            Debug.LogWarning("The scene to load called: " + m_sceneToLoad + " does not exist in the build settings");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (SceneExists(m_sceneToLoad))
        {
            if (collision.CompareTag("Player") && m_onlyOnce)
            {
                m_onlyOnce = false;

                if (m_preLoadScene)
                {
                    LoadSceneManager.eUnLoadScene?.Invoke(m_sceneToLoad);
                }
            }
        }
        else
        {
            Debug.LogWarning("The scene to unload called: " + m_sceneToLoad + " does not exist in the build settings");
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

        LoadSceneManager.eLoadScene?.Invoke(m_sceneToLoad);
        GameManagerEvents.eStartPlayerPosition?.Invoke(spawnNumber);
    }
}
