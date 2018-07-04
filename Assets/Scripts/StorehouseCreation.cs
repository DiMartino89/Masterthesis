using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorehouseCreation : MonoBehaviour {

	public GameObject container;

	private GameObject container_holder;
	private float wall_length = 2.0f;
	private int storehouse_width = 6;
	private int storehouse_height = 7;
	private int max_items = 2;
	private Vector3 initial_position;

	// Use this for initialization
	void Start () {
		CreateGrounds();
	}
	
	void CreateGrounds() {
		initial_position = new Vector3((-storehouse_width / 2) + wall_length / 2, 0.0f, (-storehouse_height / 2) + wall_length / 2);
		Vector3 position = initial_position;
		GameObject container_temporary;
		
		container_holder = new GameObject();
		container_holder.name = "StorageArea_Containers";
		container_holder.transform.parent = GameObject.Find("StorageArea").transform;
		
		for(int i = 0; i < storehouse_width; i++) {
			for(int j = 0; j < storehouse_height; j++) {
				position = new Vector3(initial_position.x + (j * wall_length), 0.0f, initial_position.z + (i * wall_length));			
				
				int set_container = Random.Range(0, 10);
				if(set_container > 1) {
					container_temporary = GameObject.Instantiate(container, new Vector3(position.x, position.y, position.z - 1.0f), Quaternion.Euler(0.0f,0.0f,0.0f)) as GameObject;
					container_temporary.transform.parent = container_holder.transform;
				}
			}
		}
		
		container_holder.transform.position = new Vector3(8.0f, 0.0f, -23.0f);
		container_holder.transform.rotation = Quaternion.Euler(0.0f,90.0f,0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
