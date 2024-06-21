using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial class SpawnCubesSystem : SystemBase  // On aurait pu implémenter ISystem, mais on met SystemBase pour voir une autre façon de faire.
{
    protected override void OnCreate()
    {
        RequireForUpdate<SpawnCubesConfig>();   // Pour que la logique ne s'exécute que sur un objet qui a ce composant.
    }

    protected override void OnUpdate()
    {
        this.Enabled = false;   // Pour que cette update ne s'exécute qu'une seule fois.

        SpawnCubesConfig spawnCubesConfig = SystemAPI.GetSingleton<SpawnCubesConfig>();

        for (int i = 0; i < spawnCubesConfig.amountToSpawn; i++)
        {
            Entity spawnedEntity = EntityManager.Instantiate(spawnCubesConfig.cubePrefabEntity);


            // Ce qu'il faut savoir, c'est que lorsqu'on spawne un prefab en entity, sa méthode Bake ne sera exécuté qu'une fois par prefab et non par instance crée,
            // donc ici nos cubes avec le composant MovementAuthoring auront tous le même float3 movement.


            EntityManager.SetComponentData(spawnedEntity, new LocalTransform
            {
                Position = new float3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f)),
                Rotation = quaternion.identity,
                Scale = 1f  // On est obligé de le préciser, sinon, de base, le scale sera de 0;
            });


            // Pour simplifier, on peut aussi faire :
            //EntityManager.SetComponentData(spawnedEntity, new LocalTransform());
            //LocalTransform.FromPosition(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));

        }
    }
}
