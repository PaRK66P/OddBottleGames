using UnityEngine;

public class TriggerEnablers : MonoBehaviour
{
    [SerializeField]
    GameObject[] _enableObjects;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player Collision
        if(collision.gameObject.tag == "Player")
        {
            // Enable all the objects
            foreach(GameObject obj in _enableObjects)
            {
                obj.SetActive(true);
            }

            // Prevent multiple calls
            Destroy(gameObject);
        }
    }
}
