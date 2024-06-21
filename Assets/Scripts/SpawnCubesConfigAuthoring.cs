using Unity.Entities;
using UnityEngine;


// Maintenant, on veut faire spawner nos cubes à partir de prefabs.
public class SpawnCubesConfigAuthoring : MonoBehaviour
{
    public GameObject cubePrefab;
    public int amountToSpawn;

    public class Baker : Baker<SpawnCubesConfigAuthoring>
    {
        public override void Bake(SpawnCubesConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);    // Ici, on ne veut pas modifier les propriétés du Transform.
            AddComponent(entity, new SpawnCubesConfig
            {
                cubePrefabEntity = GetEntity(authoring.cubePrefab, TransformUsageFlags.Dynamic),
                amountToSpawn = authoring.amountToSpawn
            });
        }
    }
}


public struct SpawnCubesConfig : IComponentData
{
    public Entity cubePrefabEntity;
    public int amountToSpawn;
}
