using Mirror;
using Mirror.Discovery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerListControl : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonTemplate;
    [SerializeField]
    private bool generateDummyEntries;
    [SerializeField]
    private NetworkManager networkManager;

    List<GameObject> items;
    Dictionary<long, MPMenu_ServerResponse> serverList;

    private long selectedServerId;

    private int maxServerNameLength = 20;

    void Start()
    {
        items = new List<GameObject>();
        serverList = new Dictionary<long, MPMenu_ServerResponse>();

        if (generateDummyEntries)
            GenerateDummyEntries();
        

        RefreshItems();
    }

    public void Update()
    {
    }

    public static long RandomLong()
    {
        int value1 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        int value2 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        return value1 + ((long)value2 << 32);
    }

    public void GenerateDummyEntries()
    {
        for (int i = 0; i < 20; i++)
        {
            MPMenu_ServerResponse servInf = new MPMenu_ServerResponse
            {
                nameServer = "SERVER " + i,
                nameHost = "HOSTNAME",
                ping = i*5,                                     //TODO: get real ping
                passwordProtection = false,
                playerCount = i,
                playerMax = 20,
                uri = new System.Uri("http://www.test.com/"),   //TODO: get real Uri
                serverId = RandomLong()
        };
            serverList.Add(servInf.serverId, servInf);
        }
    }

    public void AddEntry(MPMenu_ServerResponse responseData)
    {
        if (!serverList.ContainsKey(responseData.serverId))
            serverList.Add(responseData.serverId, responseData);

        RefreshItems();
    }

    public void ClearList()
    {
        serverList.Clear();
    }

    public void RefreshItems()
    {
        // do not refresh if we are already in the next scene
        if (NetworkServer.active || NetworkClient.active)
            return;

        foreach (GameObject b in items)
        {
            Destroy(b);
        }

        string itemText = "";
        foreach (MPMenu_ServerResponse serverData in serverList.Values)
        {
            GameObject button = Instantiate(buttonTemplate) as GameObject;
            button.SetActive(true);

            itemText += serverData.nameServer.PadRight(maxServerNameLength).Substring(0, maxServerNameLength);

            itemText += "\t"; //tabstop

            itemText += "          ";   //10 blank spaces
            itemText += ("Connection: " + serverData.ping + "%").PadRight(20);

            itemText += "\t"; //tabstop

            if (serverData.passwordProtection)
                itemText += "password".PadRight(20);
            else
                itemText += "no password".PadRight(20);

            itemText += "\n";   //newline

            itemText += "Players: " + serverData.playerCount + "/" + serverData.playerMax;

            button.GetComponent<ServerListItem>().serverListControl = this;
            button.GetComponent<ServerListItem>().SetText(itemText);
            button.GetComponent<ServerListItem>().serverId = serverData.serverId;

            //(re)select button if it has the selectedServer ID
            if (serverData.serverId == selectedServerId)
                button.GetComponent<Button>().Select();

            button.transform.SetParent(buttonTemplate.transform.parent, false);

            items.Add(button);
            itemText = "";
        }
    }

    public void SetSelectedServer(long servId)
    {
        selectedServerId = servId;
    }

        public void StartSelectedServer()
    {
        try
        {
            NetworkManager.singleton.StartClient(serverList[selectedServerId].uri);
            GameObject.Find("NetworkManager").GetComponent<MPMenu_NetworkDiscovery>().StopDiscovery();
        }
        catch (KeyNotFoundException e)
        {
            Debug.LogError("No Server was selected!");
        }
    }
}
