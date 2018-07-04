using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingLight : MonoBehaviour {

	public Light light;
	public AudioClip flicker;
	
	private AudioSource audio;
	
	void Start() {
		audio = GetComponent<AudioSource>();
	}
	
	/*
	 *	Function: 		FlashingLight
	 *	Description:	Make random lights flicker
	 *	Date:			19.04.2018
	 *	Source:			https://www.youtube.com/watch?v=ZqVu27oY__U
	 *	Functionality:
	 *	1.	...
	 *	2.	...
	 */
	
	void FixedUpdate() {
		int random = Random.Range(0, 10);
		
		if(random < 7) {
			light.enabled = true;
			if(!audio.isPlaying) {
				audio.PlayOneShot(flicker, 1.0f);
				audio.enabled = true;
			}
		} else {
			light.enabled = false;
			audio.enabled = false;
		}
	}
}
