using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource_UI_Entry : MonoBehaviour
{
    public Sprite Sprite
    {
        get { return this.GetComponentInChildren<Image>().sprite; }
        set { this.GetComponentInChildren<Image>().sprite = value; }
    }

    private float count;
    public float Count
    {
        get { return count; }
        set
        {
            count = value;
            this.GetComponentInChildren<Text>().text = CountAsString();
        }
    }

    private string CountAsString()
    {
        if (count < 1000)
            return (Mathf.Round(count)).ToString(); // 12.5 -> 12
        else if (count < 1000000)
            return (0.1f * Mathf.Round(count * 0.01f)).ToString() + "K"; // 2100 -> 2.1K
        else
            return (0.1f * Mathf.Round(count * 0.00001f)).ToString() + "M"; // 1200300 -> 1.2M
    }
}
