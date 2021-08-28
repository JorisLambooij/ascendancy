using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class AI_PlayerRoomScript : PlayerRoomScript
{
    [SerializeField]
    protected GameObject aiPrefab;

    public override void OnStartServer()
    {
        base.OnStartServer();

        transform.SetParent(MP_Lobby.instance.transform);//FindObjectOfType<MPMenu_NetworkRoomManager>().transform);
        gameObject.name = "AI - " + playerName;
        RpcChangeClientName(playerName);
        SetReady(true);

        SceneManager.activeSceneChanged += OnSceneChange;
    }

    protected void OnSceneChange(Scene old, Scene current)
    {
        Debug.Log(current.name);

        if (current.name != "GameScene")
            return;
        GameObject ai = Instantiate(aiPrefab);//, FindObjectOfType<MPMenu_NetworkRoomManager>().transform
        ai.GetComponent<AI_Player>().SetRoomScript(this);
        NetworkServer.Spawn(ai);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    
}
