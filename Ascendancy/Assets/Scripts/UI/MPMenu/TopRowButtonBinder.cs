using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopRowButtonBinder : MonoBehaviour
{
    public ServerListControl controlList;

    MPMenu_NetworkRoomManager netMngr;
    MPMenu_NetworkDiscovery netDisc;

    void Start()
    {
        if(netMngr == null)
        {
            GameObject go = GameObject.Find("NetworkManager");
            netMngr = go.GetComponent<MPMenu_NetworkRoomManager>();
            netDisc = go.GetComponent<MPMenu_NetworkDiscovery>();
            netDisc.control = controlList;
        }
    }

    public void CreateClick()
    {
        netMngr.StartHost();
        netDisc.AdvertiseServer();
    }

    public void RefreshClick()
    {
        netDisc.StartDiscovery();
    }
}
