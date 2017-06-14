using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject pausedPanel;

    public void EndGame()
    {
        // Gotta restart
        SceneManager.LoadScene(3);

    }

    public void PauseGame()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            Debug.Log("Pause");
            pausedPanel.SetActive(true);
        }
        else if (Time.timeScale == 0)
        {
            Debug.Log("high");
            Time.timeScale = 1;
        }
    }

    public void ResumeGame()
    {
        if (Time.timeScale == 0)
        {
            pausedPanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

}
