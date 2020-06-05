using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackOrder : AttackOrder
{
    /// <summary>
    /// The Feature that allows this Entity to perform Ranged Attacks
    /// </summary>
    protected RangedAttackFeature rangedFeature;

    protected bool isFiring;

    public RangedAttackOrder(Entity unit, Entity target, bool guardMode = false) : base(unit, target, guardMode)
    {
        Debug.Log("Ranged Order: " + target);
        this.rangedFeature = unit.FindFeature<RangedAttackFeature>();
        isFiring = false;
    }

    public override void Update()
    {
        base.Update();
        
        Vector3 targetPos = target.transform.position;


        float distance = Vector3.Distance(entity.transform.position, target.transform.position);
        // If target is in range, proceed
        if (distance < rangedFeature.maxRange)
        {
            // If the target is outside of the minimum range, make an attack
            if (distance > rangedFeature.minRange)
            {
                if (moveFeature != null)
                    moveFeature.entity.Controller.NavAgent.isStopped = true;

                if (cooldown <= 0)
                {
                    Attack();
                    cooldown = rangedFeature.rangedAttackCooldown;
                }
            }
            // Target too close, do nothing
            // TODO: Maybe Unit should move away automatically?
            else
            {

            }

        }
        // Not in range, but if this entity is not on guard mode (and can actually move), move toward target until it is in range
        else if (!guardMode && moveFeature != null)
        {
            moveFeature.entity.Controller.NavAgent.SetDestination(targetPos);
            moveFeature.entity.Controller.NavAgent.isStopped = false;
        }
    }


    /// <summary>
    /// Carry out one ranged attack.
    /// </summary>
    protected void Attack()
    {
        // make the attack with an amount of projectiles equal to the 'volley' parameter
        if (!isFiring)
            entity.StartCoroutine(FireVolley());
    }

    IEnumerator FireVolley()
    {
        isFiring = true;
        // Stagger the projectiles slightly, so they do not come out all at once
        for (int i = 0; i < rangedFeature.volley; i++)
        {
            GameObject projectilePrefab = Resources.Load("Prefabs/Entities/Projectile") as GameObject;
            GameObject projectile = GameObject.Instantiate(projectilePrefab);
            projectile.transform.position = entity.transform.position;
            projectile.GetComponent<Projectile>().Launch(rangedFeature, target.transform);

            yield return new WaitForSeconds(rangedFeature.volleyDuration / rangedFeature.volley);
        }
        isFiring = false;
    }

    public override void Cancel()
    {
        base.Cancel();
        entity.StopCoroutine(FireVolley());
        isFiring = false;
    }

    /// <summary>
    /// Is the target in range?
    /// </summary>
    protected override bool IsInRange
    {
        get
        {
            float distance = Vector3.Distance(entity.transform.position, target.transform.position);
            return distance < rangedFeature.maxRange;
        }
    }

}
