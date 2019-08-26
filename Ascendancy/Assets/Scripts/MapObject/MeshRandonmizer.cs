using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRandonmizer : MonoBehaviour
{
    public bool randomizeRotation = true;
    // This is necessary if you import your models from 3ds
    public bool rotationFix = false;

    public bool randomizeScale = true;

    public bool uniformScale = true;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    // Start is called before the first frame update
    void Start()
    {
        if (randomizeRotation)
        {

            float fix = 0;
            if (rotationFix)
            {
                fix = -90;
            }

            this.transform.localRotation = Quaternion.Euler(this.transform.rotation.x + fix, Random.Range(0, 360), this.transform.rotation.z );
        }

        // change the model's size
        if (randomizeScale)
        {
            if (uniformScale)
            {
                float v = Random.Range(minScale, maxScale);
                this.transform.localScale = new Vector3(v, v, v);
            }
            else
            {
                float vX = Random.Range(minScale, maxScale);
                float vY = Random.Range(minScale, maxScale);
                float vZ = Random.Range(minScale, maxScale);

                this.transform.localScale = new Vector3(vX, vY, vZ);
            }
        }
    }
}
