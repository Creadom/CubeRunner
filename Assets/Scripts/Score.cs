using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    public Text scoreText;
    public BlockSpawner BlockSpawner;
    public int score;
	// Use this for initialization
	void Start ()
    {

	}
    string GetScore()
    {
        score = BlockSpawner.GetNumberOfWaves();
        if (score >= 1)
        {
            return "Score : " + score;
        }
        else
        {
            return "";
        }
    }
	// Update is called once per frame
	void Update () {
        
        scoreText.text = GetScore();
        PlayerPrefs.SetInt("CurrentScore", score);
    }
}
