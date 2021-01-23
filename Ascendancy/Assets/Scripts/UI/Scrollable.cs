using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Scrollable : MonoBehaviour
{
    public bool invertDrag = false;

    public Rect boundRect;

    private Vector3 lastMousePos;

    public void OnMouseDown()
    {
        lastMousePos = Input.mousePosition;
    }

    public void OnMouseDrag()
    {
        Vector3 currentMousePos = Input.mousePosition;
        Vector3 delta = currentMousePos - lastMousePos;

        if (invertDrag)
            delta *= -1;

        Vector3 newPos = transform.position + delta;
        RectTransform rectTransform = GetComponent<RectTransform>();
        Debug.Log(newPos);
        transform.position = new Vector3(Mathf.Clamp(newPos.x, boundRect.x, boundRect.x + boundRect.width), Mathf.Clamp(newPos.y, boundRect.y, boundRect.y + boundRect.height), 0);

        lastMousePos = currentMousePos;
    }
}
