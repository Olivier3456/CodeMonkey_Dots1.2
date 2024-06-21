using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public partial class PlayerShootingSystem : SystemBase
{
    // Comme nous sommes dans un SystemBase, qui est pour les Managed Components (ISystem est pour les Unmanaged), nous pouvons utiliser des events:
    public event EventHandler OnShoot;


    protected override void OnCreate()
    {
        RequireForUpdate<Player>();
    }

    protected override void OnUpdate()
    {
        // Pour le test, quand on appuie sur T, notre player devient stunned (voir StunnedAuthoring pour mieux comprendre).
        if (Input.GetKeyDown(KeyCode.T))
        {
            Entity playerEntity = SystemAPI.GetSingletonEntity<Player>();
            EntityManager.SetComponentEnabled<Stunned>(playerEntity, true);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Entity playerEntity = SystemAPI.GetSingletonEntity<Player>();
            EntityManager.SetComponentEnabled<Stunned>(playerEntity, false);    // Ici nous dé-stunnons notre player.
        }
        // On peut voir les tags du player dans son Inspector, en mode Runtime.


        if (!Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }


        SpawnCubesConfig spawnCubesConfig = SystemAPI.GetSingleton<SpawnCubesConfig>();

        // En ajoutant une entity en runtime, on risque de casser des choses, car la boucle foreach ci-dessous itère dans la collection même où on ajoute nos entities.
        // C'est pourquoi il vaut mieux instancier les entities avec un EntityCommandBuffer :

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(WorldUpdateAllocator);    // WorldUpdateAllocator fait gagner un peu en performances.

        foreach ((RefRO<LocalTransform> localTransform, Entity entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Player>().WithDisabled<Stunned>().WithEntityAccess())
        // Fait spawner un cube pour chaque Player dans la scène, à condition que le Player ne soit pas Stunned (voir StunnedAuthoring).
        // Si au contraire on veut que l'action ne se déclenche que si le composant Stunned est Enabled, comme il n'existe pas de "WithEnabled", donc il faut juste ajouter Stunned dans le WithAll, à la suite de Player. 
        // "Entity entity" nous sert à récupérer une référence de l'entity concerné, ce qui nous permet de renseigner quelle entity déclenche l'event OnShoot, quand on l'invoke (voir ci-dessous).
        // Ici, cela peut être utile s'il y a plusieurs Players dans la scène, et qu'on veut savoir lequel a tiré.
        // WithEntityAccess(), à la fin, est ce qui permet de récupérer cette référence à l'entity. Si on ne veut pas récupérer cette référence, on peut l'enlever.
        {
            Entity spawnedEntity = entityCommandBuffer.Instantiate(spawnCubesConfig.cubePrefabEntity);

            entityCommandBuffer.SetComponent(spawnedEntity, new LocalTransform
            {
                Position = localTransform.ValueRO.Position, // Pour que la balle apparaisse à la position du Player.
                Rotation = quaternion.identity,
                Scale = 1f
            });

            // Cet event est écouté par PlayerShootManager, relié à sa fonction PlayerShoot.
            OnShoot?.Invoke(entity, EventArgs.Empty);

            // Pour déclencher ici une fonction de PlayerShootManager, on n'est pas obligés d'avoir recours à un event, on peut aussi faire de PlayerShootManager un Singleton.
            // On peut alors déclencher toutes ses fonctions publiques, par exemple :
            //PlayerShootManager.Instance.PlayerShoot(localTransform.ValueRO.Position);
        }

        entityCommandBuffer.Playback(EntityManager);    // Exécute les commandes du foreach après être sorti de la boucle.
    }
}
