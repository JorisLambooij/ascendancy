using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using Mirror;
using System;

public class Entity : NetworkBehaviour, OccupationType
{
    public const float UPDATE_FREQ = 3;

    [SyncVar]
    public int ownerID;

    [SyncVar]
    public string entityInfoString;

    /// <summary>
    /// Holds all the stats for this Entity.
    /// </summary>
    public EntityInfo entityInfo;

    [HideInInspector]
    public Transform modelParent;

    protected List<EntityFeature> features;

    protected EntityOrderController controller;

    protected Sprite minimapMarker;

    public UnityEvent OnDestroyCallbacks;

    [ClientRpc]
    public void RpcSetOwner(Transform owner)
    {
        Debug.Log("set owner: " + owner.name);
        transform.SetParent(transform);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (entityInfo == null)
            entityInfo = ResourceLoader.GetEntityInfo(entityInfoString);

        //Debug.Log("Creating model for " + entityInfo.name);

        try
        {
            //Debug.Log(ResourceLoader.instance.entityInfoData);
            GameObject e_model = Instantiate(ResourceLoader.instance.entityInfoData[entityInfoString].prefab, transform);
            foreach (MeshRenderer mr in e_model.GetComponentsInChildren<MeshRenderer>())
            {
                foreach (Material mat in mr.materials)
                    if (mat.name.ToLower().Contains("playercolor"))
                        mat.SetColor("_BaseColor", Owner.PlayerColor);
            }
            modelParent = e_model.transform;
        }
        catch (Exception e)
        {
            Debug.LogError("Error when loading model for " + gameObject.name + "| " + e.Message);
        }
    }

    [ClientRpc]
    public void RpcCreateModel()
    {
        //Debug.Log("Creating model for " + entityInfo.name);
    }

    /// <summary>
    /// The player who owns this Entity.
    /// </summary>
    public Player Owner
    {
        get
        {
            if (transform.parent == null)
            {
                Player[] players = FindObjectsOfType<Player>();
                foreach (Player p in players)
                    if (p.playerID == ownerID)
                    {
                        if (entityInfo.construction_Method == ConstructionMethod.Unit)
                            transform.SetParent(p.UnitsGO);
                        else
                            transform.SetParent(p.BuildingsGO);
                    }

                if (transform.parent == null)
                {
                    Debug.LogError("Entity (" + gameObject.name + ") has no parent!");
                    return null;
                }
            }
            return transform.parent.GetComponentInParent<Player>();
        }
    }

    /// <summary>
    /// The current Health of this Entity.
    /// </summary>
    public float Health
    {
        get { return currentHealth; }
    }

    public EntityInfo GetEntityInfo()
    {
        return entityInfo;
    }

    public void LocalUpdate()
    {
        if (features != null)
            foreach (EntityFeature feature in features)
                feature.LocalUpdate();
    }

    /// <summary>
    /// Receive an order
    /// </summary>
    public virtual void ClickOrder(RaycastHit hit, bool enqueue = false, bool ctrl = false)
    {
        bool success = false;
        int i = 0;
        while (!success && i < features.Count)
            success = features[i++].ClickOrder(hit, enqueue, ctrl);
        
    }

    /// <summary>
    /// The current health status of this Entity.
    /// </summary>
    protected float currentHealth;

    /// <summary>
    /// Deal damage to this Entity.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(DamageComposition attackStrength)
    {
        float totalDamage = 0;
        foreach(DamageAmount dmgAmount in attackStrength.dmgComp)
        {
            float modifiedDamage = Mathf.Clamp(dmgAmount.nonAPAmount - entityInfo.armor, 1, dmgAmount.nonAPAmount);
            modifiedDamage += dmgAmount.APAmount;
            totalDamage += modifiedDamage;
        }

        totalDamage = Mathf.Clamp(totalDamage, 0, currentHealth);
        currentHealth -= totalDamage;

        if (currentHealth <= 0)
            Die();
    }

    public void TakeHealing(float amountRestored)
    {
        currentHealth = Mathf.Clamp(currentHealth + amountRestored, 0, entityInfo.maxHealth);
    }

    protected virtual void Start()
    {
        // Create the Selection Marker
        GameObject selectionMarkerPrefab = Resources.Load("Prefabs/UI/SelectionMarker") as GameObject;
        Instantiate(selectionMarkerPrefab, this.transform);

        controller = transform.GetComponent<EntityOrderController>();

        //if (controller == null)
        //    Debug.LogWarning("EntityController not found on " + transform.name);

        // Create a map marker for this Entity
        minimapMarker = entityInfo.minimapMarker;
        GameObject markerObject = Resources.Load("Prefabs/UI/MinimapMarker") as GameObject;

        // If a sprite was provided, we use it while keeping the position and settings
        if (minimapMarker != null)
        {
            markerObject.GetComponent<SpriteRenderer>().sprite = minimapMarker;
            //Debug.Log(markerObject.GetComponent<SpriteRenderer>().sprite.name);
        }
        Instantiate(markerObject, this.transform);

        // Start with max Health
        this.currentHealth = entityInfo.maxHealth;

        // Copy all features as new objects, and immediately sort them by priority.
        int count = entityInfo.entityFeatures.Count;
        EntityFeature[] featuresCopy = new EntityFeature[count];

        for (int i = 0; i < count; i++)
        {
            EntityFeature f = Instantiate(entityInfo.entityFeatures[i]);
            featuresCopy[i] = f;
        }
        //entityInfo.EntityFeatures.CopyTo(featuresCopy);
        
        features = featuresCopy.OrderBy(f => -f.clickPriority).ToList();

        // Initialize all Features
        foreach (EntityFeature feature in features)
            feature.Initialize(this);

        // Start the 10-second Update Routine
        StartCoroutine(Update10Coroutine());
    }

    protected virtual void Update()
    {
        foreach (EntityFeature feature in features)
            feature.UpdateOverride();
    }

    protected virtual void Update10()
    {
        foreach (EntityFeature feature in features)
            feature.Update10Override();
    }

    protected IEnumerator Update10Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(UPDATE_FREQ);
            Update10();
        }

    }

    /// <summary>
    /// Remove this Entity from the game
    /// </summary>
    protected virtual void Die()
    {
        OnDestroyCallbacks.Invoke();
        Destroy(this.gameObject);
    }

    public T FindFeature<T>() where T : EntityFeature
    {
        if (features == null)
            return default(T);

        foreach(EntityFeature feature in features)
        {
            if (feature.GetType() == typeof(T))
                return feature as T;
        }
        return default(T);
    }

    public void ForceMove(Vector3 position)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.enabled = false;
        transform.position = position;
        if (agent != null)
            agent.enabled = true;
    }

    /// <summary>
    /// Relay an order to this Entity.
    /// </summary>
    /// <param name="order">The order that is being issued.</param>
    /// <param name="enqueue">Whether the order should be queued or replace the current order queue. </param>
    public void IssueOrder(UnitOrder order, bool enqueue)
    {
        if (controller == null)
        {
            Debug.LogError("Issuing order to Entity without EntityOrderController. Most likely because it is a building");
            return;
        }

        if (enqueue)
            controller.orders.Enqueue(order);
        else
        {
            controller.orders.Clear();
            controller.NewOrder(order);
        }
    }

    public EntityOrderController Controller
    {
        get { return controller; }
    }
}
