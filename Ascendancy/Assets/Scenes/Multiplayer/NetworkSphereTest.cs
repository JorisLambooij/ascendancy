using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkSphereTest : NetworkBehaviour
{
    [SyncVar]
    public int testValue = 1;

    private void Update()
    {
        if (isServer)
            if (Input.GetKeyDown(KeyCode.Space))
                testValue++;
    }

}
