using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScript : MonoBehaviour
{
    private Camera cam;
    
    void Start()
    {
        cam = GetComponent<Camera>();
    }
    
    public Ray MouseCursorRay()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 point2 = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

        return new Ray(transform.position, point2 - transform.position);
    }
}
