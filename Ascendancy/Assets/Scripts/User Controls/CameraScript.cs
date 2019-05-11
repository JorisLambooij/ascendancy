using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera controls.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraScript : MonoBehaviour
{
    public float cameraSpeed;
    public float cameraRotationSpeed;

    private Camera cam;
    private Transform center;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        center = transform.parent;
    }

    void Update()
    {
        WASD();
    }

    private void WASD()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float rotational = Input.GetAxis("Rotational");

        Vector3 pos = center.position;
        Vector3 movement = Vector3.ProjectOnPlane(transform.right, Vector3.up) * horizontal + Vector3.ProjectOnPlane(transform.forward, Vector3.up) * vertical;
        pos += movement * Time.deltaTime * cameraSpeed;

        center.position = pos;

        //Vector3 r = transform.rotation.eulerAngles;
        //r.x += rotational * Time.deltaTime * cameraRotationSpeed;
        //transform.rotation = new Quaternion(r.x, r.y, r.z, 1);

        center.Rotate(Vector3.up, rotational * Time.deltaTime * cameraRotationSpeed);
    }
    public Ray MouseCursorRay()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 point2 = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

        return new Ray(transform.position, point2 - transform.position);
    }
}
