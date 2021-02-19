using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage the color and position of a Health Bar.
/// </summary>
public class Healthbar : MonoBehaviour
{
    public bool activeOverride = false;
    public float margin = 0.05f;

    private Camera cam;
    private Entity entity;

    private SpriteRenderer barBack, barFront;

    // Start is called before the first frame update
    void Start()
    {
        entity = transform.GetComponentInParent<Entity>();
        cam = Camera.main;

        barBack = GetComponent<SpriteRenderer>();
        barFront = transform.GetChild(0).GetComponent<SpriteRenderer>();

        barBack.color = Color.red;
        barFront.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        if (entity != null)
        {
            int maxHealth = entity.entityInfo.maxHealth;
            float currentHealth = entity.Health;
            bool visible = maxHealth > currentHealth || activeOverride;


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

    void UpdateSize(float percentage)
    {
        float relativePosition = Mathf.Clamp01(1 - percentage);
        barFront.transform.localPosition = new Vector3(relativePosition, 0, 0);
        barFront.transform.localScale = new Vector3(20 * percentage, 1 + margin, 1);

        transform.LookAt(transform.position - cam.transform.forward);
    }

    void UpdateColor(float percentage)
    {
        barFront.color = new Color(1 - percentage, percentage, 0);
    }

    public void SetEntity(Entity entity)
    {
        this.entity = entity;
    }
}
