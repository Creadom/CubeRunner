using UnityEngine;
using UnityEngine.UI;

public class FailedScore : MonoBehaviour {

    public Text scoreText;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        scoreText.text = "Score : " + PlayerPrefs.GetInt("CurrentScore");
    }
}
