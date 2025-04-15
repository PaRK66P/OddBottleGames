using UnityEngine;

public class CompanionDashRechargeZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) // Might be better as TriggerStay
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerManager>().GainDashCharges();
        }
    }
}
