using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackOrder : UnitOrder
{
    /// <summary>
    /// The target of the attack Order.
    /// </summary>
    private Entity target;

    /// <summary>
    /// Guard Mode means the unit will not follow the target when it gets out range.
    /// </summary>
    private bool guardMode;

    public MeleeAttackOrder(Unit unit, Entity target, bool guardMode = false) : base(unit)
    {
        this.target = target;
        this.guardMode = guardMode;
    }

    public override Vector3 CurrentDestination
    {
        get { return target.transform.position; }
    }

    public void SetTarget(Entity e)
    {
        target = e;
    }
    
    public override void Update()
    {
        if (cooldown > 0)
            cooldown -= Time.deltaTime;

        Vector3 targetPos = target.transform.position;

        if (IsInRange)
        {
            unit.Controller.NavAgent.isStopped = true;

            if (cooldown <= 0)
            {
                Attack();
                cooldown = unit.unitInfo.attackSpeed;
            }
        }
        else if (!guardMode)
        {
            unit.Controller.NavAgent.SetDestination(targetPos);
            unit.Controller.NavAgent.isStopped = false;
        }
    }

    /// <summary>
    /// Whether or not the target is in melee range.
    /// </summary>
    private bool IsInRange
    {
        get { return Vector3.Distance(unit.transform.position, target.transform.position) < unit.unitInfo.meleeRange; }
    }

    /// <summary>
    /// Carry out one attempt at a melee attack.
    /// </summary>
    private void Attack()
    {
        if (target is Unit)
        {
            // melee duel
            (target as Unit).Controller.EnterMelee(unit);

            int unitAttack = unit.unitInfo.meleeAttack;
            int targetDefense = (target as Unit).unitInfo.meleeDefense;

            int chanceToHit = Mathf.Clamp(50 + unitAttack - targetDefense, 10, 90);

            Debug.Log("rolling to hit");
            if (Random.Range(0, 100) < chanceToHit)
            {
                // successful attack
                Debug.Log("attack success: " + unit.unitInfo.meleeStrength + " dmg");
                target.TakeDamage(unit.unitInfo.meleeStrength);
            }
        }
        else if (target is Building)
        {
            // buildings cant defend themselves, so auto-hit
            target.TakeDamage(unit.unitInfo.meleeStrength);
            cooldown = unit.unitInfo.attackSpeed;
        }

    }

    public override bool Fulfilled
    {
        get
        {
            float targetHealth = target.Health;
            return targetHealth <= 0 || (guardMode && !IsInRange);
        }
    }

    public bool GuardMode
    {
        get { return guardMode; }
        set { guardMode = value; }
    }
}
