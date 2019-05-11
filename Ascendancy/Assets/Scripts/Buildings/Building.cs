using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Entity
{
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
