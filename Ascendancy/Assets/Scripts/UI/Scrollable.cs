using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Scrollable : MonoBehaviour
{
    public bool invertDrag = false;

    private Vector3 lastMousePos;

    public void OnMouseDown()
    {
        lastMousePos = Input.mousePosition;
    }

    public void OnMouseDrag()
    {
        Vector3 currentMousePos = Input.mousePosition;
        Vector3 delta = currentMousePos - lastMousePos;
        
        transform.position += delta;

        lastMousePos = currentMousePos;
    }
}
