using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class LobbyButtons : MonoBehaviour
{
    [HideInInspector]
    public MP_Lobby lobby;

    public Button startButton;
    public Button readyButton;
    public Button addPlayerButton;

    private void Start()
    {
        StartCoroutine(FindLobby());

        startButton?.onClick.AddListener(ButtonStart);
        readyButton?.onClick.AddListener(ButtonReady);
        addPlayerButton?.onClick.AddListener(ButtonAddAI);
    }

    IEnumerator FindLobby()
    {
        while(true)
        {
            lobby = FindObjectOfType<MP_Lobby>();
            if (lobby != null)
                break;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void ButtonReady()
    {
        if (lobby == null)
            lobby = FindObjectOfType<MP_Lobby>();

        lobby.ButtonReadyStartClick();
    }

    public void ButtonStart()
    {
        if (lobby == null)
            lobby = FindObjectOfType<MP_Lobby>();

        lobby.LoadGame();
    }

    public void ButtonAddAI()
    {
        Debug.Log("AI added");
        lobby.AddAI();
    }
}
