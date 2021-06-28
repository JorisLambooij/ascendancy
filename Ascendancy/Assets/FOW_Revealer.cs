using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOW_Revealer : MonoBehaviour
{
    void Start()
    {
        Entity e = GetComponentInParent<Entity>();
        if (e != null)
        {
            float s = 1 + 3f * e.entityInfo.viewDistance;
            transform.localScale = new Vector3(s, s, s);
        }
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.y = 10;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
