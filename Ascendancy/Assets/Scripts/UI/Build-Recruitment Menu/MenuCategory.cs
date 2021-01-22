using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCategory : MonoBehaviour
{
    /// <summary>
    /// If this Category is expanded or not.
    /// </summary>
    protected bool expanded;

    /// <summary>
    /// The parent GO of the Options
    /// </summary>
    [SerializeField]
    protected GameObject optionsGO;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Expanded = true;
    }
    
    // Toggle Expansion status.
    public void ToggleExpand()
    {
        Expanded = !expanded;
    }

    // To directly set expanded or collapsed.
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
