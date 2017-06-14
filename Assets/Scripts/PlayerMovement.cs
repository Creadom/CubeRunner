using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public Rigidbody rb;

    public float speed;
    public float mapWidth = 15f;
	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
    // Use FixedUpdate to mess with physics
	void FixedUpdate ()
    {
        float horizontalInput = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * speed;
        Vector3 newPosition = (rb.position + Vector3.right * horizontalInput);
        newPosition.x = Mathf.Clamp(newPosition.x, -mapWidth, mapWidth);
        rb.MovePosition(newPosition);

        if (rb.position.y < -1f)
        {
            FindObjectOfType<GameManager>().EndGame();
        }

        //ANDROID COPY-PASTA from SO check carefully
        if (Input.touchCount > 0)
        {
            Vector3 touchPosition = Input.GetTouch(0).position;
            touchPosition.z = rb.transform.position.z - Camera.main.transform.position.z;
            touchPosition = Camera.main.ScreenToWorldPoint(touchPosition);
            touchPosition.y = rb.transform.position.y;
            touchPosition.x = Mathf.Clamp(touchPosition.x, -mapWidth, mapWidth);
            rb.transform.position = Vector3.MoveTowards(rb.transform.position, touchPosition, Time.deltaTime * speed);

        }
    }
}
