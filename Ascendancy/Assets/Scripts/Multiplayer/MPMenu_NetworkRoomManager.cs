using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mirror
{
    public class MPMenu_NetworkRoomManager : NetworkRoomManager
    {
        public GameObject playermanagerPrefab;
        private Dictionary<int, Player> playerDict;

        public override void OnRoomStartServer()
        {
            base.OnRoomStartServer();
            ServerListen();
        }

        public override void OnRoomServerPlayersReady()
        {
            Debug.Log("Players Ready!");
            FindObjectOfType<MP_Lobby>().ToggleStartButton(true);
            //GameObject.Find("PlayerManager").GetComponent<MP_Lobby>().InitializePlayers();
            base.OnRoomServerPlayersReady();
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);

            Debug.Log("Scene: " + sceneName);
            if (sceneName == "Assets/Scenes/Multiplayer/Lobby.unity")
            {
                GameObject playerManager = Instantiate(playermanagerPrefab);
                //playerManager.name = "PlayerManager";
                NetworkServer.Spawn(playerManager);
            }
        }

        public void InitPlayerDict(Dictionary<int, Player> dictNew)
        {
            playerDict = dictNew;
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "Lobby":
                    Debug.Log("Client [" + conn.address + "] disconnected from the server!");
                    MP_Lobby lobby = GameObject.Find("PlayerManager").GetComponent<MP_Lobby>();
                    lobby.PlayerDisconnected(conn.identity);

                    break;
                default:
                    break;
            }
            base.OnServerDisconnect(conn);
        }

        public void ServerListen()
        {
            NetworkServer.RegisterHandler<ReadyMessage>(OnClientReady);
            NetworkServer.RegisterHandler<ConnectMessage>(OnServerConnect);

        }
        void OnClientReady(NetworkConnection conn, ReadyMessage msg)
        {
            Debug.Log("Client is ready to start: " + conn);
            NetworkServer.SetClientReady(conn);
        }
        void OnServerConnect(NetworkConnection conn, ConnectMessage msg)
        {
            Debug.Log("New client connected: " + conn);
        }

    }
}