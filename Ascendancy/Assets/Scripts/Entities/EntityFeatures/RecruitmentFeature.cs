using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecruitmentFeature", menuName = "Entity Features/Recruitment Feature")]
public class RecruitmentFeature : EntityFeature
{
    public List<EntityInfo> recruitableUnits;

    private List<QueueObject> queue = new List<QueueObject>();
    private float timer = 0f;
    private readonly int maxQueueSize = 10;
    
    public override void Initialize(Entity entity)
    {

    }

    public override void UpdateOverride(Entity entity)
    {
        if (Input.GetKeyDown(KeyCode.T))
            AddToQueue(recruitableUnits[0].Prefab.GetComponent<Unit>(), entity);

        if (queue.Count > 0)
            if (timer <= 0f)
            {
                Recruit(queue[0].BaseUnit, entity);
                queue.RemoveAt(0);

                if (queue.Count > 0)
                    timer = queue[0].BaseUnit.entityInfo.BuildTime;
                else
                    timer = 0f;
            }
            else
            {
                timer -= Time.deltaTime;
                Debug.Log("TIMER: " + timer + "s / QUEUE: " + queue.Count + " $" + queue[0].BaseUnit.entityInfo.name);
            }
    }

    /// <summary>
    /// Recruits a specific Unit.
    /// </summary>
    /// <param name="unitInfo">The Unit we wish to spawn.</param>
    /// <returns>True on a success, false otherwise.</returns>
    public bool AddToQueue(Unit unit, Entity entity)
    {
        Debug.Log("Add to queue: " + unit.entityInfo.name);
        // if unit is not allowed, abort
        if (!recruitableUnits.Contains(unit.entityInfo))
            return false;

        Debug.Log("Recruitable: YES");

        if (queue.Count >= maxQueueSize)
        {
            Debug.Log("Queue is already full!");
            return false;
        }

        //check resource amount
        List<Resource_Amount> unitRecruitmentCosts = unit.entityInfo.ResourceAmount;

        bool enough = true;
        foreach (Resource_Amount amount in unitRecruitmentCosts)
            if (entity.Owner.economy.resourceStorage.GetValue(amount.resource) < amount.amount)
                enough = false;

        if (enough == true)
        {
            foreach (Resource_Amount amount in unitRecruitmentCosts)
            {
                float newAmount = entity.Owner.economy.resourceStorage.GetValue(amount.resource) - amount.amount; ;
                entity.Owner.economy.resourceStorage.SetValue(amount.resource, newAmount);
            }

            if (queue.Count == 0)
                timer = unit.entityInfo.BuildTime;

            queue.Add(new QueueObject(unit, unit.entityInfo.ResourceAmount));

            Debug.Log("Successfully added " + unit.entityInfo.name + " to the queue!");

            return true;
        }
        else
        {
            Debug.Log("Not enough resources for " + unit.entityInfo.name + "!");
            //alert the player
            //TODO

            return false;
        }
    }

    private void Recruit(Unit unit, Entity entity)
    {
        Transform parent = entity.Owner.unitsGO.transform;
        Unit newUnit = Instantiate(unit.entityInfo.Prefab, parent).GetComponent<Unit>();
        newUnit.transform.position = entity.transform.position;

        Debug.Log("Recruitment Success");
    }
    
}
