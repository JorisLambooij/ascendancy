using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecruitmentFeature", menuName = "Building Features/Recruitment Feature", order = 3)]
public class RecruitmentFeature : BuildingFeature
{
    public List<UnitInfo> recruitableUnits;
    private List<UnitInfo> queue = new List<UnitInfo>();

    private float timer = 0f;
    private int maxQueueSize = 10;


    public override void Initialize(Building building)
    {

    }

    public override void UpdateOverride(Building building)
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AddToQueue(recruitableUnits[0], building);
        }

        if (queue.Count > 0)
            if (timer <= 0f)
            {
                Recruit(queue[0], building);
                queue.RemoveAt(0);

                if (queue.Count > 0)
                {
                    timer = queue[0].build_time;
                }
                else
                {
                    timer = 0f;
                }
            }
            else
            {
                timer -= Time.deltaTime;
                Debug.Log("TIMER: " + timer + "s / QUEUE: " + queue.Count + " $" + queue[0].unitName);
            }

        
    }

    /// <summary>
    /// Recruits a specific Unit.
    /// </summary>
    /// <param name="unitInfo">The Unit we wish to spawn.</param>
    /// <returns>True on a success, false otherwise.</returns>
    public bool AddToQueue(UnitInfo unitInfo, Building building)
    {
        Debug.Log("Add to queue: " + unitInfo.unitName);
        // if unit is not allowed, abort
        if (!recruitableUnits.Contains(unitInfo))
            return false;

        Debug.Log("Recruitable: YES");

        if (queue.Count >= maxQueueSize)
        {
            Debug.Log("Queue is already full!");
            return false;
        }

        //check resource amount
        List<Resource_Amount> unit_cost = unitInfo.resource_amount;

        bool enough = true;
        foreach (Resource_Amount amount in unit_cost)
            if (building.Owner.economy.resourceStorage[amount.resource] < amount.amount)
                enough = false;

        if (enough == true)
        {
            foreach (Resource_Amount amount in unit_cost)
                building.Owner.economy.resourceStorage[amount.resource] -= amount.amount;

            if (queue.Count == 0)
                timer = unitInfo.build_time;

            queue.Add(unitInfo);

            Debug.Log("Successfully added " + unitInfo.unitName + " to the queue!");

            return true;
        }
        else
        {
            Debug.Log("Not enough resources for " + unitInfo.unitName + "!");
            //alert the player
            //TODO

            return false;
        }
    }

    private void Recruit(UnitInfo unitInfo, Building building)
    {
        Transform parent = building.Owner.unitsGO.transform;
        Unit newUnit = Instantiate(unitInfo.prefab, parent).GetComponent<Unit>();
        newUnit.transform.position = building.transform.position;

        Debug.Log("Recruitment Success");
    }
}
