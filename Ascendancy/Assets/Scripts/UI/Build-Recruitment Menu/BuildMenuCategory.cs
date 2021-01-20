using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuCategory : MenuCategory
{
    [SerializeField]
    protected GameObject optionPrefab;
    /// <summary>
    /// To keep track of all current Options.
    /// </summary>
    protected List<BuildMenuOption> buildOptions;

    protected override void Start()
    {
        base.Start();

        // Initialization.
        //optionsGO = GetComponentInChildren<VerticalLayoutGroup>().gameObject;
        //optionPrefab = Resources.Load<GameObject>("Prefabs/UI/Build Option");

        // Get all existing options and save them as a list.
        buildOptions = new List<BuildMenuOption>(GetComponentsInChildren<BuildMenuOption>());

        if (category.icon != null)
            GetComponent<Image>().sprite = category.icon;
    }

    public void AddBuildOption(EntityInfo entity)
    {
        //Debug.Log("New Option: " + entity.name);
        GameObject newOptionGO = Instantiate(optionPrefab, optionsGO.transform);
        newOptionGO.name = entity.name;

        BuildMenuOption newOption = newOptionGO.GetComponent<BuildMenuOption>();
        newOption.building = entity;
        //buildOptions.Add(newOption);
    }

}
