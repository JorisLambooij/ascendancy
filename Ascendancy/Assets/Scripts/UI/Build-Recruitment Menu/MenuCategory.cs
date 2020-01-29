using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCategory : MonoBehaviour
{
    protected bool expanded;
    protected GameObject optionsGO;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        optionsGO = transform.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        Expanded = false;
    }

    public void Expand()
    {
        Expanded = !expanded;
    }

    public bool Expanded
    {
        get { return expanded; }
        set
        {
            expanded = value;

            if (optionsGO == null)
                optionsGO = transform.GetComponentInChildren<VerticalLayoutGroup>().gameObject;

            optionsGO.SetActive(value);
        }
    }
}
