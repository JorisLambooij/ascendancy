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
    public float zoomSpeed;
    public float targetHeight;


    private Camera cam;
    private Transform center;
    private float distanceFromCenter;

    [Range(5, 120)]
    public float currentZoom;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        center = transform.parent;
        distanceFromCenter = (transform.position - center.position).magnitude;
    }

    void Update()
    {
        WASD();
        ZoomLevel();
    }

    private void ZoomLevel()
    {
        float scroll = Input.mouseScrollDelta.y * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom - scroll, 5, 120);
        float inRads = Mathf.Deg2Rad * Mathf.Clamp(currentZoom, 10, 80);
        float height = inRads;
        float distance = Mathf.Sin(inRads);

        Vector3 d = Vector3.ProjectOnPlane((transform.position - center.position), Vector3.up).normalized;
        Vector3 h = Vector3.up;

        transform.position = center.position + (distance * d + h * height) * (currentZoom / 5f);
        cam.transform.LookAt(center);
    }

    private void WASD()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float rotational = Input.GetAxis("Rotational");

        Vector3 pos = center.position;
        Vector3 movement = Vector3.ProjectOnPlane(transform.right, Vector3.up) * horizontal + Vector3.ProjectOnPlane(transform.forward, Vector3.up) * vertical;
        pos += movement * Time.deltaTime * cameraSpeed;

        float tileHeight = World.Instance.GetTile(pos).height;
        float desiredY = (targetHeight + tileHeight) * 0.5f;

        pos.y = Mathf.Lerp(pos.y, desiredY, 10 * Time.deltaTime);

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

        Ray r = new Ray(transform.position, point2 - transform.position);
        Debug.DrawRay(r.origin, r.direction * 50, Color.red);
        return r;
    }
}
