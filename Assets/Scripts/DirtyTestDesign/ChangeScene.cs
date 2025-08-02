using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [Header("Spawn To")]
    public SceneField sceneToLoad;
    public int pivotNumber;

    public bool onlyChargeOnce = false;

    private float m_MinTimeToEnableCollision = 1f;
    private float m_timeToEnableCounter = 0f;

    private void Update()
    {
        if (m_timeToEnableCounter < m_MinTimeToEnableCollision)
        {
            m_timeToEnableCounter += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision Detected");

        if (collision.gameObject.CompareTag("Player"))
        {
            if(sceneToLoad != null && !onlyChargeOnce && m_timeToEnableCounter >= m_MinTimeToEnableCollision)
            {
                StartCoroutine(LoadNewScene());
                onlyChargeOnce = true;
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
