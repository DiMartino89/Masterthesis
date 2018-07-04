/* ============================================================ *
 *	Project:	Masterthesis 2018								*
 *	File:		MazeCreation									*
 *	Author: 	Martin Boy										*
 *	Source:		Flattutorials									*
 * ============================================================ */

using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class MazeCreation : MonoBehaviour {
	
	[System.Serializable]
	public class Cell {
	
		public bool visited;
		public GameObject north;
		public GameObject east;
		public GameObject west;
		public GameObject south;
	}

	// Variables for creating a solvable maze
	
	public GameObject wall;
	public GameObject start;
	public GameObject finish;
	public GameObject ground;
	public GameObject ceiling_01;
	public GameObject ceiling_02;
	public GameObject opponent;
	public GameObject task_01;
	public GameObject task_02;
	public float wall_length = 2.0f;
	public int maze_width = 16;
	public int maze_height = 8;
	public int max_opponents = 2;
	public int max_tasks = 2;
	
	private int opponent_counter = 0;
	private int task_counter = 0;
	private int cell_current = 0;
	private int cells_total = 0;
	
	private GameObject maze_holder;
	private GameObject wall_holder;
	private GameObject ground_holder;
	private GameObject ceiling_holder;
	private GameObject opponent_holder;
	private GameObject task_holder;
	private GameObject door_holder;
	private Cell[] cells;
	private Vector3 initial_position;
	private Vector3 finish_position;
	private int cells_visited = 0;
	private bool start_mazebuild = false;
	private int neighbor_current = 0;
	private List<int> cells_last;
	private int backup = 0;
	private int wall_to_break = 0;

	// Cell-Object for the single cells of the maze
	
	void Start() {
		
		cells_total = maze_width * maze_height;
		
		// Init all holders for objects
		maze_holder = GameObject.Find("Catacombs");
		
		wall_holder = new GameObject();
		wall_holder.name = "Catacombs_Walls";
		
		door_holder = new GameObject();
		door_holder.name = "Catacombs_Doors";
		
		ground_holder = new GameObject();
		ground_holder.name = "Catacombs_Grounds";
		
		ceiling_holder = new GameObject();
		ceiling_holder.name = "Catacombs_Ceilings";
		
		opponent_holder = new GameObject();
		opponent_holder.name = "Catacombs_Opponents";
		
		task_holder = new GameObject();
		task_holder.name = "Catacombs_Tasks";
		
		AttachMazeComponents();
		CreateAllWalls();
		SetMazePosition();
		
	}
	
	/*
	 *	Function: 		CreateWalls()
	 *	Description:	Function to create the basic wall-grid for the maze
	 *	Date:			18.04.2018
	 *	Source:			https://www.youtube.com/watch?v=OzENv_ZRA1g
	 *	Functionality:
	 *	1.	...
	 *	2.	...
	 */
	 
	void CreateAllWalls() {
		
		initial_position = new Vector3((-maze_width / 2) + wall_length / 2, 0.0f, (-maze_height / 2) + wall_length / 2);
		Vector3 position = initial_position;
		GameObject wall_temporary;
		GameObject door_temporary;
		GameObject ground_temporary;
		GameObject ceiling_temporary;
		GameObject opponent_temporary;
		
		// Create walls along y-Axis
		
		for(int i = 0; i < maze_height; i++) {
			for(int j = 0; j <= maze_width; j++) {
				position = new Vector3(initial_position.x + (j * wall_length) - wall_length / 2, 1.0f, initial_position.z + (i * wall_length));
				if(j == maze_width) {
					wall_temporary = GameObject.Instantiate(wall, position, Quaternion.Euler(90.0f,270.0f,0.0f)) as GameObject;
					wall_temporary.transform.parent = wall_holder.transform;
				} else {
					wall_temporary = GameObject.Instantiate(wall, position, Quaternion.Euler(90.0f,90.0f,0.0f)) as GameObject;
					wall_temporary.transform.parent = wall_holder.transform;
				}
			}
		}

		// Create walls along x-Axis
		
		for(int i = 0; i <= maze_height; i++) {
			for(int j = 0; j < maze_width; j++) {
				position = new Vector3(initial_position.x + (j * wall_length), 1.0f, initial_position.z + (i * wall_length) - wall_length / 2);
				if(i == maze_height) {
					wall_temporary = GameObject.Instantiate(wall, position, Quaternion.Euler(90.0f,180.0f,0.0f)) as GameObject;
					wall_temporary.transform.parent = wall_holder.transform;
				} else {
					wall_temporary = GameObject.Instantiate(wall, position, Quaternion.Euler(90.0f,0.0f,0.0f)) as GameObject;
					wall_temporary.transform.parent = wall_holder.transform;
				}
			}
		}
		
		// Create Ground and Ceilings
		
		for(int i = 0; i < maze_height; i++) {
			for(int j = 0; j < maze_width; j++) {
				position = new Vector3(initial_position.x + (j * wall_length), 0.0f, initial_position.z + (i * wall_length));			
				
				// Ground
				ground_temporary = GameObject.Instantiate(ground, new Vector3(position.x, position.y, position.z), Quaternion.Euler(0.0f,0.0f,0.0f)) as GameObject;
				ground_temporary.transform.parent = ground_holder.transform;
				int opponent_chance = Random.Range(0, 20);
				if(opponent_chance < 1 && opponent_counter < max_opponents) {
					opponent_counter++;
					opponent_temporary = GameObject.Instantiate(opponent, new Vector3(position.x, position.y, position.z), Quaternion.identity) as GameObject;
					opponent_temporary.transform.parent = opponent_holder.transform;
				}
				
				// Ceilings
				int randomCeiling = Random.Range(0, 10);
				if(randomCeiling < 8) {
					ceiling_temporary = GameObject.Instantiate(ceiling_01, new Vector3(position.x, position.y + 2.0f, position.z), Quaternion.Euler(180.0f,0.0f,0.0f)) as GameObject;
					ceiling_temporary.transform.parent = ceiling_holder.transform;
				} else {
					int flashLight = Random.Range(0, 10);
					if(flashLight < 2) {
						ceiling_temporary = GameObject.Instantiate(ceiling_02, new Vector3(position.x, position.y + 2.0f, position.z), Quaternion.Euler(180.0f,0.0f,0.0f)) as GameObject;
						FlashingLight flashingLight = ceiling_temporary.GetComponentInChildren<FlashingLight>();
						flashingLight.enabled = true;
						ceiling_temporary.transform.parent = ceiling_holder.transform;
					} else {
						ceiling_temporary = GameObject.Instantiate(ceiling_02, new Vector3(position.x, position.y + 2.0f, position.z), Quaternion.Euler(180.0f,0.0f,0.0f)) as GameObject;
						ceiling_temporary.transform.parent = ceiling_holder.transform;
					}
				}
			}
		}
		
		// Set the start door
		Transform start_ceiling = ceiling_holder.transform.GetChild(0);
		Vector3 start_pos = start_ceiling.position;
		Destroy(start_ceiling.gameObject);
		
		//door_temporary = GameObject.Instantiate(start, new Vector3(start_pos.x, start_pos.y - 1.0f, start_pos.z), Quaternion.Euler(0, start_wall.transform.eulerAngles.y, 0)) as GameObject;
		//door_temporary.transform.parent = door_holder.transform;
		
		// Set the finish door
		Transform finish_ceiling = ceiling_holder.transform.GetChild(47);
		Vector3 finish_pos = finish_ceiling.position;
		Destroy(finish_ceiling.gameObject);
		
		//door_temporary = GameObject.Instantiate(finish, new Vector3(finish_pos.x, finish_pos.y - 1.0f, finish_pos.z), Quaternion.Euler(0, finish_wall.transform.eulerAngles.y, 0)) as GameObject;
		//door_temporary.transform.parent = door_holder.transform;
		
		CreateAllCells();
	}
	
	/*
	 *	Function: 		CreateAllCells()
	 *	Description:	Function to create the cells within the grid and assign its walls
	 *	Date:			18.04.2018	
	 *	Source:			https://www.youtube.com/watch?v=r4fQzkPzNx4
	 *	Functionality:
	 *	1.	...
	 *	2.	...
	 */
	
	void CreateAllCells() {
		
		GameObject[] all_walls;
		int children = wall_holder.transform.childCount;
		all_walls = new GameObject[children];
		cells = new Cell[cells_total];
		int east_to_west = 0;
		int child_process = 0;
		int east_to_west_count = 0;
		cells_last = new List<int>();
		cells_last.Clear();
		
		// Gets all children
		
		for(int i = 0; i < children; i++) {
			all_walls[i] = wall_holder.transform.GetChild(i).gameObject;
		}
		
		// Assign walls to cells
		
		for(int cell = 0; cell < cells.Length; cell++) {
			cells[cell] = new Cell();
			
			// East-Wall
			cells[cell].east = all_walls[east_to_west];
			// South-Wall
			cells[cell].south = all_walls[child_process + (maze_width + 1) * maze_height];
			
			// When max-width (west) is reached, go to next cell-line (up) and switch back to east
			if(east_to_west_count == maze_width) {
				east_to_west += 2;
				east_to_west_count = 0;
			} else {
				east_to_west++;
			}
			
			east_to_west_count++;
			child_process++;
			
			// West-Wall
			cells[cell].west = all_walls[east_to_west];
			// North-Wall
			cells[cell].north = all_walls[(child_process + (maze_width + 1) * maze_height) + maze_width - 1];
		}
		
		ApplyDepthFirst();
	}
	
	/*
	 *	Function: 		ApplyDepthFirst()
	 *	Description:	Create a maze based on the depth-first-Function
	 *	Date:			19.04.2018
	 *	Source:			https://www.youtube.com/watch?v=z7wHZMB9YYs&t=1372s
	 *	Functionality:
	 *	1.	...
	 *	2.	...
	 */
	
	void ApplyDepthFirst() {
		while(cells_visited < cells_total) {
			// Check if first wheel chosen or not
			if(start_mazebuild) {
				FindNeighbors();
				if(cells[neighbor_current].visited == false && cells[cell_current].visited == true) {
					BreakWall();
					cells[neighbor_current].visited = true;
					cells_visited++;
					cells_last.Add(cell_current);
					cell_current = neighbor_current;
					if(cells_last.Count > 0) {
						backup = cells_last.Count - 1;
					}
				}
				
			} else {
				cell_current = Random.Range(0, cells_total);
				cells[cell_current].visited = true;
				cells_visited++;
				start_mazebuild = true;
			}
		}
		
		SetTasks();
	}
	
	/*
	 *	Function: 		BreakWall()
	 *	Description:	Break Walls when there is a path
	 *	Date:			19.04.2018
	 *	Source:			https://www.youtube.com/watch?v=z7wHZMB9YYs&t=1372s
	 *	Functionality:
	 *	1.	...
	 *	2.	...
	 */
	
	void BreakWall() {
		switch(wall_to_break) {
			case 1: cells[cell_current].north.transform.parent = null; Destroy(cells[cell_current].north); break;
			case 2: cells[cell_current].east.transform.parent = null; Destroy(cells[cell_current].east); break; 
			case 3: cells[cell_current].west.transform.parent = null; Destroy(cells[cell_current].west); break; 
			case 4: cells[cell_current].south.transform.parent = null; Destroy(cells[cell_current].south); break; 			
		}
	}
	
	/*
	 *	Function: 		FindNeighbors()
	 *	Description:	Function to find neighbors for a maze-cell
	 *	Date:			19.04.2018
	 *	Source:			https://www.youtube.com/watch?v=57RHhUXOA60
	 *	Functionality:
	 *	1.	...
	 *	2.	...
	 */
	
	void FindNeighbors() {
		int length = 0;
		int[] neighbors = new int[4];
		int[] wall_connecting = new int[4];
		int check = 0;
		
		// Check if West is reached as there is no longer a neigbor
		check = ((cell_current + 1) / maze_width);
		check -= 1;
		check *= maze_width;
		check += maze_width;
		
		// North-Wall
		if((cell_current + maze_width) < cells_total) {
			if(cells[cell_current + maze_width].visited == false) {
				neighbors[length] = cell_current + maze_width;
				wall_connecting[length] = 1;
				length++;
			}
		}
		
		// East-Wall
		if((cell_current - 1) >= 0 && cell_current != check) {
			if(cells[cell_current - 1].visited == false) {
				neighbors[length] = cell_current - 1;
				wall_connecting[length] = 2;
				length++;
			}
		}
		
		
		// West-Wall
		if((cell_current + 1) < cells_total && (cell_current + 1) != check) {
			if(cells[cell_current + 1].visited == false) {
				neighbors[length] = cell_current + 1;
				wall_connecting[length] = 3;
				length++;
			}
		}
		
		// South-Wall
		if((cell_current - maze_width) >= 0) {
			if(cells[cell_current - maze_width].visited == false) {
				neighbors[length] = cell_current - maze_width;
				wall_connecting[length] = 4;
				length++;
			}
		}
		
		// Check if there is a neighbor
		if(length != 0) {
			int  neighbor_chosen = Random.Range(0, length);
			neighbor_current = neighbors[neighbor_chosen];
			wall_to_break = wall_connecting[neighbor_chosen];
		} else {
			if(backup > 0) {
				cell_current = cells_last[backup];
				backup--;
			}
		}
	}
	
	/*
	 *	Function: 		SetTasks()
	 *	Description:	Function to set the tasks in maze
	 *	Date:			19.04.2018
	 *	Functionality:
	 *	1.	...
	 *	2.	...
	 */
	
	void SetTasks() {
		
		GameObject task_temporary;
		
		for(int i = 0; i < max_tasks; i++) {
			
			GameObject walls = GameObject.Find("Catacombs_Walls");
			
			int index = Random.Range(0, walls.transform.childCount);
			
			Transform child = walls.transform.GetChild(index);
			
			Vector3 position = child.position;
			
			Destroy(child.gameObject);
			
			if(i == 0) {
				task_temporary = GameObject.Instantiate(task_01, new Vector3(position.x, position.y, position.z), Quaternion.Euler(90.0f, child.transform.eulerAngles.y, 0)) as GameObject;
				task_temporary.transform.parent = wall_holder.transform;
			}
			if(i == 1) {
				task_temporary = GameObject.Instantiate(task_02, new Vector3(position.x, position.y, position.z), Quaternion.Euler(90.0f, child.transform.eulerAngles.y, 0)) as GameObject;
				task_temporary.transform.parent = wall_holder.transform;
			}
		}
	}
	
	void AttachMazeComponents() {
		ground_holder.transform.parent = maze_holder.transform;
		ceiling_holder.transform.parent = maze_holder.transform;
		wall_holder.transform.parent = maze_holder.transform;
		door_holder.transform.parent = maze_holder.transform;
		opponent_holder.transform.parent = maze_holder.transform;
	}
	
	void SetMazePosition() {
		maze_holder.transform.position = new Vector3(-10.0f, -2.15f, -18.0f);
	}
	
	void Update() {
		
	}
}