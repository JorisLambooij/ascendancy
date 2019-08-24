using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalView : MonoBehaviour
{
    public Camera portalCam;
    public MeshRenderer portalPlane;
    public Material portalRenderMaterialPrefab;
    
    public RenderTexture portalRenderTexture;
    public int portalResolution = 256;

    private Portal thisPortal;

    // Start is called before the first frame update
    void Start()
    {
        thisPortal = GetComponentInParent<Portal>();

        Debug.Assert(thisPortal != null);

        // Create a new RenderTexture for this portal
        portalRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        portalRenderTexture.Create();
    }

    // Update is called once per frame
    void Update()
    {
        portalCam.fieldOfView = Camera.main.fieldOfView;

        Vector3 posInLocalSpaceOfThisPortal = thisPortal.transform.worldToLocalMatrix * Camera.main.transform.position;
        Debug.Log("Local: " + posInLocalSpaceOfThisPortal);

        portalCam.transform.position = thisPortal.partnerPortal.transform.localToWorldMatrix * posInLocalSpaceOfThisPortal;

        
        // look in the same direction, adjusting for portal rotation
        float angularDifferenceOfPortals = Quaternion.Angle(thisPortal.transform.rotation, thisPortal.partnerPortal.transform.rotation);
        Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDifferenceOfPortals, Vector3.up);
        Vector3 newCameraDirection = portalRotationalDifference * Camera.main.transform.forward;
        
        // adjust the rotation of the portal camera
        portalCam.transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
        
        // the relative position of the main cam to this portal
        //Vector3 mainCamOffset = Camera.main.transform.position - thisPortal.transform.position;

        // set the portal camera to the same position, relative to the partner portal
        //portalCam.transform.position = thisPortal.partnerPortal.transform.position + mainCamOffset;
        //portalCam.transform.RotateAround(thisPortal.partnerPortal.transform.position, Vector3.up, -angularDifferenceOfPortals);
        
    }

    /// <summary>
    /// Called by the Portal script after the partnerPortal has been assigned. This Method will handle the visuals only.
    /// </summary>
    public void LinkPortalView()
    {
        // Set the camera of the other portal to render to the RenderTexture of this portal
        PortalView otherPortalView = thisPortal.partnerPortal.GetComponentInChildren<PortalView>();
        this.portalCam.targetTexture = portalRenderTexture;

        // Create a copy of the portal material and use the RenderTexture
        Material portalMat = new Material(portalRenderMaterialPrefab);
        portalPlane.material = portalMat;

        portalPlane.material.mainTexture = portalRenderTexture;
    }
}
