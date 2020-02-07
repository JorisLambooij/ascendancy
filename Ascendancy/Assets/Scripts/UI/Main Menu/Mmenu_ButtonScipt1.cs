using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mmenu_ButtonScipt1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BT1_click()
    {
        NextSceneStatic.sceneName = "Development";
        SceneManager.LoadScene("LoadScreen");
    }

    public void BT2_click()
    {

    }

    public void BT3_click()
    {
        NextSceneStatic.sceneName = "MPMenu";
        SceneManager.LoadScene("LoadScreen");
    }

    public void BT4_click()
    {
        NextSceneStatic.sceneName = "MapEditor";
        SceneManager.LoadScene("LoadScreen");
    }

    public void BT5_click()
    {

    }

    public void BT6_click()
    {

    }

    public void BT7_click()
    {

    }

    public void BT8_click()
    {
        Application.Quit();
    }
}
