using UnityEngine;

public class Credits : MonoBehaviour {

	public void Quit()
    {
        PlayerPrefs.Save();
        Application.Quit();
        Debug.Log("Quit");
    }
}
