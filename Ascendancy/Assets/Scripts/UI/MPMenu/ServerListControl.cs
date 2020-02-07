using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerListControl : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonTemplate;

    List<GameObject> items;
    List<ServerInfo> serverList;

    private int maxServerNameLength = 20;

    void Start()
    {
        items = new List<GameObject>();
        serverList = new List<ServerInfo>();

        GenerateDummyEntries();
        RefreshItems();
    }

    public void Update()
    {
        //TODO Refresh Items every X seconds (10? 20?)
    }

    public void GenerateDummyEntries()
    {
        for (int i = 0; i < 20; i++)
        {
            ServerInfo servInf = new ServerInfo
            {
                ServerName = "SERVER " + i,
                Latency = i*5,
                Password = (i%3 == 0),
                PlayerCountCurrent = i,
                PlayerCountMax = 20,
                IPAddress = "127.0.0.1"
            };
            serverList.Add(servInf);
        }
    }

    public void RefreshItems()
    {
        foreach (GameObject b in items)
        {
            Destroy(b);
        }

        string itemText = "";
        foreach (ServerInfo serverData in serverList)
        {
            GameObject button = Instantiate(buttonTemplate) as GameObject;
            button.SetActive(true);

            itemText += serverData.ServerName.PadRight(maxServerNameLength).Substring(0, maxServerNameLength);

            itemText += "\t"; //tabstop

            itemText += "          ";   //10 blank spaces
            itemText += ("Connection: " + serverData.Latency + "%").PadRight(20);

            itemText += "\t"; //tabstop

            if (serverData.Password)
                itemText += "password".PadRight(20);
            else
                itemText += "no password".PadRight(20);

            itemText += "\n";   //newline

            itemText += "Players: " + serverData.PlayerCountCurrent + "/" + serverData.PlayerCountMax;


            button.GetComponent<ServerListItem>().SetText(itemText);

            button.transform.SetParent(buttonTemplate.transform.parent, false);

            items.Add(button);
            itemText = "";
        }
    }
}
