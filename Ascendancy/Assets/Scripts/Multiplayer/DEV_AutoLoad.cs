using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DEV_AutoLoad : MonoBehaviour
{
    [SerializeField]
    private MPMenu_NetworkRoomManager networkManager;

    [SerializeField]
    private MPMenu_NetworkDiscovery networkDiscovery;

    private bool destroyThis;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        networkManager.StartHost();
        networkDiscovery.AdvertiseServer();

        destroyThis = false;
    }

    void ssUpdate()
    {
        if (destroyThis)
            return;

        MP_Lobby lobby = null;
        GameObject playerManager = GameObject.Find("PlayerManager");
        if (playerManager != null)
        {
            lobby = playerManager.GetComponent<MP_Lobby>();
            if (lobby != null)
            {
                lobby.ButtonReadyStartClick();
                Destroy(this.gameObject);
                destroyThis = true;
            }
        }    
    }
}
