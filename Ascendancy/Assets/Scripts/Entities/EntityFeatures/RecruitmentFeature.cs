using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecruitmentFeature", menuName = "Entity Features/Recruitment Feature", order = 3)]
public class RecruitmentFeature : EntityFeature
{
    public List<UnitInfo> recruitableUnits;
    private List<QueueObject> queue = new List<QueueObject>();

    private float timer = 0f;
    private readonly int maxQueueSize = 10;


    public override void Initialize(Entity entity)
    {

    }

    public override void UpdateOverride(Entity entity)
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AddToQueue(recruitableUnits[0].prefab.GetComponent<Unit>(), entity);
        }

        if (queue.Count > 0)
            if (timer <= 0f)
            {
                Recruit(queue[0].BaseUnit, entity);
                queue.RemoveAt(0);

                if (queue.Count > 0)
                {
                    timer = queue[0].BaseUnit.unitInfo.build_time;
                }
                else
                {
                    timer = 0f;
                }
            }
            else
            {
                timer -= Time.deltaTime;
                Debug.Log("TIMER: " + timer + "s / QUEUE: " + queue.Count + " $" + queue[0].BaseUnit.unitInfo.unitName);
            }


    }

    /// <summary>
    /// Recruits a specific Unit.
    /// </summary>
    /// <param name="unitInfo">The Unit we wish to spawn.</param>
    /// <returns>True on a success, false otherwise.</returns>
    public bool AddToQueue(Unit unit, Entity entity)
    {
        Debug.Log("Add to queue: " + unit.unitInfo.unitName);
        // if unit is not allowed, abort
        if (!recruitableUnits.Contains(unit.unitInfo))
            return false;

        Debug.Log("Recruitable: YES");

        if (queue.Count >= maxQueueSize)
        {
            Debug.Log("Queue is already full!");
            return false;
        }

        //check resource amount
        List<Resource_Amount> unit_cost = unit.unitInfo.resource_amount;

        bool enough = true;
        foreach (Resource_Amount amount in unit_cost)
            if (entity.Owner.economy.resourceStorage[amount.resource] < amount.amount)
                enough = false;

        if (enough == true)
        {
            foreach (Resource_Amount amount in unit_cost)
                entity.Owner.economy.resourceStorage[amount.resource] -= amount.amount;

            if (queue.Count == 0)
                timer = unit.unitInfo.build_time;

            queue.Add(new QueueObject(unit, unit.unitInfo.resource_amount));

            Debug.Log("Successfully added " + unit.unitInfo.unitName + " to the queue!");

            return true;
        }
        else
        {
            Debug.Log("Not enough resources for " + unit.unitInfo.unitName + "!");
            //alert the player
            //TODO

            return false;
        }
    }

    private void Recruit(Unit unit, Entity entity)
    {
        Transform parent = entity.Owner.unitsGO.transform;
        Unit newUnit = Instantiate(unit.unitInfo.prefab, parent).GetComponent<Unit>();
        newUnit.transform.position = entity.transform.position;

        Debug.Log("Recruitment Success");
    }
}
