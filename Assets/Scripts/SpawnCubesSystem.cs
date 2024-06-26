using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEditor;

// Pour une fa�on plus s�re de spawner nos entities, voir PlayerShootingSystem.
public partial class SpawnCubesSystem : SystemBase  // On aurait pu impl�menter ISystem, mais on met SystemBase pour voir une autre fa�on de faire.
{
    protected override void OnCreate()
    {
        RequireForUpdate<SpawnCubesConfig>();   // Pour que la logique ne s'ex�cute que sur un objet qui a ce composant.
    }

    protected override void OnUpdate()
    {
        this.Enabled = false;   // Pour que cette update ne s'ex�cute qu'une seule fois.

        SpawnCubesConfig spawnCubesConfig = SystemAPI.GetSingleton<SpawnCubesConfig>();

        for (int i = 0; i < spawnCubesConfig.amountToSpawn; i++)
        {
            Entity spawnedEntity = EntityManager.Instantiate(spawnCubesConfig.cubePrefabEntity);


            // Ce qu'il faut savoir, c'est que lorsqu'on spawne un prefab en entity, sa m�thode Bake ne sera ex�cut� qu'une fois par prefab et non par instance cr�e,
            // donc ici nos cubes avec le composant MovementAuthoring auront tous le m�me float3 movement.


            SystemAPI.SetComponent(spawnedEntity, new LocalTransform
            {
                Position = new float3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f)),
                Rotation = quaternion.identity,
                Scale = 1f  // On est oblig� de le pr�ciser, sinon, de base, le scale sera de 0;
            });


            // Pour simplifier, on peut aussi faire :
            //SystemAPI.SetComponent(spawnedEntity, new LocalTransform());
            //LocalTransform.FromPosition(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));
        }


        // Si on veut faire spawner un grand nombre d'entities et optimiser au maximum,
        // mieux vaut utiliser une surcharge d'EntityManager.Instantiate() qui prend un tableau d'entities et les cr�e toutes ensemble.


        // Mais ATTENTION, cette m�thode peut cr�er des erreurs, il vaut donc mieux faire spawner nos entities comme dans PlayerShootingSystem.
    }
}
