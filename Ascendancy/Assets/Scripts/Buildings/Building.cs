using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Entity
{

    /// <summary>
    /// Holds all the stats for this Unit.
    /// </summary>
    public UnitInfo unitInfo;

    public override void ClickOrder(RaycastHit hit, bool enqueue)
    {
        throw new System.NotImplementedException();
    }

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
