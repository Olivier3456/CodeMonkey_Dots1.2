using Unity.Entities;
using Unity.Transforms;

// Ce composant nous permet � la fois de faire tourner et de d�placer nos cubes.
public partial struct HandleCubesSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        //foreach ((RefRW<LocalTransform> localTransform, RefRO<RotateSpeed> rotateSpeed, RefRO<Movement> movement) in
        //          SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateSpeed>, RefRO<Movement>>())
        //{
        //    localTransform.ValueRW = localTransform.ValueRO.RotateY(rotateSpeed.ValueRO.value * SystemAPI.Time.DeltaTime);
        //    localTransform.ValueRW = localTransform.ValueRO.Translate(movement.ValueRO.movementVector * SystemAPI.Time.DeltaTime);
        //};



        // Comme on peut le voir, c'est un peu le bazard, donc on cr�e un "aspect" pour regrouper tous ces param�tres : RotatingMovingCubeAspect, une struct qui impl�mente IAspect.
        // Il nous suffit ensuite de remplacer ce foreach par celui-ci :

        //foreach (RotatingMovingCubeAspect rotatingMovingCubeAspect in
        //         SystemAPI.Query<RotatingMovingCubeAspect>().WithAll<RotatingCube>())
        //{
        //    rotatingMovingCubeAspect.localTransform.ValueRW = rotatingMovingCubeAspect.localTransform.ValueRO.RotateY(rotatingMovingCubeAspect.rotateSpeed.ValueRO.value * SystemAPI.Time.DeltaTime);
        //    rotatingMovingCubeAspect.localTransform.ValueRW = rotatingMovingCubeAspect.localTransform.ValueRO.Translate(rotatingMovingCubeAspect.movement.ValueRO.movementVector * SystemAPI.Time.DeltaTime);
        //};



        // Mais on peut encore faire mieux et ajouter la logique � notre aspect.
        foreach (RotatingMovingCubeAspect rotatingMovingCubeAspect in
                 SystemAPI.Query<RotatingMovingCubeAspect>().WithAll<RotatingCube>())
        {
            rotatingMovingCubeAspect.MoveAndRotate(SystemAPI.Time.DeltaTime);
        };
    }
}
