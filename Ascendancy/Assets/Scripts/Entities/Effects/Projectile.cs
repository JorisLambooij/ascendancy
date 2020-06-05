using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileInfo info;

    private Rigidbody rb;
    private Transform target;

    // Each Entity can only be hit once by a projectile. This is to keep track of that.
    private HashSet<Entity> entitiesPierced;

    public void Launch(Entity launcher, Transform target, ProjectileInfo projectileInfo)
    {
        this.info = projectileInfo;
        this.target = target;
        rb = GetComponent<Rigidbody>();

        Vector3 predictedTargetLocation = PredictedTargetLocation();

        // Add some inaccuracy
        predictedTargetLocation += RandomUnitVector() * launcher.FindFeature<RangedAttackFeature>().accuracy;

        transform.LookAt(predictedTargetLocation, Vector3.up);
        Vector3 targetDir = (predictedTargetLocation - transform.position).normalized;
        rb.velocity = targetDir * info.launchVelocity;

        // Do not hit the launcher itself
        entitiesPierced = new HashSet<Entity>();
        entitiesPierced.Add(launcher);
        Destroy(this.gameObject, info.lifeTime);
    }
    

    void Update()
    {
        if (target != null)
        {
            Vector3 predictedTargetLocation = PredictedTargetLocation();

            // Target-Seeking
            Vector3 targetDir = predictedTargetLocation - transform.position;
            float step = info.targetSeekingCoefficient * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
            Vector3 newVel = Vector3.RotateTowards(rb.velocity, targetDir, step, 0.0F);

            Debug.DrawRay(transform.position, newDir, Color.red);
            transform.rotation = Quaternion.LookRotation(newDir);
            rb.velocity = newVel;
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
                hitEntity.TakeDamage(info.attackStrength);
                entitiesPierced.Add(hitEntity);
            }
        }
        else
        {

        }
    }


    public Vector3 RandomUnitVector()
    {
        float random = Random.Range(0f, 260f);
        return new Vector3(Mathf.Cos(random), 0, Mathf.Sin(random));
    }
}
    
