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
    /// The Combat-EntityFeature that enables this Entity to fight.
    /// </summary>
    private MeleeFeature combatFeature;

    /// <summary>
    /// The Movement-EntityFeature that enables this Entity to move. If not found, Entity still will be able to defend its melee range.
    /// </summary>
    private MovementFeature moveFeature;

    /// <summary>
    /// Guard Mode means the unit will not follow the target when it gets out range.
    /// </summary>
    private bool guardMode;

    public MeleeAttackOrder(Entity entity, Entity target, bool guardMode = false) : base(entity)
    {
        this.target = target;
        this.guardMode = guardMode;
        this.combatFeature = entity.FindFeature<MeleeFeature>();
        moveFeature = entity.FindFeature<MovementFeature>();

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
            if (moveFeature != null)
                moveFeature.entity.Controller.NavAgent.isStopped = true;

            if (cooldown <= 0)
            {
                Attack();
                cooldown = combatFeature.attackSpeed;
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
    private bool IsInRange
    {
        get { return Vector3.Distance(entity.transform.position, target.transform.position) < combatFeature.meleeRange; }
    }

    /// <summary>
    /// Carry out one attempt at a melee attack.
    /// </summary>
    private void Attack()
    {
        MeleeFeature targetCombatFeature = target.FindFeature<MeleeFeature>();
        if (targetCombatFeature != null)
        {
            // Target has combat capabilities, so enter a melee duel
            (target as Unit).Controller.EnterMelee(entity);

            int unitAttack = combatFeature.meleeAttack;
            int targetDefense = targetCombatFeature.meleeDefense;

            int chanceToHit = Mathf.Clamp(50 + unitAttack - targetDefense, 10, 90);
            
            if (Random.Range(0, 100) < chanceToHit)
            {
                // Successful attack
                target.TakeDamage(combatFeature.meleeStrength);
            }
        }
        else
        {
            // Target entitiy can't defend itself, so auto-hit.
            target.TakeDamage(combatFeature.meleeStrength);
            cooldown = combatFeature.attackSpeed;
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
