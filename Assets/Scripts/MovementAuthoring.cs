using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MovementAuthoring : MonoBehaviour
{
    public class Baker : Baker<MovementAuthoring>
    {
        public override void Bake(MovementAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic); // Dynamic car on veut modifier le transform : on va le faire tourner.

            AddComponent(entity, new Movement
            {
                movementVector = new float3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f))
            });
        }
    }
}



public struct Movement : IComponentData
{
    public float3 movementVector;


}