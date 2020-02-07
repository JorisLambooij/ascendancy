using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerListItem : MonoBehaviour
{
    [SerializeField]
    private Text myText;

    public void SetText(string itemText)
    {
        myText.text = itemText;
    }

    public void OnClick()
    {
        Debug.Log("Selected \"" + myText.text.Substring(0, 20).Trim() + "\"");
    }

    void Start()
    {
        
    }
}
