using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour {

    private float volumeValue;
	// Use this for initialization
	void Start () {
        volumeValue = PlayerPrefs.GetFloat("Volume", 1);
	}
	
	// Update is called once per frame
	void Update () {
		if (volumeValue != PlayerPrefs.GetFloat("Volume, 1"))
        {
            volumeValue = PlayerPrefs.GetFloat("Volume", 1);
            ChangeVolume(volumeValue);
        }
        ChangeVolume(volumeValue);
    }

    private void ChangeVolume(float newValue)
    {
        float newVol = AudioListener.volume;
        newVol = newValue;
        AudioListener.volume = newVol;
    }
}
