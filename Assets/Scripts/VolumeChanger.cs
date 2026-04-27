using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class VolumeChanger : MonoBehaviour
{
	public AudioMixer MusicMixer; 
	public TMP_Text MusicValue;
	float MusicFloat;
	float MusicVol;
	public Slider MusicSlider;

	void Start()
	{
		MusicVol = PlayerPrefs.GetFloat("MusicVolume", 80);
		MusicMixer.SetFloat("Music", MusicVol * 0.8f - 80);
		MusicSlider.value = MusicVol;
		MusicFloat = MusicVol;
		MusicValue.text = MusicFloat.ToString();
	}

	public void MusicVolumeChange(float MusicVolume)
	{
		PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
		MusicMixer.SetFloat("Music", MusicVolume * 0.8f - 80);
		MusicFloat = MusicVolume;
		MusicValue.text = MusicFloat.ToString();
	}
}
