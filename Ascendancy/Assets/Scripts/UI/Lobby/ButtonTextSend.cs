using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextSend : MonoBehaviour
{
    public InputField inputBox;
    public MessageWindow msgWindow;

    public void OnClick()
    {
        if (inputBox.text != "")
        {
            msgWindow.ReceiveMessage("PLAYERNAME", inputBox.text);
            inputBox.text = "";
        }
    }
}
