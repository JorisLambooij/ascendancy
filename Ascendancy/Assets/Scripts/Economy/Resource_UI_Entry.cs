using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource_UI_Entry : MonoBehaviour
{
    [SerializeField]
    private Text amountText;
    [SerializeField]
    private Text productionText;

    public Sprite Sprite
    {
        get { return GetComponentInChildren<Image>().sprite; }
        set { GetComponentInChildren<Image>().sprite = value; }
    }

    private float count;
    public float Count
    {
        get { return count; }
        set
        {
            count = value;
            amountText.text = AsString(count);
        }
    }

    private float production;
    public float Production
    {
        get { return production; }
        set
        {
            production = value;
            productionText.text = AsString(production, true);
        }
    }

    private string AsString(float f, bool positivePrefix = false)
    {
        string prefix = "";
        if (f > 0 && positivePrefix)
            prefix = "+";
        if (f < 0)
            prefix = "-";
        f = Mathf.Abs(f);
        if (f < 1000)
            return prefix + (Mathf.Round(f)).ToString(); // 12.5 -> 12
        else if (f < 1000000)
            return prefix + (0.1f * Mathf.Round(f * 0.01f)).ToString() + "K"; // 2100 -> 2.1K
        else
            return prefix + (0.1f * Mathf.Round(f * 0.00001f)).ToString() + "M"; // 1200300 -> 1.2M
    }
}
