using UnityEngine;
using UnityEngine.UI;

public class FailedHighScore : MonoBehaviour {

    public Text highScoreText;
    private int highScore;
    private int currentScore;
	// Use this for initialization
	void Start () {
        highScore = PlayerPrefs.GetInt("HighScore");
        currentScore = PlayerPrefs.GetInt("CurrentScore");

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", currentScore);
        }
        
	}
	
	// Update is called once per frame
	void Update () {
        highScoreText.text = "Best : " + PlayerPrefs.GetInt("HighScore");
    }
}
