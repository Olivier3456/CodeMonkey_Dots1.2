using UnityEngine;

// Il est très important de savoir faire cohabiter des GameObjects normaux avec des composants DOTS/ECS.
// Ceci est un script normal, attaché à un GameObject normal (un prefab).

public class ShootPopup : MonoBehaviour
{
    private float destroyTimer = 1f;

    private void Update()
    {
        float moveSpeed = 2f;
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        destroyTimer -= Time.deltaTime;
        if (destroyTimer <= 0f )
        {
            Destroy(gameObject);
        }
    }
}
