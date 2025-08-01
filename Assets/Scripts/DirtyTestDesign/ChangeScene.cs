using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [Header("Spawn To")]
    public SceneField sceneToLoad;
    public int pivotNumber;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision Detected");
        if (collision.gameObject.CompareTag("Player"))
        {
            if(sceneToLoad != null)
            {
                StartCoroutine(LoadNewScene());
            }
        }
    }

    private IEnumerator LoadNewScene()
    {
        FadeToBlackEvents.eFadeToBlackAction?.Invoke(0.001f, 1.5f);

        yield return new WaitForSeconds(0.03f);

        SceneManager.LoadScene(sceneToLoad.SceneName);
        GameManagerEvents.eSearchStartPlayerPosition?.Invoke(pivotNumber, sceneToLoad.SceneName);
    }
}
