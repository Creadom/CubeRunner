using UnityEngine;

public class PlayerCollision : MonoBehaviour {

    public PlayerMovement movement;
    public GameManager gm;
    public int score;
	// Use this for initialization
	void Start () {
        score = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // OnCollision is called everytime 2 objects collide
    void OnCollisionEnter(Collision collisionInfo)
    {
        while (collisionInfo.collider.tag != "Obstacle")
        {

        }
        movement.enabled = false;
        gm.EndGame();
    }
}
