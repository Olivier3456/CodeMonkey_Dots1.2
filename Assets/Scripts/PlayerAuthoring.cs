using Unity.Entities;
using UnityEngine;

// Composant à attacher à notre objet Player.
public class PlayerAuthoring : MonoBehaviour
{
    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Player());
        }
    }
}


public struct Player : IComponentData
{

}
