
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : MonoBehaviour
{
    public bool displayTestMessages;
    public int maximumNumberOfMessages;
    public GameObject messagePrefab;
    public ScrollRect scrollRect;

    public Transform chatWindow;

    public SubscribableProperty<int> currentMessageTimestamp;

    private bool gluedToBottom;
    private const float epsilon = 0.01f;


    void Start()
    {
        gluedToBottom = true;
        currentMessageTimestamp = new SubscribableProperty<int>(0);



        if (displayTestMessages)
            for (int i = 0; i < 25; i++)
                PrintMessage(new ChatMessage("TEST", i.ToString(), Color.gray));
    }

    public void PrintMessage(ChatMessage chatMessage)
    {
        currentMessageTimestamp.Value++;

        string text = "<color=#" + ColorUtility.ToHtmlStringRGBA(chatMessage.color) + ">" + chatMessage.sender + "</color>" + " [" + System.DateTime.Now.Hour.ToString("00") + ":" + System.DateTime.Now.Minute.ToString("00") + "]: " + chatMessage.message;
        Debug.Log(text);
        ChatMessageGO chatMessageGO = Instantiate(messagePrefab, chatWindow).GetComponent<ChatMessageGO>();
        chatMessageGO.textContent.text = text;
        chatMessageGO.Index = currentMessageTimestamp.Value + maximumNumberOfMessages;
        chatMessageGO.MessageWindow = this;
        currentMessageTimestamp.Subscribe(chatMessageGO);

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
