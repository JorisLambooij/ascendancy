using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour, PropertySubscriber<int>
{
    public Text textContent;

    public int index { get; set; }
    public MessageWindow messageWindow { get; set; }
    
    // Start is called before the first frame update
    void Start()
    {
        textContent = GetComponent<Text>();
    }

    public void Callback(int value)
    {
        if (value > index)
        {
            StartCoroutine(DestroyThis());
        }
    }

    IEnumerator DestroyThis()
    {
        yield return new WaitForEndOfFrame();
        messageWindow.currentMessageTimestamp.Unsubscribe(this);
        Destroy(this.gameObject);
        messageWindow.ScrollToBottom();
    }
}
