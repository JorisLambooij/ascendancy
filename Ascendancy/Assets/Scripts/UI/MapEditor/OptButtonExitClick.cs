using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptButtonExitClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClick()
    {
        NextSceneStatic.sceneName = "MainMenu";
        SceneManager.LoadScene("LoadScreen");
    }
}
