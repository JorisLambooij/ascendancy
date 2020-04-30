using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldManager : MonoBehaviour
{

    public InputField inputField;
    public PrefManager prefManager;

    private void Start()
    {
        string newName = prefManager.GetPlayerName();

        if (newName != "")
            inputField.text = newName;
    }

    public void OnEndEdit()
    {
        prefManager.SetPlayerName(inputField.text);
    }
}
