using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : MonoBehaviour
{
    public int maximumNumberOfMessages;
    public Text messageWindowText;
    public Transform chatWindow;
    public GameObject messagePrefab;
    public ScrollRect scrollRect;

    public SubscribableProperty<int> currentMessageTimestamp;

    private bool gluedToBottom;
    private const float epsilon = 0.01f;

    void Start()
    {
        gluedToBottom = true;
        currentMessageTimestamp = new SubscribableProperty<int>(0);

        ReceiveMessage("SYSTEM", "Lobby created");

        for (int i = 0; i < 25; i++)
            ReceiveMessage("TEST", i.ToString());
    }

    public void ReceiveMessage(string sender, string message)
    {
        currentMessageTimestamp.Value++;

        message = sender + " [" + System.DateTime.Now.Hour.ToString("00") + ":" + System.DateTime.Now.Minute.ToString("00") + "]: " + message;
        ChatMessage chatMessage = Instantiate(messagePrefab, chatWindow).GetComponent<ChatMessage>();
        chatMessage.textContent.text = message;
        chatMessage.index = currentMessageTimestamp.Value + maximumNumberOfMessages;
        chatMessage.messageWindow = this;
        currentMessageTimestamp.Subscribe(chatMessage);
        
        gluedToBottom = scrollRect.verticalNormalizedPosition < epsilon;
        if (gluedToBottom)
            ScrollToBottom();
    }

    public void ScrollToBottom()
    {
        StartCoroutine(ScrollToBottomCoroutine());
    }

    private IEnumerator ScrollToBottomCoroutine()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0;
    }

    public void OnScroll()
    {
        gluedToBottom = scrollRect.verticalNormalizedPosition < epsilon;
    }
}
