using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Vector2 vel = new UnityEngine.Vector2(10 * Time.deltaTime, 0);
        rb.AddForce(vel);
    }
}
