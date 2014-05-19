using UnityEngine;
using FiercePlanet;
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
	GameState gamestate;


	public void Start () {
		gamestate = GameObject.Find("Main Camera").GetComponent<GameState>();
		agentNumber = gamestate.numAgents;


		particleScale = 1.0f;
		particleRange = 10f;

		Init();
	}

	public void Init() {

		agents = new ArrayList(agentNumber);

		// Do the borders conservatively
		BorderControl borderControl = GameObject.Find("Border").GetComponent<BorderControl>();
		int boundarySize = gamestate.boundarySize;
		if (boundarySize == 0)
			boundarySize = 25;
		borderControl.RedoBorders(boundarySize);

		// Update the dimensions
		UpdateWorldDimensions();

		// Spawn the right number of children
		GameObject baseAgent = GameObject.Find("BaseAgent");
		float lifeExpectancy = LifeExpectancy(gamestate.CurrentTimeInUnits());
		for (int i = 0; i < agentNumber; i++) {
			SpawnAgent(baseAgent);
			Agent agent = baseAgent.GetComponent<Agent>();
			float currentAge = Random.Range (0f, lifeExpectancy * 2f);
			agent.SetAge(currentAge);
		}

		// Set up coroutine to spawn migrating and reproducing agents
		StartCoroutine("Populate");
	}

	/// <summary>
	/// Handles asynchronous population of the simulation model.
	/// </summary>
	IEnumerator Populate() {
		GameObject baseAgent = GameObject.Find("BaseAgent");
		for (;;) {

			// Do net migration
			float percent = gamestate.percentageOfNewAgentsPerTimeUnit / 100f;
			float variation = gamestate.variationOfNewAgents / 100f;
			float multiplier = percent + Random.Range(- variation, variation);
			GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");

			int currentAgents = agents.Length;
//			float probabilty = currentAgents * multiplier * (1 / (float)gamestate.timeSecondsPerUnit);
			float probabilty = currentAgents * multiplier;
			int agentsToSpawn = Mathf.FloorToInt(probabilty);
			float chance = Random.Range(0f, 1f);
			if (chance < probabilty - agentsToSpawn)
				agentsToSpawn++;
			float lifeExpectancy = LifeExpectancy(gamestate.CurrentTimeInUnits());
			for (int i = 0; i < agentsToSpawn ; i++) {
				SpawnAgent(baseAgent);
				Agent agent = baseAgent.GetComponent<Agent>();
				float currentAge = Random.Range (0f, lifeExpectancy * 2f);
				agent.SetAge(currentAge);
			}


			// Reproduction


			// Life expectancy - USE VERY CRUDE APPROXIMATION FOR NOW
			// UPDATE, e.g. from http://www.gapminder.org/data/
			List<GameObject> deadAgents = new List<GameObject>();
			for (int i = 0; i < agents.Length; i++) 
			{
				Agent agent = agents[i].GetComponent<Agent>();
				float age = agent.GetAge();
				float thisAgentsLifeExpectancy = LifeExpectancyAtAge(gamestate.CurrentTimeInUnits(), agent.GetAge());
				float prob = Random.Range (0, thisAgentsLifeExpectancy);
				if (prob < 1.0f) {
					deadAgents.Add(agents[i]);
				}
			}
			foreach (GameObject deadAgent in deadAgents) {
				Debug.Log ("Destroying " + deadAgent.name);
				deadAgent.SetActive(false);
			}


			// To drip feed 
			//			yield return new WaitForSeconds(1f);
			yield return new WaitForSeconds((float)gamestate.timeSecondsPerUnit);
		}
	}



	private float LifeExpectancyAtAge(float time, float age) 
	{
		float expectancy = LifeExpectancy(time);
		if (age > expectancy)
			expectancy = age;
		return expectancy - age;
	}

	/// <summary>
	/// Very crude approximation to Australia's changing life expectancy
	/// </summary>
	/// <returns>The expectancy.</returns>
	/// <param name="time">Time.</param>
	private float LifeExpectancy(float time) {
		float baseExpectancy = 0f;
		if (time >= 1871 && time < 1900) {
			baseExpectancy = 41.9882f * 2f;
		}
		else if (time >= 1900 && time < 1930) {
			baseExpectancy = 57.8647f * 2f;
		}
		else if (time >= 1931 && time < 1960) {
			baseExpectancy = 68.46f * 2f;
		}
		else if (time >= 1871 && time < 1900) {
			baseExpectancy = 72.84f * 2f;
		}
		else if (time >= 1991) {
			baseExpectancy = 79.93f * 2f;
		}
		else {
			baseExpectancy = 34.05f * 2f;
		}
		return baseExpectancy;
	}

	public void UpdateWorldDimensions() {
		// Adjust the walls
		GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
		foreach (GameObject wall in walls) 
		{
			wall.transform.position += new Vector3(gamestate.originX, 0, gamestate.originZ);
		}
		
		// Adjust the baseAgent
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		float height = terrainData.GetHeight((int)gamestate.originX, (int)gamestate.originZ) + 6.0f;

		GameObject baseAgent = GameObject.Find("BaseAgent");
		baseAgent.transform.position = new Vector3(gamestate.originX, height, gamestate.originZ);

		// Adjust the camera
		GameObject camera = GameObject.Find ("Main Camera");
		camera.transform.position = new Vector3(gamestate.originX, camera.transform.position.y, gamestate.originZ);
	}

	public void SpawnAgent(GameObject agent) {
		Grid grid = GameObject.Find ("GridOrigin").GetComponent<Grid>();
		// Random x and z values
		float x = gamestate.originX + Random.Range(-particleRange, particleRange);
		float z = gamestate.originZ + Random.Range(-particleRange, particleRange);

		// Get the y value (approx) of the current position
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		float height = terrainData.GetHeight((int)(grid.GetHeightmapX( (int)(x + terrainData.size.x))), (int)(grid.GetHeightmapY( (int)(z + terrainData.size.z)))) + 6.0f;

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
		if (agents == null)
			agents = new ArrayList(newSize);
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
		transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * 100);
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

