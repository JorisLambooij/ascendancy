using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatMessage
{
    public string sender;
    public string message;
    public Color color;

    public ChatMessage(string sender, string message, Color color)
    {
        this.sender = sender;
        this.message = message;
        this.color = color;
    }

    public ChatMessage()
    {
        this.sender = "System";
        this.message = "Error!";
        this.color = Color.red;
    }
}
