using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Camera playerCamera;
    public Camera minimapCamera;
    //public Collider floorCollider;
    public World world;
    public float yOffset = 64;

    public Vector3 topLeftPosition, topRightPosition, bottomLeftPosition, bottomRightPosition;
    public Vector3 mousePosition;

    // The "normal" position, fully zoomed out and centered
    private Vector3 standardPosition;
    private float standardSize;

    private Collider floorCollider;

    // Read-Properties for standard position and size.
    public float StandardSize { get => standardSize; }
    public Vector3 StandardPosition { get => standardPosition; }

    public void Start()
    {
        //this.minimapCamera = this.GetComponent<Camera>();
        if (this.playerCamera == null)
        {
            Debug.LogError("Unable to determine where the Player Camera component is at.");
        }

        if (this.minimapCamera == null)
        {
            Debug.LogError("Unable to determine where the Minimap Camera component is at.");
        }

        if (this.world == null)
        {
            //GameObject floorObject = GameObject.FindGameObjectWithTag("FloorCollider");
            //this.floorCollider = floorObject.GetComponent<Collider>();

            GameObject worldGO = GameObject.Find("World");
            this.world = worldGO.GetComponent<World>();

            if (this.floorCollider == null)
                Debug.LogError("Cannot set Quad floor collider to this variable. Please check.");

        }
        this.floorCollider = world.GetCollider();

        // Calculate the "normal" camera state.
        standardSize = 2 * world.worldSize / world.tileSize;
        Vector3 center = new Vector3(1, 0, 1) * standardSize;
        Vector3 yOffsetVector = new Vector3(0, yOffset, 0);
        standardPosition = world.transform.position + center + yOffsetVector;

        ResetMinimap();
    }
    
    /// <summary>
    /// Resets the Minimap to the highest zoom level, completely centered.
    /// </summary>
    public void ResetMinimap()
    {
        this.transform.position = standardPosition;
        GetComponent<Camera>().orthographicSize = standardSize;
    }

    public void Update()
    {
        Ray topLeftCorner = this.playerCamera.ScreenPointToRay(new Vector3(0f, 0f));
        Ray topRightCorner = this.playerCamera.ScreenPointToRay(new Vector3(Screen.width, 0f));
        Ray bottomLeftCorner = this.playerCamera.ScreenPointToRay(new Vector3(0, Screen.height));
        Ray bottomRightCorner = this.playerCamera.ScreenPointToRay(new Vector3(Screen.width, Screen.height));

        RaycastHit[] hits = new RaycastHit[4];
        if (this.floorCollider.Raycast(topLeftCorner, out hits[0], 40f))
        {
            this.topLeftPosition = hits[0].point;
        }
        if (this.floorCollider.Raycast(topRightCorner, out hits[1], 40f))
        {
            this.topRightPosition = hits[1].point;
        }
        if (this.floorCollider.Raycast(bottomLeftCorner, out hits[2], 40f))
        {
            this.bottomLeftPosition = hits[2].point;
        }
        if (this.floorCollider.Raycast(bottomRightCorner, out hits[3], 40f))
        {
            this.bottomRightPosition = hits[3].point;
        }

        this.topLeftPosition = this.minimapCamera.WorldToViewportPoint(this.topLeftPosition);
        this.topRightPosition = this.minimapCamera.WorldToViewportPoint(this.topRightPosition);
        this.bottomLeftPosition = this.minimapCamera.WorldToViewportPoint(this.bottomLeftPosition);
        this.bottomRightPosition = this.minimapCamera.WorldToViewportPoint(this.bottomRightPosition);

        this.topLeftPosition.z = -1f;
        this.topRightPosition.z = -1f;
        this.bottomLeftPosition.z = -1f;
        this.bottomRightPosition.z = -1f;
    }

    public void OnPostRender()
    {
        GL.PushMatrix();
        {
            GL.LoadOrtho();
            GL.Begin(GL.LINES);
            {
                GL.Color(Color.red);
                GL.Vertex(this.topLeftPosition);
                GL.Vertex(this.topRightPosition);
                GL.Vertex(this.topRightPosition);
                GL.Vertex(this.bottomRightPosition);
                GL.Vertex(this.bottomRightPosition);
                GL.Vertex(this.bottomLeftPosition);
                GL.Vertex(this.bottomLeftPosition);
                GL.Vertex(this.topLeftPosition);
            }
            GL.End();
        }
        GL.PopMatrix();
    }
}