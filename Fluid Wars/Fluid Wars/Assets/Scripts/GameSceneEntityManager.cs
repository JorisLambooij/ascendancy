using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

public class GameSceneEntityManager : MonoBehaviour
{
    public int numberOfEntities = 10;
    public Mesh mesh;
    public Material material;
    public int arenaRange;

    void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype dotEntityArchetype = entityManager.CreateArchetype(
            typeof(Translation),   //position in 2D space
            typeof(RenderMesh), // draw the mesh in 3D space
            typeof(LocalToWorld), // to get the world position
            typeof(COM_PlayerInfo) //info about owner
            );

        NativeArray <Entity> dotEntityArray = new NativeArray<Entity>(numberOfEntities, Allocator.Temp);
        entityManager.CreateEntity(dotEntityArchetype, dotEntityArray);

        for (int i = 0; i < dotEntityArray.Length; i++)
        {
            Entity dotEntity = dotEntityArray[i];

            int x = UnityEngine.Random.Range(-arenaRange, arenaRange);
            int y = UnityEngine.Random.Range(-arenaRange, arenaRange);
            entityManager.SetComponentData(dotEntity, new Translation
            {
                Value = new float3(x, y, 0)
            });

            
            entityManager.SetComponentData<COM_PlayerInfo>(dotEntity, new COM_PlayerInfo
            {
                playerID = 1,
                playerColor = Color.green
            });
            

            entityManager.SetSharedComponentData<RenderMesh>(dotEntity, new RenderMesh
            {
                mesh = this.mesh,
                material = this.material
            });
        }

        dotEntityArray.Dispose();
    }
}
