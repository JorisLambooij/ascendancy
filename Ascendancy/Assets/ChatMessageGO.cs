using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessageGO : MonoBehaviour, PropertySubscriber<int>
{
    public Text textContent;

    public int Index { get; set; }
    public MessageWindow MessageWindow { get; set; }
    
    // Start is called before the first frame update
    void Start()
    {
        textContent = GetComponent<Text>();
    }

    public void Callback(int value)
    {
        if (value > Index)
        {
            StartCoroutine(DestroyThis());
        }
    }

    IEnumerator DestroyThis()
    {
        yield return new WaitForEndOfFrame();
        MessageWindow.currentMessageTimestamp.Unsubscribe(this);
        Destroy(this.gameObject);
        MessageWindow.ScrollToBottom();
    }
}
