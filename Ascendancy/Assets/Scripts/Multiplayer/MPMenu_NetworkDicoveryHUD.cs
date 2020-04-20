using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Discovery
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/CustomNetworkDiscoveryHUD")]
    [HelpURL("https://mirror-networking.com/docs/Components/NetworkDiscovery.html")]
    [RequireComponent(typeof(MPMenu_NetworkDiscovery))]
    public class MPMenu_NetworkDicoveryHUD : MonoBehaviour
    {
        readonly Dictionary<long, MPMenu_ServerResponse> discoveredServers = new Dictionary<long, MPMenu_ServerResponse>();
        Vector2 scrollViewPos = Vector2.zero;

        public MPMenu_NetworkDiscovery networkDiscovery;
        public MP_ServerList serverList;

#if UNITY_EDITOR
        void OnValidate()
        {
            if (networkDiscovery == null)
            {
                networkDiscovery = GetComponent<MPMenu_NetworkDiscovery>();
                UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
                Debug.Log("Set NetworkDiscovery");
                UnityEditor.Undo.RecordObjects(new Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
            }
        }
#endif

        void Update()
        {
            if (NetworkManager.singleton == null)
                return;

            if (NetworkServer.active || NetworkClient.active)
                return;

            if (!NetworkClient.isConnected && !NetworkServer.active && !NetworkClient.active)
                serverList.CreateList(discoveredServers);
        }
        
        void Connect(ServerResponse info)
        {
            NetworkManager.singleton.StartClient(info.uri);
        }

        public void OnDiscoveredServer(MPMenu_ServerResponse info)
        {
            // Note that you can check the versioning to decide if you can connect to the server or not using this method
            discoveredServers[info.serverId] = info;
        }
    }
}
