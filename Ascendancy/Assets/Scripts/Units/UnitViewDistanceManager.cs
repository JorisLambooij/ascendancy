using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitViewDistanceManager : MonoBehaviour
{
    public CapsuleCollider viewCollider;

    private float viewDistance = 10;

    void Start()
    {
        Unit unitScript = GetComponent<Unit>();
        viewDistance = unitScript.unitInfo.viewDistance;

        viewCollider.radius = viewDistance;
    }

    void Update()
    {
        
    }
}
