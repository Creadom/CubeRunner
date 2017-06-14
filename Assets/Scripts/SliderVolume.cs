using UnityEngine;
using UnityEngine.UI;

public class SliderVolume : MonoBehaviour {

    public Slider volumeSlider;
	// Use this for initialization
	void Start () {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void NewVolume()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }
}
