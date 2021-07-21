using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapImage : MonoBehaviour
{
    private World world;

    public RawImage terrainImage;
    public RawImage maskImage;

    public Texture maskRenderTexture;

    // Start is called before the first frame update
    void Start()
    {
        if (this.world == null)
        {
            //GameObject floorObject = GameObject.FindGameObjectWithTag("FloorCollider");
            //this.floorCollider = floorObject.GetComponent<Collider>();

            GameObject worldGO = GameObject.Find("World");
            this.world = worldGO.GetComponent<World>();

        }

        terrainImage.texture = world.TerrainColorTexture;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
