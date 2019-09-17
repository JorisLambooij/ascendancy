using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Canvas : MonoBehaviour
{
    public GameObject gameScreen;
    public GameObject techScreen;
    public GameObject buildMenu;
    
    void Start()
    {
        
    }

    public void ToggleBuildMenu()
    {
        bool isActive = buildMenu.activeSelf;
        buildMenu.SetActive(!isActive);
    }

    public void OpenTechScreen()
    {
        gameScreen.SetActive(false);
        techScreen.SetActive(true);
    }

    public void CloseAllScreens()
    {
        gameScreen.SetActive(true);
        techScreen.SetActive(false);
    }
}
