using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// Il vaut mieux éviter aussi d'ajouter un composant à un entity en runtime, car cela peut créer des problèmes :
// c'est un changement structurel, comme quand on spawne un entity.
// Il vaut donc mieux utiliser un IEnableableComponent, que l'on va donc ajouter dans la méthode Bake du Baker,
// et ensuite on pourra l'activer ou le désactiver quand on veut, ce qui n'est pas un changement structurel.
// (Pour que ça fonctionne, il faut bien sûr aussi ajouter ce composant StunnedAuthoring à notre Player).
public class StunnedAuthoring : MonoBehaviour
{
    public class Baker : Baker<StunnedAuthoring>
    {
        public override void Bake(StunnedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Stunned());
            SetComponentEnabled<Stunned>(entity, false);    // Ici, on met false car on ne veut pas que notre player soit Stunned au départ.
                                                            // Voir dans PlayerShootingSystem : c'est là qu'on enable notre component.
        }
    }
}

public struct Stunned : IComponentData, IEnableableComponent
{

}
