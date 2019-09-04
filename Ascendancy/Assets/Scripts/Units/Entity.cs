using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    /// <summary>
    /// The current health status of this Entity.
    /// </summary>
    protected int currentHealth;

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
    public abstract void ClickOrder(RaycastHit hit, bool enqueue);

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
    }

    protected virtual void Update()
    {

    }

    /// <summary>
    /// Remove this Entity from the game
    /// </summary>
    protected virtual void Die()
    {
        Destroy(this.gameObject);
    }
}
