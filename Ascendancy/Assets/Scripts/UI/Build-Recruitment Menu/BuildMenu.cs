using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour, ListSubscriber<EntityInfo>
{
    /// <summary>
    /// For which player is this menu?
    /// </summary>
    protected Player player;

    [SerializeField]
    protected GameObject categoryPrefab;
    
    protected Dictionary<string, BuildMenuCategory> categories;

    void Start()
    {
        player = GameManager.Instance.playerScript;
        //categoryPrefab = Resources.Load<GameObject>("Prefabs/UI/Build Category");

        // Generate all existing categories and subscribe for future changes.
        GenerateCategories();
        player.TechLevel.entitiesUnlocked.Subscribe(this);
        
        foreach (BuildMenuCategory category in categories.Values)
            category.Expanded = false;
    }
    
    private void GenerateCategories()
    {
        categories = new Dictionary<string, BuildMenuCategory>();
        foreach (EntityInfo info in player.TechLevel.entitiesUnlocked.AsList)
        {
            AddNewBuildingOption(info);
        }
    }

    private void AddNewBuildingOption(EntityInfo info)
    {
        if (categories.ContainsKey(info.Category))
            categories[info.Category].AddBuildOption(info);
        else
            CreateNewCategory(info.Category, info);
    }

    private void CreateNewCategory(string name, EntityInfo firstEntry = null)
    {
        GameObject newCat = Instantiate(categoryPrefab, this.transform);
        newCat.name = "Build Category - " + name;

        if (firstEntry != null)
            newCat.GetComponent<BuildMenuCategory>().AddBuildOption(firstEntry);
    }

    public void NewElementCallback(EntityInfo updatedValue)
    {
        Debug.Log("New Building: " + updatedValue.name);
        AddNewBuildingOption(updatedValue);
    }

    public void NewListCallback(List<EntityInfo> newList)
    {
        throw new System.NotImplementedException();
    }

}
