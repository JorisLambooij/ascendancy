using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage the color and position of a Health Bar.
/// </summary>
public class Healthbar : ProgressBar
{
    private Entity entity;

    // Start is called before the first frame update
    void Start()
    {
        entity = transform.GetComponentInParent<Entity>();
    }

    // Update is called once per frame
    public override void Update()
    {
        if (entity != null)
        {
            int maxHealth = entity.entityInfo.maxHealth;
            float currentHealth = entity.Health;
            bool visible = maxHealth > currentHealth || alwaysVisible;

            if (visible)
            {
                if (float.IsNaN(currentHealth))
                    Debug.LogError("Current Health is NaN");
                float percentage = Mathf.Clamp((float)currentHealth / maxHealth, 0, 1);
                
                UpdateSize(percentage);
                //UpdateColor(percentage);
            }

            barBack.enabled = visible;
            barFront.enabled = visible;
        }
    }

    public void SetEntity(Entity entity)
    {
        this.entity = entity;
    }
}
