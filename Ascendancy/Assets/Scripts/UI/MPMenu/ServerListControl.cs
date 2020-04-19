using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerListControl : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonTemplate;
    [SerializeField]
    private bool generateDummyEntries; 

    List<GameObject> items;
    List<MPMenu_ServerResponse> serverList;

    private int maxServerNameLength = 20;

    void Start()
    {
        items = new List<GameObject>();
        serverList = new List<MPMenu_ServerResponse>();

        if (generateDummyEntries)
            GenerateDummyEntries();
        

        RefreshItems();
    }

    public void Update()
    {
        //TODO Refresh Items every X seconds (10? 20?)
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
                ping = i*5,
                passwordProtection = false,
                playerCount = i,
                playerMax = 20,
                uri = new System.Uri("http://www.test.com/"),
                serverId = RandomLong()
        };
            serverList.Add(servInf);
        }
    }

    public void AddEntry(MPMenu_ServerResponse responseData)
    {
        serverList.Add(responseData);
    }

    public void RefreshItems()
    {
        foreach (GameObject b in items)
        {
            Destroy(b);
        }

        string itemText = "";
        foreach (MPMenu_ServerResponse serverData in serverList)
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


            button.GetComponent<ServerListItem>().SetText(itemText);

            button.transform.SetParent(buttonTemplate.transform.parent, false);

            items.Add(button);
            itemText = "";
        }
    }
}
