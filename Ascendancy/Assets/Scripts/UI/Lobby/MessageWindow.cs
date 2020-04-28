using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : NetworkBehaviour
{
    public bool displayTestMessages;
    public int maximumNumberOfMessages;
    //public Text messageWindowText;
    public GameObject messagePrefab;
    public ScrollRect scrollRect;

    [SyncVar]
    public Transform chatWindow;

    public SubscribableProperty<int> currentMessageTimestamp;

    private bool gluedToBottom;
    private const float epsilon = 0.01f;

    private MPMenu_NetworkRoomManager roomMngr;
    private bool isServer = false;


    void Start()
    {
        roomMngr = GameObject.Find("NetworkManager").GetComponent<MPMenu_NetworkRoomManager>();
        if (roomMngr.mode == NetworkManagerMode.Host)
            isServer = true;
        else
            isServer = false;

        gluedToBottom = true;
        currentMessageTimestamp = new SubscribableProperty<int>(0);

        if (isServer)
            ReceiveMessage("SYSTEM", Color.gray, "Lobby created");

        if (displayTestMessages)
            for (int i = 0; i < 25; i++)
                ReceiveMessage("TEST", Color.gray, i.ToString());
    }

    public void ReceiveMessage(string sender, Color color, string message)
    {
        if (isServer)
        {
            currentMessageTimestamp.Value++;

            message = "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + sender + "</color>" + " [" + System.DateTime.Now.Hour.ToString("00") + ":" + System.DateTime.Now.Minute.ToString("00") + "]: " + message;
            Debug.Log(message);
            ChatMessage chatMessage = Instantiate(messagePrefab, chatWindow).GetComponent<ChatMessage>();
            chatMessage.textContent.text = message;
            chatMessage.Index = currentMessageTimestamp.Value + maximumNumberOfMessages;
            chatMessage.MessageWindow = this;
            currentMessageTimestamp.Subscribe(chatMessage);

            gluedToBottom = scrollRect.verticalNormalizedPosition < epsilon;
            if (gluedToBottom)
                ScrollToBottom();
        }
        else
            CmdReceiveMessage(sender, color, message);
    }

    [Command]
    public void CmdReceiveMessage(string sender, Color color, string message)
    {
        ReceiveMessage(sender, color, message);
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
