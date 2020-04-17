using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class SYS_Mover : ComponentSystem
{
    private float moveSpeed;
    private Camera camera;

    protected override void OnStartRunning()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        moveSpeed = GameObject.Find("SettingsObject").GetComponent<GameSettings>().movementSpeed;
    }

    protected override void OnUpdate()
    {
        bool move = false;
        float3 target = new float3(0, 0, 0);

        if (Input.GetMouseButton(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                if (hit.transform.CompareTag("BackgroundPlane"))
                {
                    move = true;
                    target = hit.point;
                }
        }

        
        Entities.ForEach((ref Translation t) =>
        {
            if (move)
            {
                Vector3 movementVector = target - t.Value;
                if (movementVector.sqrMagnitude > moveSpeed)
                    movementVector = movementVector.normalized * moveSpeed;

                float3 movementFloat3 = movementVector * Time.DeltaTime;
                t.Value += movementFloat3;
            }
        }
        );
    }
}
