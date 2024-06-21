using Unity.Entities;
using UnityEngine;

// Composant à attacher à nos cubes
public class RotatingCubeAuthoring : MonoBehaviour
{
    private class Baker : Baker<RotatingCubeAuthoring>
    {
        public override void Bake(RotatingCubeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new RotatingCube());
        }
    }
}

// Ce composant vide va juste nous servir de tag pour identifier nos cubes et les inclure ou les exclure de certaines logiques,
// avec [WithAll(typeof(RotatingCube))] (voir dans RotatingCubeSystem).
public struct RotatingCube : IComponentData
{

}
