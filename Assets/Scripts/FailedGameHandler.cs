using UnityEngine;
using UnityEngine.SceneManagement;

public class FailedGameHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void RestartGame()
    {
        Debug.Log("Clicked");
        SceneManager.LoadSceneAsync(1);
    }

    public void GiveUp()
    {
        Debug.Log("Clicked");
        SceneManager.LoadSceneAsync(2);
    }
}
