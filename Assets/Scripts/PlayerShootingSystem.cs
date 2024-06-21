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
            EntityManager.SetComponentEnabled<Stunned>(playerEntity, false);    // Ici nous d�-stunnons notre player.
        }
        // On peut voir les tags du player dans son Inspector, en mode Runtime.


        if (!Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }


        SpawnCubesConfig spawnCubesConfig = SystemAPI.GetSingleton<SpawnCubesConfig>();

        // En ajoutant une entity en runtime, on risque de casser des choses, car la boucle foreach ci-dessous it�re dans la collection m�me o� on ajoute nos entities.
        // C'est pourquoi il vaut mieux instancier les entities avec un EntityCommandBuffer :

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(WorldUpdateAllocator);    // WorldUpdateAllocator fait gagner un peu en performances.

        foreach ((RefRO<LocalTransform> localTransform, Entity entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Player>().WithDisabled<Stunned>().WithEntityAccess())
        // Fait spawner un cube pour chaque Player dans la sc�ne, � condition que le Player ne soit pas Stunned (voir StunnedAuthoring).
        // Si au contraire on veut que l'action ne se d�clenche que si le composant Stunned est Enabled, comme il n'existe pas de "WithEnabled", donc il faut juste ajouter Stunned dans le WithAll, � la suite de Player. 
        // "Entity entity" nous sert � r�cup�rer une r�f�rence de l'entity concern�, ce qui nous permet de renseigner quelle entity d�clenche l'event OnShoot, quand on l'invoke (voir ci-dessous).
        // Ici, cela peut �tre utile s'il y a plusieurs Players dans la sc�ne, et qu'on veut savoir lequel a tir�.
        // WithEntityAccess(), � la fin, est ce qui permet de r�cup�rer cette r�f�rence � l'entity. Si on ne veut pas r�cup�rer cette r�f�rence, on peut l'enlever.
        {
            Entity spawnedEntity = entityCommandBuffer.Instantiate(spawnCubesConfig.cubePrefabEntity);

            entityCommandBuffer.SetComponent(spawnedEntity, new LocalTransform
            {
                Position = localTransform.ValueRO.Position, // Pour que la balle apparaisse � la position du Player.
                Rotation = quaternion.identity,
                Scale = 1f
            });

            // Cet event est �cout� par PlayerShootManager, reli� � sa fonction PlayerShoot.
            OnShoot?.Invoke(entity, EventArgs.Empty);

            // Pour d�clencher ici une fonction de PlayerShootManager, on n'est pas oblig�s d'avoir recours � un event, on peut aussi faire de PlayerShootManager un Singleton.
            // On peut alors d�clencher toutes ses fonctions publiques, par exemple :
            //PlayerShootManager.Instance.PlayerShoot(localTransform.ValueRO.Position);
        }

        entityCommandBuffer.Playback(EntityManager);    // Ex�cute les commandes du foreach apr�s �tre sorti de la boucle.
    }
}
