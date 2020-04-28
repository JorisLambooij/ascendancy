using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GOPool))]
public class RecruitmentMenu : MonoBehaviour, ListSubscriber<EntitySelector>
{
    protected GOPool pool;

    List<RecruitmentMenuCategory> categories;

    void Awake()
    {
        pool = GetComponent<GOPool>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Find the Gama Manager and subscribe to the list of selected Entities.
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
        //throw new System.NotImplementedException("Subscriber Method for when new Entities are selected not yet implemented");
    }

    /// <summary>
    /// When new Entities are selected, construct the Recruitment Menu accordingly
    /// </summary>
    /// <param name="newList"></param>
    public void NewListCallback(List<EntitySelector> newList)
    {
        // Filter for Entities that can recruit.
        List<EntitySelector> recrList = newList.Where(e => e.ParentEntity.FindFeature<RecruitmentFeature>() != null).ToList();

        // Open the Recruitment Menu when there are recruitments available
        if (recrList.Count > 0)
            GameManager.Instance.ui_Manager.OpenScreen("Recruitment Menu", false);
        // Close it otherwise
        else
            GameManager.Instance.ui_Manager.SetScreen("Recruitment Menu", false);

        // Fill each Category with the Recruitment Options of the recruiting Entity.
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
