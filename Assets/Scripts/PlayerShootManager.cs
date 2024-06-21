using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

// Ce script aussi est attach� � un game object normal, dans la sc�ne et non dans la subscene.
// Comme on peut le voir, il communique avec un script DOTS, PlayerShootingSystem, par l'interm�diaire d'un event.

public class PlayerShootManager : MonoBehaviour
{
    [SerializeField] private GameObject shootPopupPrefab;

    public static PlayerShootManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        PlayerShootingSystem playerShootingSystem =
            World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerShootingSystem>(); // On r�cup�re notre PlayerShootingSystem (il ne peut y avoir qu'un System de chaque type).

        playerShootingSystem.OnShoot += PlayerShootingSystem_OnShoot;
    }

    private void PlayerShootingSystem_OnShoot(object sender, System.EventArgs e)
    {
        Entity plyerEntity = (Entity)sender;

        // On r�cup�re le transform du Player qui a tir� :
        LocalTransform localTransform = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<LocalTransform>(plyerEntity);

        // On instancie notre prefab comme dans n'importe quel MonoBehaviour, et � la position du joueur qui a tir� :
        Instantiate(shootPopupPrefab, localTransform.Position, Quaternion.identity);
    }


    // Si on fait de cette classe un singleton, on peut aussi appeler ses fonctions publiques directement depuis PlayerShootingSystem, par exemple pour instancier notre prefab ShootPopup :
    //public void PlayerShoot(Vector3 playerPosition)
    //{
    //    Instantiate(shootPopupPrefab, playerPosition, Quaternion.identity);
    //}
}
