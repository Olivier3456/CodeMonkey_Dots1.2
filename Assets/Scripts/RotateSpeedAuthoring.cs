using Unity.Entities;
using UnityEngine;


// Composant à attacher à nos cubes, qu'on veut faire tourner.
public class RotateSpeedAuthoring : MonoBehaviour
{
    public float value;


    private class Baker : Baker<RotateSpeedAuthoring>
    {
        public override void Bake(RotateSpeedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            // Dynamic car on veut modifier le transform : on va le faire tourner.

            AddComponent(entity, new RotateSpeed { value = authoring.value });
        }
    }
}


// Ce compent data ne contient qu'une seule donnée, la vitesse de rotation.
public struct RotateSpeed : IComponentData
{
    public float value;
}

