using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public float percentage = 0;

    public Color fillColor;
    public Color backgroundColor;

    public bool alwaysVisible = false;
    public float margin = 0.05f;

    protected Camera cam;
    protected SpriteRenderer barBack, barFront;

    // Start is called before the first frame update
    public virtual void Start()
    {
        cam = Camera.main;

        barBack = GetComponent<SpriteRenderer>();
        barFront = transform.GetChild(0).GetComponent<SpriteRenderer>();

        barBack.color = backgroundColor;
        barFront.color = fillColor;
    }


    // Update is called once per frame
    public virtual void Update()
    {
        if (float.IsNaN(percentage))
            Debug.LogError("Current Value is NaN");

        UpdateSize(percentage);
        //UpdateColor(percentage);
    }


    protected void UpdateSize(float percentage)
    {
        float relativePosition = Mathf.Clamp01(1 - percentage);
        barFront.transform.localPosition = new Vector3(relativePosition, 0, 0);
        barFront.transform.localScale = new Vector3(20 * percentage, 1 + margin, 1);

        transform.LookAt(transform.position - cam.transform.forward);
    }
}
