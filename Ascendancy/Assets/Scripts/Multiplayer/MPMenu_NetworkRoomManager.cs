using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror
{
    public class MPMenu_NetworkRoomManager : NetworkRoomManager
    {
        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);
            if (sceneName == GameplayScene)
                GameObject.Find("PlayerManager").GetComponent<MP_Lobby>().InitializePlayers();
        }
    }
}