using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkSphereTest : NetworkBehaviour
{
    [SyncVar(hook = nameof (TestValueHook))]
    public int testValue = 1;

    private void Start()
    {
        Debug.Log("Sphere is here!");
    }

    void TestValueHook(int oldValue, int newValue)
    {
        testValue = newValue;
        Debug.Log("Sphere value: " + newValue);
        transform.localScale = Vector3.one * newValue;
    }

    private void Update()
    {
        if (isServer)
            if (Input.GetKeyDown(KeyCode.Space))
                testValue++;
    }

}
