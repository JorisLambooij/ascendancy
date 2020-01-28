using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    void Start()
    {
        MenuCategory[] menuCategories = transform.GetComponentsInChildren<MenuCategory>();

        Debug.Log(transform.parent.name + " " + menuCategories.Length);
        foreach (MenuCategory category in menuCategories)
            category.Expanded = false;

        if (menuCategories.Length == 1)
            menuCategories[0].Expanded = true;
    }
}
