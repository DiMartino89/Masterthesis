using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Camera variables
	private enum RotationAxis {
		mouse_x = 1,
		mouse_y = 2
	}
	
	private RotationAxis axis = RotationAxis.mouse_x;
	
	private float min_vertical = -45.0f;
	private float max_vertical = 45.0f;
	
	private float sens_horizontal = 5.0f;
	private float sens_vertical = 5.0f;
	
	private float rotation_x = 0;
	private float rotation_y = 0;
	
	/*
	 *	Function: 		CameraController
	 *	Description:	Spawn the player always on a random position at the bottom and set first person view
	 *	Date:			19.04.2018
	 *	Source:			https://www.youtube.com/watch?v=Ov9ekwAGhMA
	 *	Functionality:
	 *	1.	...
	 *	2.	...
	 */

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(axis == RotationAxis.mouse_x) {
			transform.Rotate(0, Input.GetAxis("Mouse X") * sens_horizontal, 0);
		} else if(axis == RotationAxis.mouse_y) {
			rotation_x -= Input.GetAxis("Mouse Y") * sens_vertical;
			rotation_x = Mathf.Clamp(rotation_x, min_vertical, max_vertical);
			rotation_y = transform.localEulerAngles.y;
			
			transform.localEulerAngles = new Vector3(rotation_x, rotation_y, 0);
		}
	}
}
