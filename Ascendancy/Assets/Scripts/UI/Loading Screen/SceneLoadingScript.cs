using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadingScript : MonoBehaviour
{
    List<string> sceneNames;

    //[SerializeField]
    public Image progressBarImage;

    // Start is called before the first frame update
    void Start()
    {
        //collecting scene names
        sceneNames = new List<string>();

#if (UNITY_EDITOR)
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                sceneNames.Add(System.IO.Path.GetFileNameWithoutExtension(scene.path));
            }
        }
#endif

#if (UNITY_STANDALONE_WIN)
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            sceneNames.Add(System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)));
        }
#endif

        if (sceneNames.Contains(NextSceneStatic.sceneName))
        {
            //start async operation
            StartCoroutine(LoadAsyncOperation());
        }
        else
        {
            Debug.LogError("Scene '" + NextSceneStatic.sceneName + "' could not be loaded. Maybe it is not enabled?");
            foreach (string sceneName in sceneNames)
                Debug.Log("'" + sceneName + "'");
        }

    }

    IEnumerator LoadAsyncOperation()
    {
        //create new async operation
        AsyncOperation loadLevel = SceneManager.LoadSceneAsync(NextSceneStatic.sceneName);

        //while still loading fill progress bar
        while (loadLevel.progress < 1)
        {
            progressBarImage.fillAmount = loadLevel.progress;
            yield return new WaitForEndOfFrame();
        }
    }
}
