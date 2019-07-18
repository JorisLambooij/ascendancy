using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Entity
{

    /// <summary>
    /// Holds all the stats for this Unit.
    /// </summary>
    public UnitInfo unitInfo;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Owner.playerNo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
