using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileInfo info;

    private Rigidbody rb;
    private Transform target;
    private float remainingPiercingPower;

    // Each Entity can only be hit once by a projectile. This is to keep track of that.
    private HashSet<Entity> entitiesPierced;

    public Rigidbody RB { get => rb; protected set => rb = value; }

    public void Launch(RangedAttackFeature rangedFeature, Transform target)
    {
        this.info = rangedFeature.projectileInfo;
        this.target = target;
        RB = GetComponent<Rigidbody>();

        Vector3 predictedTargetLocation = PredictedTargetLocation();
        Vector3 distance = predictedTargetLocation - transform.position;
        // Add some inaccuracy
        float relativeRange = Mathf.InverseLerp(rangedFeature.minRange, rangedFeature.maxRange, distance.magnitude);
        float inaccuracyLerped = Mathf.Lerp(0, rangedFeature.inaccuracy, relativeRange);
        predictedTargetLocation += RandomUnitVector() * inaccuracyLerped;

        transform.LookAt(predictedTargetLocation, Vector3.up);
        Vector3 targetDir = (predictedTargetLocation - transform.position).normalized;
        RB.velocity = targetDir * info.launchVelocity;

        // Do not hit the launcher itself
        remainingPiercingPower = info.piercingPower;
        entitiesPierced = new HashSet<Entity>();
        entitiesPierced.Add(rangedFeature.entity);
        Destroy(this.gameObject, info.lifeTime);
    }
    

    void Update()
    {
        if (target != null)
        {
            Vector3 predictedTargetLocation = PredictedTargetLocation();

            // Target-Seeking
            Vector3 targetDir = predictedTargetLocation - transform.position;

            // Only if the projectile could still reasonably hit the target, do we adjust the trajectory
            // This prevents situations where projectiles that have clearly already missed still try to come back.
            if (Vector3.Angle(targetDir, transform.forward) < 60)
            {
                float step = info.targetSeekingCoefficient * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
                Vector3 newVel = Vector3.RotateTowards(RB.velocity, targetDir, step, 0.0F);

                Debug.DrawRay(transform.position, newDir, Color.red);
                transform.rotation = Quaternion.LookRotation(newDir);
                RB.velocity = newVel;
            }
        }
    }

    // TODO: Properly predict the target's location based on its current velocity
    private Vector3 PredictedTargetLocation()
    {
        return target.position;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Unit" || collider.tag == "Building" || collider.tag == "Entity")
        {
            Entity hitEntity = collider.GetComponentInParent<Entity>();
            if (!entitiesPierced.Contains(hitEntity))
            {
                // Calculate the damage based
                // If this Projectile has pierced Entities before, reduce its damage
                float piercingFactor = remainingPiercingPower / info.piercingPower;
                piercingFactor = Mathf.Lerp(0.2f, 1, piercingFactor);
                AttackStrength attStr = info.attackStrength.MultiplyDamage(piercingFactor);
                //Debug.Log(attStr.damageComposition[0].APAmount);

                hitEntity.TakeDamage(attStr);

                // Prevent the Entity from taking damage from the same Projectile twice.
                entitiesPierced.Add(hitEntity);

                // See if we pierce through the Entity
                Pierce(hitEntity);
            }
        }
        else
        {
            //Debug.Log("Hit something else: " + collider.tag);
            //Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Handles piercing of Projectiles. If the Projectile does not have enough piercing power left, it gets destroyed.
    /// </summary>
    /// <param name="entity">The Entity that this Projectile is trying to pierce through.</param>
    private void Pierce(Entity entity)
    {
        remainingPiercingPower -= entity.entityInfo.mass;

        if (remainingPiercingPower <= 0)
            GameObject.Destroy(this.gameObject);
    }

    /// <summary>
    /// Returns a random unit vector in a 2D-circle.
    /// </summary>
    /// <returns></returns>
    public Vector3 RandomUnitVector()
    {
        float random = Random.Range(0f, 2 * Mathf.PI);
        return new Vector3(Mathf.Cos(random), 0, Mathf.Sin(random));
    }
}
    
