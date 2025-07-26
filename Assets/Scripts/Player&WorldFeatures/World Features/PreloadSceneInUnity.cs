using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoadSceneManager
{
    public static Action<string> ePreLoadScene;
    public static Action<string> eUnLoadScene;
    public static Action<string> eLoadScene;
}


// https://github.com/PixelLifetime/unity-game-development-stack-exchange/blob/master/Assets/%7B%40%23%7D%20Answers/Preload%20scene%20in%20unity/PreloadSceneInUnity.cs
public class PreloadSceneInUnity : MonoBehaviour
{
	private AsyncOperation _asyncOperation;

    private void OnEnable()
    {
        LoadSceneManager.ePreLoadScene += OnPreLoadSceneRequest;
        LoadSceneManager.eUnLoadScene += OnUnLoadSceneRequest;
        LoadSceneManager.eLoadScene += OnLoadSceneRequest;
    }

    private void OnDisable()
    {
        LoadSceneManager.ePreLoadScene -= OnPreLoadSceneRequest;
        LoadSceneManager.eUnLoadScene -= OnUnLoadSceneRequest;
        LoadSceneManager.eLoadScene -= OnLoadSceneRequest;
    }

	private void OnPreLoadSceneRequest(string sceneName)
    {
        this.StartCoroutine(this.LoadSceneAsyncProcess(sceneName));
    }

    private void OnUnLoadSceneRequest(string sceneName)
    {
        this.StartCoroutine(this.UnLoadSceneAsyncProcess(sceneName));
    }

    private void OnLoadSceneRequest(string sceneName)
    {
        Debug.Log("Allowed Scene Activation");
        this._asyncOperation.allowSceneActivation = true; 

        var scene = SceneManager.GetSceneByName(sceneName);

        foreach (var go in scene.GetRootGameObjects())
        {
            if(go.name != "Player" && go.name != "GameManager")
            {
                go.SetActive(true);
            }
            else
            {
                Destroy(go);
            }
        }

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }

    private IEnumerator LoadSceneAsyncProcess(string sceneName)
	{
		this._asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

		this._asyncOperation.allowSceneActivation = false;

        while (_asyncOperation.progress < 0.9f)
        {
            Debug.Log($"[scene]:{sceneName} [load progress]: {_asyncOperation.progress}");
            yield return null;
        }

        _asyncOperation.allowSceneActivation = true;

        yield return new WaitUntil(() => _asyncOperation.isDone);

        OnLoaded(sceneName);
    }

    //https://stackoverflow.com/questions/44727881/how-to-use-scenemanager-unloadsceneasync
    private IEnumerator UnLoadSceneAsyncProcess(string sceneName)
	{
        yield return new WaitUntil(() => _asyncOperation.progress >= 0.9f);

        // Forza activación y espera a que concluya la carga
        //_asyncOperation.allowSceneActivation = true;
        yield return _asyncOperation;

        // Descarga la escena inmediatamente
        SceneManager.UnloadSceneAsync(sceneName);

        _asyncOperation = null;
    }

    private void OnLoaded(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);

        foreach (var go in scene.GetRootGameObjects())
        {
            go.SetActive(false);
        }

        _asyncOperation.allowSceneActivation = false;
    }
}