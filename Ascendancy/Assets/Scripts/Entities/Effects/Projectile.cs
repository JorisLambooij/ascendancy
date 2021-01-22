using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileInfo info;

    private Entity launcher;
    private Transform target;
    private Vector3 launchPos;
    private Vector3 predictedTargetLocation;
    private float remainingPiercingPower;

    // Each Entity can only be hit once by a projectile. This is to keep track of that.
    private HashSet<Entity> entitiesPierced;
    
    public void Launch(RangedAttackFeature rangedFeature, Transform target)
    {
        this.info = rangedFeature.projectileInfo;
        this.target = target;
        this.transform.position = rangedFeature.entity.transform.position;
        this.launchPos = transform.position;
        
        // Get the location we want this Projectile to aim for.
        predictedTargetLocation = target.position;
        Vector3 distance = predictedTargetLocation - transform.position;

        // Add some inaccuracy
        float relativeRange = Mathf.InverseLerp(rangedFeature.minRange, rangedFeature.maxRange, distance.magnitude);
        float inaccuracyLerped = Mathf.Lerp(0, rangedFeature.inaccuracy, relativeRange);
        predictedTargetLocation += RandomUnitVector() * inaccuracyLerped;

        // Launch the Projectile.
        transform.LookAt(predictedTargetLocation, Vector3.up);

        /*
        Vector3 targetDir = (predictedTargetLocation - transform.position).normalized;

        // Make the Projectile travel in a parabolic arc.
        Vector3 arcing = new Vector3(0, Mathf.Sqrt(-Physics.gravity.y * info.arcHeight), 0);
        //Debug.Log(targetDir.y);
        RB.useGravity = arcing.y > 0;
        RB.velocity = targetDir * info.launchVelocity + arcing;
        */

        // Do not hit the launcher itself
        remainingPiercingPower = info.piercingPower;
        entitiesPierced = new HashSet<Entity>();
        entitiesPierced.Add(rangedFeature.entity);

        // Handle the timing stuff
        StartCoroutine(TempDeactivateCollider(0.2f));
        Destroy(this.gameObject, info.lifeTime);
    }
    

    void Update()
    {
        PredictTargetLocation();

        // Predict next location for Target-Seeking
        Vector3 targetDir = predictedTargetLocation - transform.position;
        
        Vector2 p0 = Project2D(launchPos);
        Vector2 p1 = Project2D(predictedTargetLocation);
        float dist = (p1 - p0).magnitude;
        Vector2 nextP = Vector2.MoveTowards(Project2D(transform.position), p1, info.horizontalVelocity * Time.deltaTime);
        float baseY = Mathf.Lerp(launchPos.y, predictedTargetLocation.y, (nextP - p0).magnitude / dist);
        float arc = info.arcHeight * (nextP - p0).magnitude * (nextP - p1).magnitude / (0.25f * dist * dist);
        Vector3 nextPos = new Vector3(nextP.x, baseY + arc, nextP.y);

        // Check if y is defined. If not, stop.
        if (float.IsNaN(nextPos.y))
            return;
        // Rotate to face the next position, and then move there
        if ((nextPos - transform.position).sqrMagnitude > 0)
            transform.rotation = Quaternion.LookRotation(nextPos - transform.position, Vector3.up);
        transform.position = nextPos;
    }
    

    // TODO: Properly predict the target's location based on its current velocity
    private void PredictTargetLocation()
    {
        if (target == null)
            return;

        predictedTargetLocation = Vector3.MoveTowards(predictedTargetLocation, target.position, info.targetSeekingCoefficient * Time.deltaTime);
    }

    /// <summary>
    /// What happens when this Projectile hits something
    /// </summary>
    /// <param name="collider">The Collider that the Projectile hit.</param>
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
                DamageComposition attStr = info.rangedDamage.MultiplyDamage(piercingFactor);
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
            Debug.Log("Hit something else: " + collider.tag);
            DestroyProjectile();
        }
    }

    IEnumerator TempDeactivateCollider(float time)
    {
        Collider coll = GetComponent<Collider>();
        coll.enabled = false;
        yield return new WaitForSeconds(time);
        coll.enabled = true;
    }

    /// <summary>
    /// Handles piercing of Projectiles. If the Projectile does not have enough piercing power left, it gets destroyed.
    /// </summary>
    /// <param name="entity">The Entity that this Projectile is trying to pierce through.</param>
    private void Pierce(Entity entity)
    {
        remainingPiercingPower -= entity.entityInfo.mass;

        if (remainingPiercingPower <= 0)
            DestroyProjectile();
    }

    protected void DestroyProjectile()
    {
        if (info is ExplodingProjectileInfo)
        {
            ExplodingProjectileInfo explodeInfo = info as ExplodingProjectileInfo;
            if (explodeInfo == null)
            {
                Debug.LogError("Something horrible has gone wrong");
                return;
            }
            if (explodeInfo.explosionEffect == null)
                Debug.LogError("Explosion Radius set to >0, but no Effect was provided. Please check " + launcher.name);
            else
            {
                Explosion explosion = Instantiate(explodeInfo.explosionEffect).GetComponent<Explosion>();
                explosion.transform.position = transform.position;
                if (explosion != null)
                    explosion.Explode(explodeInfo, launcher);
                else
                    Debug.LogError(launcher.name + " Effect has no Explosion-script.");
            }
        }

        Destroy(this.gameObject);
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

    private Vector2 Project2D(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
}
    
