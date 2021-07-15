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
    
    protected Dictionary<EntityCategoryInfo, BuildMenuCategory> categories;

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
        categories = new Dictionary<EntityCategoryInfo, BuildMenuCategory>();
        foreach (EntityInfo info in player.TechLevel.entitiesUnlocked.AsList)
        {
            AddNewBuildingOption(info);
        }
    }

    private void AddNewBuildingOption(EntityInfo info)
    {
        //Debug.Log("New Building " + info.name + " in " + info.category.name);
        if (categories.ContainsKey(info.category))
            categories[info.category].AddBuildOption(info);
        else
            CreateNewCategory(info.category, info);
    }

    private void CreateNewCategory(EntityCategoryInfo categoryInfo, EntityInfo firstEntry = null)
    {
        BuildMenuCategory newCat = Instantiate(categoryPrefab, this.transform).GetComponent<BuildMenuCategory>();
        newCat.category = categoryInfo;
        newCat.gameObject.name = "Build Category - " + categoryInfo.name;
        categories.Add(categoryInfo, newCat);

        if (firstEntry != null)
            newCat.GetComponent<BuildMenuCategory>().AddBuildOption(firstEntry);
    }

    public void NewElementCallback(EntityInfo updatedValue)
    {
        //Debug.Log("New Building: " + updatedValue);
        AddNewBuildingOption(updatedValue);
    }

    public void NewListCallback(List<EntityInfo> newList)
    {
        throw new System.NotImplementedException();
    }

}
