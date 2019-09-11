using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuidingMenuCategory : MonoBehaviour
{
    private bool expanded;
    public GameObject options;

    // Start is called before the first frame update
    void Start()
    {
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
            options.SetActive(value);
        }
    }
}
