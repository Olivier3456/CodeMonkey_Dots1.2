using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// Il vaut mieux �viter aussi d'ajouter un composant � un entity en runtime, car cela peut cr�er des probl�mes :
// c'est un changement structurel, comme quand on spawne un entity.
// Il vaut donc mieux utiliser un IEnableableComponent, que l'on va donc ajouter dans la m�thode Bake du Baker,
// et ensuite on pourra l'activer ou le d�sactiver quand on veut, ce qui n'est pas un changement structurel.
// (Pour que �a fonctionne, il faut bien s�r aussi ajouter ce composant StunnedAuthoring � notre Player).
public class StunnedAuthoring : MonoBehaviour
{
    public class Baker : Baker<StunnedAuthoring>
    {
        public override void Bake(StunnedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Stunned());
            SetComponentEnabled<Stunned>(entity, false);    // Ici, on met false car on ne veut pas que notre player soit Stunned au d�part.
                                                            // Voir dans PlayerShootingSystem : c'est l� qu'on enable notre component.
        }
    }
}

public struct Stunned : IComponentData, IEnableableComponent
{

}
