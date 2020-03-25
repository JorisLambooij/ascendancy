using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public class GameSceneEntityManager : MonoBehaviour
{
    void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype dotEntityArchetype = entityManager.CreateArchetype(
            typeof(COM_Position),   //position in 2D space
            typeof(COM_PlayerInfo)  //info about owner
            );

        NativeArray < Entity > dotEntityArray = new NativeArray<Entity>(1, Allocator.Temp);
        entityManager.CreateEntity(dotEntityArchetype, dotEntityArray);

        for (int i = 0; i < dotEntityArray.Length; i++)
        {
            Entity dotEntity = dotEntityArray[i];

            entityManager.SetComponentData(dotEntity, new COM_Position
            {
                x = Random.Range(-50, 50),
                y = Random.Range(-50, 50)
        });

            //strange fuckery happens below

            //entityManager.SetSharedComponentData<COM_PlayerInfo>(dotEntity, new COM_PlayerInfo
            //{
            //    playerID = 3,
            //    playerColor = Color.green
            //});
        }

        dotEntityArray.Dispose();
    }
}
