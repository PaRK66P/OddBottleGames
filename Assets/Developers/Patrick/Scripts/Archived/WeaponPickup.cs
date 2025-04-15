using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player Collision
        if (collision.gameObject.layer == 7)
        {
            // Pickup weapon and destroy the pickup
            //collision.gameObject.GetComponent<PlayerManager>().RegainWeapon();
            Destroy(this.gameObject);
        }
    }
}
