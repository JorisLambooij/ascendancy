using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;

public class EntityManagerScript : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;
    
    // Start is called before the first frame update
    void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityArchetype sphereArchetype = entityManager.CreateArchetype(
            typeof(Sphere),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld)
            );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(1, Allocator.Temp);
        entityManager.CreateEntity(sphereArchetype, entityArray);

        for (int i = 0; i < entityArray.Length; i++)
        {
            Entity sphereEntity = entityArray[i];
            float h, s = 1, v = 1;
            h = Random.Range(0, 1);
            Color rgb = Color.HSVToRGB(h, s, v);
            entityManager.SetComponentData(sphereEntity, new Sphere { color = rgb });

            entityManager.SetSharedComponentData(sphereEntity, new RenderMesh { mesh = this.mesh, material = this.material, });
        }

        entityArray.Dispose();
    }
}
