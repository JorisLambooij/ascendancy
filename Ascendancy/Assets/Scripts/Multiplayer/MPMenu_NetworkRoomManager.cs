using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mirror
{
    public class MPMenu_NetworkRoomManager : NetworkRoomManager
    {
        public override void OnRoomServerPlayersReady()
        {
            GameObject.Find("PlayerManager").GetComponent<MP_Lobby>().InitializePlayers();
            base.OnRoomServerPlayersReady();
        }
        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);
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
    }
}