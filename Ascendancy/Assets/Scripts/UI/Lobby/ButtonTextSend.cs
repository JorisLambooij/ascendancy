using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextSend : MonoBehaviour
{
    public InputField inputBox;
    public MP_Lobby lobby;

    public void OnClick()
    {
        if (inputBox.text != "")
        {
            lobby.SendChatMessage(inputBox.text);
            inputBox.text = "";
        }
    }
}
