using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuidingMenuCategory : MonoBehaviour
{
    protected bool expanded;
    protected GameObject optionsGO;

    // Start is called before the first frame update
    void Start()
    {
        optionsGO = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
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
            optionsGO.SetActive(value);
        }
    }
}
