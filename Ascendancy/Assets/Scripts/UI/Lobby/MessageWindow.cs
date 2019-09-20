using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : MonoBehaviour
{
    public Text messageWindowText;

    void Start()
    {
        ReceiveMessage("SYSTEM", "Lobby created");
    }

    public void ReceiveMessage(string sender, string message)
    {
        message = sender + " [" + System.DateTime.Now.Hour.ToString("00") + ":" + System.DateTime.Now.Minute.ToString("00") + "]: " + message;
        messageWindowText.text = messageWindowText.text + "\n" + message;
    }

}
