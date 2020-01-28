using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GOPool))]
public class RecruitmentMenu : MonoBehaviour, ListSubscriber<EntitySelector>
{
    protected GOPool pool;

    List<RecruitmentMenuCategory> categories;

    // Start is called before the first frame update
    void Start()
    {
        pool = GetComponent<GOPool>();

        GameManager gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        (gameManager.controlModeDict[ControlModeEnum.gameMode] as GameMode).selectedUnits.Subscribe(this);
        categories = new List<RecruitmentMenuCategory>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NewElementCallback(EntitySelector updatedValue)
    {
        //Debug.Log("New Element");
        //RecruitmentMenuCategory newCategory = Instantiate(categoryPrefab, transform).GetComponent<RecruitmentMenuCategory>();
        //categories.Add(newCategory);
    }

    public void NewListCallback(List<EntitySelector> newList)
    {
        List<EntitySelector> recrList = newList.Where(e => e.ParentEntity.FindFeature<RecruitmentFeature>() != null).ToList();

        foreach (RecruitmentMenuCategory cat in categories)
            cat.Expanded = false;

        pool.Generate(recrList.Count);
        int i = 0;
        foreach(EntitySelector es in recrList)
        {
            RecruitmentMenuCategory cat = pool.pool[i].GetComponent<RecruitmentMenuCategory>();
            cat.Expanded = true;
            cat.SelectRecruiter(es.ParentEntity);
            i++;
        }
    }
}
