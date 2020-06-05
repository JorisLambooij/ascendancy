using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackOrder : AttackOrder
{
    /// <summary>
    /// The Combat-EntityFeature that enables this Entity to fight.
    /// </summary>
    protected MeleeFeature meleeFeature;
    
    public MeleeAttackOrder(Entity entity, Entity target, bool guardMode = false) : base(entity, target, guardMode)
    {
        Debug.Log("Melee Order: Attack " + target);
        this.meleeFeature = entity.FindFeature<MeleeFeature>();
    }
    
    public override void Update()
    {
        base.Update();

        Vector3 targetPos = target.transform.position;

        if (IsInRange)
        {
            if (moveFeature != null)
                moveFeature.entity.Controller.NavAgent.isStopped = true;

            if (cooldown <= 0)
            {
                Attack();
                cooldown = meleeFeature.meleeAttackSpeed;
            }
        }
        else if (!guardMode && moveFeature != null)
        {
            moveFeature.entity.Controller.NavAgent.SetDestination(targetPos);
            moveFeature.entity.Controller.NavAgent.isStopped = false;
        }
    }

    /// <summary>
    /// Whether or not the target is in melee range.
    /// </summary>
    protected override bool IsInRange
    {
        get { return Vector3.Distance(entity.transform.position, target.transform.position) < meleeFeature.meleeRange; }
    }

    /// <summary>
    /// Carry out one attempt at a melee attack.
    /// </summary>
    protected void Attack()
    {
        MeleeFeature targetCombatFeature = target.FindFeature<MeleeFeature>();
        if (targetCombatFeature != null)
        {
            // Target has combat capabilities, so enter a melee duel
            (target as Entity).Controller.EnterMelee(entity);

            int unitAttack = meleeFeature.meleeAttack;
            int targetDefense = targetCombatFeature.meleeDefense;

            int chanceToHit = Mathf.Clamp(50 + unitAttack - targetDefense, 10, 90);
            
            if (Random.Range(0, 100) < chanceToHit)
            {
                // Successful attack
                target.TakeDamage(meleeFeature.meleeStrength);
            }
        }
        else
        {
            // Target entitiy can't defend itself, so auto-hit.
            target.TakeDamage(meleeFeature.meleeStrength);
            cooldown = meleeFeature.meleeAttackSpeed;
        }

    }

    

    public bool GuardMode
    {
        get { return guardMode; }
        set { guardMode = value; }
    }
}
