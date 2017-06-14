using UnityEngine;
using UnityEngine.UI;

public class IntroScore : MonoBehaviour {

    public Text highScoreText;
    private int highScore;
	// Use this for initialization
	void Start () {
        highScore = PlayerPrefs.GetInt("HighScore");
	}
	
	// Update is called once per frame
	void Update () {
        if (highScore <= 0)
        {
            highScoreText.text = "";
        }
        else
        {
            highScoreText.text = "Best : " + highScore;
        }
       
	}
}
