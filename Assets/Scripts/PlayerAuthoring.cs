using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic); // Dynamic car on veut modifier le transform : on va le faire tourner.

            AddComponent(entity, new Player());
        }
    }
}


public struct Player : IComponentData
{

}
