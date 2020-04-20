using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerListItem : MonoBehaviour
{
    [SerializeField]
    private Text myText;

    public ServerListControl serverListControl;

    public long serverId;

    public void SetText(string itemText)
    {
        myText.text = itemText;
    }

    public void OnClick()
    {
        Debug.Log("Selected \"" + myText.text.Substring(0, 20).Trim() + "\"");

        if (serverId != 0)
            serverListControl.SetSelectedServer(serverId);
        else
            Debug.LogError("Selected Server is missing an ID!");
    }
}
