using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Player variables
	public GameObject flashLight;
	public AudioClip running;
	public AudioClip light;
	
	private float speed = 4.0f;
	private CharacterController controller;
	private AudioSource audio;
	private int health = 100;
	
	//public float speed = 3.0F;
    //public float rotateSpeed = 3.0F;
	
	void Start () {
		controller = GetComponent<CharacterController>();
		audio = GetComponent<AudioSource>();
	}
	
	void Update () {
		
		controller = GetComponent<CharacterController>();
		
		// Movement and Look
		float delta_x = Input.GetAxis("Horizontal") * speed;
		float delta_z = Input.GetAxis("Vertical") * speed;
		Vector3 movement = new Vector3(delta_x, 0, delta_z);
		
		//movement *= Time.deltaTime;
		movement = transform.TransformDirection(movement);
		controller.SimpleMove(movement);
		
		// Running
		if(Input.GetButton("Horizontal") || Input.GetButton("Vertical")) {
			if(!audio.isPlaying) {
				float volume = Random.Range(0.1f, 0.5f);
				audio.pitch = 1.0f;
				audio.PlayOneShot(running, volume);
				audio.enabled = true;
				audio.loop = false;
			}
		}
		
		// CodingBox
		if (Input.GetKeyDown(KeyCode.Space)) {
			
			GameObject codeBox_panel = GameObject.Find("CodingBoxPanel");
			BoxSlider box_slider = codeBox_panel.GetComponent<BoxSlider>();
			
			box_slider.toggleBox();
		}
		
		// Flashlight
		if (Input.GetKeyDown(KeyCode.L)) {
			if(!flashLight.activeSelf) {
				flashLight.SetActive(true);
				audio.pitch = 1.0f;
				audio.PlayOneShot(light, 1.0f);
			} else {
				flashLight.SetActive(false);
				audio.pitch = 1.0f;
				audio.PlayOneShot(light, 1.0f);
			}
		} 
	}
}
