using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    /// <summary>
    /// Holds all the stats for this Entity.
    /// </summary>
    public EntityInfo entityInfo;

    private List<EntityFeature> features;

    private UnitController controller;

    private Sprite minimapMarker;

    /// <summary>
    /// The player who owns this Entity.
    /// </summary>
    public Player Owner
    {
        get { return transform.parent.GetComponentInParent<Player>(); }
    }

    /// <summary>
    /// The current Health of this Entity.
    /// </summary>
    public int Health
    {
        get { return currentHealth; }
    }

    /// <summary>
    /// Receive an order
    /// </summary>
    public virtual void ClickOrder(RaycastHit hit, bool enqueue)
    {
        bool success = false;
        int i = 0;
        while (!success && i < features.Count)
            success = features[i++].ClickOrder(this, hit, enqueue);
        
    }

    /// <summary>
    /// The current health status of this Entity.
    /// </summary>
    protected int currentHealth;

    /// <summary>
    /// Deal damage to this Entity.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        damage = Mathf.Clamp(damage, 0, currentHealth);
        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Start()
    {
        GameObject selectionMarkerPrefab = Resources.Load("Prefabs/SelectionMarker") as GameObject;
        Instantiate(selectionMarkerPrefab, this.transform);

        this.currentHealth = entityInfo.MaxHealth;

        // Copy all features as new objects, and immediately sort them by priority.
        EntityFeature[] featuresCopy = new EntityFeature[entityInfo.EntityFeatures.Count];
        entityInfo.EntityFeatures.CopyTo(featuresCopy);
        
        features = featuresCopy.OrderBy(f => -f.clickPriority).ToList();

        foreach (EntityFeature feature in features)
            feature.Initialize(this);
    }

    protected virtual void Update()
    {
        foreach (EntityFeature feature in features)
            feature.UpdateOverride(this);
    }

    /// <summary>
    /// Remove this Entity from the game
    /// </summary>
    protected virtual void Die()
    {
        Destroy(this.gameObject);
    }

    public T FindFeature<T>() where T : EntityFeature
    {
        foreach(EntityFeature feature in features)
        {
            if (feature.GetType() == typeof(T))
                return feature as T;
        }
        return default(T);
    }


    /// <summary>
    /// Relay an order to this Entity.
    /// </summary>
    /// <param name="order">The order that is being issued.</param>
    /// <param name="enqueue">Whether the order should be queued or replace the current order queue. </param>
    public void IssueOrder(UnitOrder order, bool enqueue)
    {
        if (enqueue)
            controller.orders.Enqueue(order);
        else
        {
            controller.orders.Clear();
            controller.NewOrder(order);
        }
    }

    public UnitController Controller
    {
        get { return controller; }
    }
}
