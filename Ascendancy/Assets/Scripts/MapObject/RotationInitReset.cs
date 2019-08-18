using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationInitReset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.rotation = Quaternion.identity;
        this.transform.Rotate(Vector3.right, 90);
    }

}
