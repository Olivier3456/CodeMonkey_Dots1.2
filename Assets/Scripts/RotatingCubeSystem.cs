using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

public partial struct RotatingCubeSystem : ISystem
{

    // On ne veut ex�cuter notre code que s'il y a au moins un Entity dans la sc�ne avec le composant RotateSpeed.Sinon, cela peut cr�er des erreurs.
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RotateSpeed>();
    }


    [BurstCompile]  // Parce qu'on veut que ce soit ex�cut� avec le compiler Burst.
    public void OnUpdate(ref SystemState state)
    {

        // (On n'a plus besoin de cette fonction, car on utilise un Job (voir plus bas), qui fait la m�me chose :
        /*
        foreach ((RefRW<LocalTransform> localTransform, RefRO<RotateSpeed> rotateSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateSpeed>>().WithNone<Player>())
        //RefRO (Read Only) car ici nous ne faisons que lire la RotateSpeed. Si nous la modifiions, il faudrait mettre RefRW.
        //Mais attention, avec RefRW, on ne peut pas avoir plusieurs JOBS qui agisent sur cette data en m�me temps.
        // Pour simplifier, on peut aussi �crire : foreach (var rotateSpeed in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateSpeed>>())

        {
            localTransform.ValueRW = localTransform.ValueRO.RotateY(rotateSpeed.ValueRO.value * SystemAPI.Time.DeltaTime);
            // On utilise bien ici SystemAPI.Time.DeltaTime et non le simple Time.deltaTime d'Unity.
            // Attention aussi, avant le "=" on �crit la valeur, donc un utilise ValueRW,
            // mais apr�s le "=", on lit seulement la valeur, donc on utilise ValueRO.
        }
        */

        RotatingCubeJob rotatingCubeJob = new RotatingCubeJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };


        //===>
        //rotatingCubeJob.Run(); // Pour sur le main thread, ce qui peut �tre utile pour debugger.
        
        //===>
        //state.Dependency = rotatingCubeJob.Schedule(state.Dependency); // S'ex�cute sur n'importe quel thread,
                                                                         // mais tout le job sera sur le m�me thread.
        // Le job peut se terminer sur la m�me frame comme sur celle d'apr�s.
        // Pour forcer le programme � attendre que le job soit fini � cette frame (mais c'est � �viter) :
        // rotatingCubeJob.Schedule(state.Dependency).Complete();

        //===>
        rotatingCubeJob.ScheduleParallel(); // le job sera partag� en chunks qui s'ex�cuteront sur plusieurs threads.
                                            // Cela ne fonctionne que si on a au moins 128 entities de ce type dans la sc�ne,
                                            // sinon �a ne fonctionne que comme un simple Schedule.
                                            // Si on veut faire des calculs lourds sur un nombre limit� d'entities,
                                            // dans la cr�ation de notre job (ci-dessous), au lieu de le faire impl�menter IJobEntity,
                                            // on lui fait impl�menter IJobParallelFor.
    }



    // Cr�ation de notre Job RotatingCubeJob :

    [BurstCompile]
    [WithNone(typeof(Player))] // On ne veut pas que notre Player, qui a le composant RotateSpeed, tourne comme les autres cubes.
    // On aurait pu aussi cr�er un composant vide pour le Cube, par exemple RotatingCube, et un RotatingCubeAuthoring qu'on aurait attach� � notre cube, ce qui nous aurait
    // permis, pour ne faire tourner que nos cubes, de faire : [WithAll(typeof(RotatingCube))]
    public partial struct RotatingCubeJob : IJobEntity
    {
        public float deltaTime;

        public void Execute(ref LocalTransform localTransform, in RotateSpeed rotateSpeed)
            // Ce code s'ex�cute sur tout entity qui a ces deux composants.
            // Ici on utilise "ref" car on veut Read Write le localTransform,
            // puis on utilise "in" car on veut seulement lire la RotateSpeed.
        {
            localTransform = localTransform.RotateY(rotateSpeed.value * deltaTime);
        }
    }
}
