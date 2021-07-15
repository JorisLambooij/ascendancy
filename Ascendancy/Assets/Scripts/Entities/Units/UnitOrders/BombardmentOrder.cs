using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombardmentOrder : UnitOrder
{
    protected Vector3 targetPosition;
    protected RangedAttackFeature rangedFeature;
    protected bool isFiring;
    protected MovementFeature moveFeature;

    public BombardmentOrder(Entity unit, Vector3 target) : base (unit)
    {
        //Debug.Log("Ranged Order: " + target);
        this.rangedFeature = unit.FindFeature<RangedAttackFeature>();
        moveFeature = unit.FindFeature<MovementFeature>();
        this.targetPosition = target;
        isFiring = false;
    }

    public override Vector3 CurrentDestination => targetPosition;

    public override bool Fulfilled => false;

    public override void Update()
    {
        base.Update();

        float distance = Vector3.Distance(entity.transform.position, targetPosition);
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
        else if (moveFeature != null)
        {
            moveFeature.entity.Controller.NavAgent.SetDestination(targetPosition);
            moveFeature.entity.Controller.NavAgent.isStopped = false;
        }
    }
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
            //GameObject projectilePrefab = Resources.Load("Prefabs/Entities/Standard Projectile") as GameObject;
            GameObject projectile = GameObject.Instantiate(rangedFeature.projectileInfo.projectilePrefab);
            projectile.GetComponent<Projectile>().Launch(rangedFeature, targetPosition);

            yield return new WaitForSeconds(rangedFeature.volleyDuration / rangedFeature.volley);
        }
        isFiring = false;
    }

}
