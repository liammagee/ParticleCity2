using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleCity : MonoBehaviour 
{
	int agentNumber;
	public float particleScale;
	float particleRange;

	ArrayList agents;

	float panZ = 0.0f;
	float panX = 0.0f;
	float panY = 0.0f;
	bool shiftHeld = false;



	public void Start () {
		GameState gamestate = GameObject.Find("Main Camera").GetComponent<GameState>();
		agentNumber = gamestate.numAgents;
		particleScale = 1.0f;
		particleRange = 100f;
		agents = new ArrayList(agentNumber);

		Init();
	}

	public void Init() {

		// Do the borders conservatively
		BorderControl borderControl = GameObject.Find("Border").GetComponent<BorderControl>();
		borderControl.RedoBorders(10f);

		// Correct the y co-ordinate for the base agent
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		float x = 0; //terrainData.size.x / 2;
		float z = 0; //terrainData.size.z / 2;
		float height = terrainData.GetHeight((int)x, (int)z) + 6.0f;

		GameObject baseAgent = GameObject.Find("BaseAgent");
		baseAgent.transform.position = new Vector3(x, height, z);

		// Spawn the right number of children
		for (int i = 0; i < agentNumber; i++) {
			SpawnAgent(baseAgent);
		}

	}

	public void SpawnAgent(GameObject agent) {
		// Random x and z values
		float x = Random.Range(-particleRange, particleRange);
		float z = Random.Range(-particleRange, particleRange);

		// Get the y value (approx) of the current position
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		float height = terrainData.GetHeight((int)x + 100, (int)z + 100) + 6.0f;

		// Set the position and scale
	  Vector3 position = new Vector3(x, height, z);
		Vector3 scale = new Vector3(particleScale, particleScale, particleScale);

		// Instantiate the new agent
		GameState gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		GameObject dynamicObjects = GameObject.Find("DynamicObjects");
		GameObject newAgent = (GameObject)Instantiate(agent);
		newAgent.transform.parent = dynamicObjects.transform;
		newAgent.transform.position = position;
		newAgent.transform.localScale = scale;
		newAgent.name = "Agent " + (agents.Count);
		Agent agentScript  = (Agent)newAgent.GetComponent("Agent");
		agentScript.showNetwork = gameState.showNetwork;
		agents.Add(newAgent);
	}

	public void ResizeAgents(int newSize) {
		if (newSize < agents.Count) {
			for (int i = newSize; i < agents.Count ;i++) {
				GameObject agent = (GameObject)agents[i];
				GameObject.Destroy(agent);
			}
			agents.RemoveRange(newSize, agents.Count - newSize - 1);
		}
		else if (newSize > agents.Count) {
			GameObject baseAgent = GameObject.Find("BaseAgent");
			for (int i = agents.Count; i < newSize ;i++) {
				SpawnAgent(baseAgent);
			}
		}
	}

	public void SoftRestart() {
		Time.timeScale = 0.0F;
		GameObject dynamicObjects = GameObject.Find("DynamicObjects");
    int childs = dynamicObjects.transform.childCount;
    for (int i = childs - 1; i >= 0; i--)
    {
        GameObject.Destroy(dynamicObjects.transform.GetChild(i).gameObject);
    }

		Grid grid = GameObject.Find("GridOrigin").GetComponent<Grid>();
		grid.InitGrid();

		Init();
		Time.timeScale = 1.0F;
	}

	public void Update () {
		TouchHandler(); 
		MouseHandler(); 
		KeyHandler(); 
	}

	// Detects if the shift key was pressed
	public void OnGUI() {
		Event e = Event.current;
		if (e.shift) {
			shiftHeld = true;
		}
		else {
			shiftHeld = false;	
		}
	}


	public void TouchHandler() {
		if (Input.touchCount >= 2)
		{
			Vector2 touch0 ;
			Vector2 touch1 ;
			float distance;
			touch0 = Input.GetTouch(0).position;
			touch1 = Input.GetTouch(1).position;
			distance = Vector2.Distance(touch0, touch1);
		}
	}

	public void MouseHandler() {
		transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * 20);
	}


	public void KeyHandler() {
		bool changed  = false;
		panZ = 0;
		panX = 0;
		panY = 0;
		float panShift = 5.0f;

		if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.Plus) || 
			(shiftHeld  && Input.GetKey(KeyCode.Equals))) {
			panX = panShift;
			panZ = panShift;
			changed = true;
		}
		else if (Input.GetKey(KeyCode.KeypadMinus) || Input.GetKey(KeyCode.Minus)) {
			panX = -panShift;
			panZ = -panShift;
			changed = true;
		}
		else if (Input.GetKey(KeyCode.RightArrow)) {
			panX = panShift;
			// panZ = -panShift;
			changed = true;
		}
		else if (Input.GetKey(KeyCode.LeftArrow)) {
			panX = -panShift;
			// panZ = panShift;
			changed = true;
		}
		else if (Input.GetKey(KeyCode.UpArrow)) {
			// panX = panShift;
			panZ = panShift;
			changed = true;
		}
		else if (Input.GetKey(KeyCode.DownArrow)) {
			// panX = -panShift;
			panZ = -panShift;
			changed = true;
		}
		if (changed)
			ResetCamera();
	}

	public void ResetCamera() {
		float newX = Camera.allCameras[0].transform.position.x + panX;
		float newY = Camera.allCameras[0].transform.position.y + panY;
		float newZ = Camera.allCameras[0].transform.position.z + panZ;
		Vector3 newPosition = new Vector3(newX, newY, newZ);
		Camera.allCameras[0].transform.position = newPosition;
	}


}

