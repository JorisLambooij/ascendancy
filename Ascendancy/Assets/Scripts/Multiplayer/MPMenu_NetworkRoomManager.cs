using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}