using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadingScript : MonoBehaviour
{
    [SerializeField]
    private Image progressBarImage;

    // Start is called before the first frame update
    void Start()
    {
        //start async operation
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        //create new async operation
        AsyncOperation loadLevel = SceneManager.LoadSceneAsync("Development");

        //while still loading fill progress bar
        while(loadLevel.progress < 1)
        {
            progressBarImage.fillAmount = loadLevel.progress;
            yield return new WaitForEndOfFrame();
        }
    }
}
