using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMovement : MonoBehaviour {

    public Rigidbody rb;
    public int waveDestroyed = 0;
    public float forwardForce;
    // Use this for initialization
    void Update ()
    {
		if (transform.position.y < -2f)
        {
            Destroy(gameObject);
            waveDestroyed++;

        }
    }

    public int getNumberOfWaves()
    {
        return waveDestroyed;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        rb.AddForce(0, 0, -forwardForce * Time.deltaTime);
    }
}
