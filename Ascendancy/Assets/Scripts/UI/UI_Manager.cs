using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [Tooltip("Primary Screen UI")]
    public UI_Screen gameScreen;
    
    protected List<UI_Screen> uiList;

    void Start()
    {
        uiList = new List<UI_Screen>(GetComponentsInChildren<UI_Screen>());
        uiList.Remove(gameScreen);
        //CloseAllScreens();
    }

    /// <summary>
    /// Open a screen.
    /// </summary>
    /// <param name="uiName">Which UI to open.</param>
    /// <param name="hideGameUI">Close the main UI of the game (i.e. is this a whole new window?)</param>
    public void OpenScreen(string uiName, bool hideGameUI = false)
    {
        CloseSecondaryScreens();
        gameScreen.SetStatus(!hideGameUI);

        SetScreen(uiName, true);
    }

    public void OpenScreenExclusive(string uiName)
    {
        OpenScreen(uiName, true);
    }
    
    public void SetScreen(string uiName, bool status)
    {
        UI_Screen ui = uiList.Find(s => s.name.ToLower() == uiName.ToLower());

        if (ui != null)
            ui.SetStatus(status);
    }
    
    /// <summary>
    /// Close all screens and return to game UI.
    /// </summary>
    public void CloseAllScreens()
    {
        // Close all secondary screens.
        CloseSecondaryScreens();

        // Set main Game UI.
        gameScreen.SetStatus(true);
    }

    /// <summary>
    /// Close all secondary screens.
    /// </summary>
    private void CloseSecondaryScreens()
    {
        foreach (UI_Screen ui in uiList)
            ui.SetStatus(false);
    }
}
