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
        Vector2 adjustedSize = rectTransform.sizeDelta * rectTransform.localScale.x * rectTransform.parent.localScale.x;
        float minX = Screen.width < adjustedSize.x ? -adjustedSize.x + 50 : -200;
        float minY = Screen.height < adjustedSize.y ? 200 : adjustedSize.y;
        //Debug.Log(adjustedSize.y + " |  " + transform.position.y);
        transform.position = new Vector3(Mathf.Clamp(newPos.x, minX, Screen.width - 200), Mathf.Clamp(newPos.y, 0, Screen.height + Mathf.Max(adjustedSize.y - 300, 0)), 0);

        lastMousePos = currentMousePos;
    }
}
