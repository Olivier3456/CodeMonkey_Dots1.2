using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

public partial struct RotatingCubeSystem : ISystem
{

    // On ne veut exécuter notre code que s'il y a au moins un Entity dans la scène avec le composant RotateSpeed.Sinon, cela peut créer des erreurs.
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RotateSpeed>();
    }


    [BurstCompile]  // Parce qu'on veut que ce soit exécuté avec le compiler Burst.
    public void OnUpdate(ref SystemState state)
    {

        // (On n'a plus besoin de cette fonction, car on utilise un Job (voir plus bas), qui fait la même chose :
        /*
        foreach ((RefRW<LocalTransform> localTransform, RefRO<RotateSpeed> rotateSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateSpeed>>().WithNone<Player>())
        //RefRO (Read Only) car ici nous ne faisons que lire la RotateSpeed. Si nous la modifiions, il faudrait mettre RefRW.
        //Mais attention, avec RefRW, on ne peut pas avoir plusieurs JOBS qui agisent sur cette data en même temps.
        // Pour simplifier, on peut aussi écrire : foreach (var rotateSpeed in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateSpeed>>())

        {
            localTransform.ValueRW = localTransform.ValueRO.RotateY(rotateSpeed.ValueRO.value * SystemAPI.Time.DeltaTime);
            // On utilise bien ici SystemAPI.Time.DeltaTime et non le simple Time.deltaTime d'Unity.
            // Attention aussi, avant le "=" on écrit la valeur, donc un utilise ValueRW,
            // mais après le "=", on lit seulement la valeur, donc on utilise ValueRO.
        }
        */

        RotatingCubeJob rotatingCubeJob = new RotatingCubeJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };


        //===>
        //rotatingCubeJob.Run(); // Pour sur le main thread, ce qui peut être utile pour debugger.
        
        //===>
        //state.Dependency = rotatingCubeJob.Schedule(state.Dependency); // S'exécute sur n'importe quel thread,
                                                                         // mais tout le job sera sur le même thread.
        // Le job peut se terminer sur la même frame comme sur celle d'après.
        // Pour forcer le programme à attendre que le job soit fini à cette frame (mais c'est à éviter) :
        // rotatingCubeJob.Schedule(state.Dependency).Complete();

        //===>
        rotatingCubeJob.ScheduleParallel(); // le job sera partagé en chunks qui s'exécuteront sur plusieurs threads.
                                            // Cela ne fonctionne que si on a au moins 128 entities de ce type dans la scène,
                                            // sinon ça ne fonctionne que comme un simple Schedule.
                                            // Si on veut faire des calculs lourds sur un nombre limité d'entities,
                                            // dans la création de notre job (ci-dessous), au lieu de le faire implémenter IJobEntity,
                                            // on lui fait implémenter IJobParallelFor.
    }



    // Création de notre Job RotatingCubeJob :

    [BurstCompile]
    [WithNone(typeof(Player))] // On ne veut pas que notre Player, qui a le composant RotateSpeed, tourne comme les autres cubes.
    // On aurait pu aussi créer un composant vide pour le Cube, par exemple RotatingCube, et un RotatingCubeAuthoring qu'on aurait attaché à notre cube, ce qui nous aurait
    // permis, pour ne faire tourner que nos cubes, de faire : [WithAll(typeof(RotatingCube))]
    public partial struct RotatingCubeJob : IJobEntity
    {
        public float deltaTime;

        public void Execute(ref LocalTransform localTransform, in RotateSpeed rotateSpeed)
            // Ce code s'exécute sur tout entity qui a ces deux composants.
            // Ici on utilise "ref" car on veut Read Write le localTransform,
            // puis on utilise "in" car on veut seulement lire la RotateSpeed.
        {
            localTransform = localTransform.RotateY(rotateSpeed.value * deltaTime);
        }
    }
}
