using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRandonmizer : MonoBehaviour
{
    public bool randomizeRotation = true;
    // This is necessary if you import your models from 3ds
    public bool rotationFix = false;

    public bool randomizeColor = false;
    public Color randomColor1;
    public Color randomColor2;

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

            this.transform.localRotation = Quaternion.Euler(this.transform.rotation.x + fix, this.transform.rotation.y, Random.Range(0, 360));
        }

        // change a bit the seize of the model.
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

        // change a bit the colors.
        if (randomizeColor && randomColor1 != randomColor2)
        {
            if (this.gameObject.GetComponent<MeshRenderer>() != null)
            {
                this.gameObject.GetComponent<MeshRenderer>().material.color = Color.Lerp(randomColor1, randomColor2, Random.value);
            }
            else
            {
                Debug.Log("ColorA[" + this.name + "]: " + this.gameObject.GetComponentInChildren<MeshRenderer>().material.color);
                this.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.Lerp(randomColor1, randomColor2, Random.value);
                Debug.Log("ColorB[" + this.name + "]: " + this.gameObject.GetComponentInChildren<MeshRenderer>().material.color);
            }
        }
    }
}
