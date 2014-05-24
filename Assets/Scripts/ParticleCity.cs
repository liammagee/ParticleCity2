using UnityEngine;
using FiercePlanet;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent (typeof (GameState))]
public class ParticleCity : MonoBehaviour 
{


	int agentNumber;
	public float particleScale;
	float boundarySize;

	ArrayList agents;

	float panZ = 0.0f;
	float panX = 0.0f;
	float panY = 0.0f;
	bool shiftHeld = false;
	public GameState gamestate;


	public void Start () 
    {
		if (gamestate == null)
			gamestate = GameObject.Find("Main Camera").GetComponent<GameState>();
		agentNumber = gamestate.numAgents;

		Init();
	}

	public void Init() 
    {
		agents = new ArrayList(agentNumber);

		// Do the borders conservatively
		BorderControl borderControl = GameObject.Find("Border").GetComponent<BorderControl>();
		boundarySize = gamestate.boundarySize;
		if (boundarySize == 0)
			boundarySize = 50;
		borderControl.RedoBorders(boundarySize);

		// Update the dimensions
		UpdateWorldDimensions();

		// Spawn the right number of children
		GameObject baseAgent = GameObject.Find("BaseAgent");
		for (int i = 0; i < agentNumber; i++) {
			SpawnAgent(baseAgent);
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
			float ct = gamestate.CurrentTimeInUnits();

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
			for (int i = 0; i < agentsToSpawn ; i++) {
				SpawnAgent(baseAgent);
			}


			// Reproduction
			IEnumerable<Agent> reproducableFemales = agents.Select(c => c.GetComponent<Agent>()).Where(c => c.GetComponent<Agent>().CanReproduce(ct));
			float fertilityRate = GetFertilityRate(ct);
			foreach (Agent potentialMother in reproducableFemales) 
			{
				float bd = potentialMother.GetBirthdate();
				float age =  ct - bd;
				int children = potentialMother.GetChildren().Count;
				// Simple calculation re: potential for childbirth in this year
				float baseChance = Mathf.Log(fertilityRate + 1.0f) / 30f;
				// Multiply the chance by the difference between 
				float childRatio = fertilityRate / ((float)children + fertilityRate);
				baseChance *= childRatio;
				float prob = Random.Range (0f, 1f);
				if (prob < baseChance) {
					Agent child = SpawnAgent(baseAgent);
					potentialMother.AddChild(child);
					child.SetMother(potentialMother);
				}
			}


			// Life expectancy - USE VERY CRUDE APPROXIMATION FOR NOW
			// UPDATE, e.g. from http://www.gapminder.org/data/
			List<GameObject> deadAgents = new List<GameObject>();
			for (int i = 0; i < agents.Length; i++) 
			{
				Agent agent = agents[i].GetComponent<Agent>();
				float bd = agent.GetBirthdate();
				float thisAgentsLifeExpectancy = LifeExpectancyAtAge(ct, ct - bd);
				float prob = Random.Range (0f, thisAgentsLifeExpectancy);
				if (prob < 1.0f)  {
					deadAgents.Add(agents[i]);
				}
			}
			foreach (GameObject deadAgent in deadAgents) {
				deadAgent.SetActive(false);
			}

			// To drip feed the changes by second
			//			yield return new WaitForSeconds(1f);
			yield return new WaitForSeconds((float)gamestate.timeSecondsPerUnit);
		}
	}



	private float LifeExpectancyAtAge(float time, float age) 
	{
		float expectancy = LifeExpectancy(time) * 2f;
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
			baseExpectancy = 41.9882f;
		}
		else if (time >= 1900 && time < 1930) {
			baseExpectancy = 57.8647f;
		}
		else if (time >= 1931 && time < 1960) {
			baseExpectancy = 68.46f;
		}
		else if (time >= 1871 && time < 1900) {
			baseExpectancy = 72.84f;
		}
		else if (time >= 1991) {
			baseExpectancy = 79.93f;
		}
		else {
			baseExpectancy = 34.05f;
		}
		return baseExpectancy;
	}

	/// <summary>
	/// Provide very rough approximation to Australia's fertility rates, 1901 onwards
	/// Taken from Gapminder data
	/// </summary>
	public float GetFertilityRate(float time) 
	{
		float fertilityRate = 0f;
		if (time >= 1800 && time < 1850) {
			fertilityRate = 41.9882f;
		}
		else if (time >= 1850 && time < 1880) {
			fertilityRate = 5.6414f;
		}
		else if (time >= 1881 && time < 1910) {
			fertilityRate = 4.0736f;
		}
		else if (time >= 1910 && time < 1940) {
			fertilityRate = 2.95f;
		}
		else if (time >= 1940 && time < 1970) {
			fertilityRate = 3.275f;
		}
		else {
			fertilityRate = 1.902f;
		}
		return fertilityRate;
	}

	public void UpdateWorldDimensions() {
		
		// Adjust the baseAgent
		Grid grid = GameObject.Find ("GridOrigin").GetComponent<Grid>();
		Terrain terrain = grid.GetTerrain();
		GameObject baseAgent = GameObject.Find("BaseAgent");
        Vector3 position = new Vector3(gamestate.originX, 0, gamestate.originZ);
        float height = terrain.SampleHeight(position);
        position.y = height + 1.0f;
        baseAgent.transform.position = position;

		// Adjust the camera
		GameObject camera = GameObject.Find ("Main Camera");
		camera.transform.position = new Vector3(gamestate.originX, camera.transform.position.y, gamestate.originZ);
	}

	public Agent SpawnAgent(GameObject agent) {
		Grid grid = GameObject.Find ("GridOrigin").GetComponent<Grid>();

		// Random x and z values
        Terrain terrain = grid.GetTerrain();
		TerrainData terrainData = terrain.terrainData; 
		Vector3 terrainSize = terrainData.size;
		float terrainX = (terrainSize.x / 2f);
		float terrainZ = (terrainSize.z / 2f);
		float adjustPercent = boundarySize / 100f;

		float x = gamestate.originX + Random.Range(-terrainX * adjustPercent, terrainX * adjustPercent);
		float z = gamestate.originZ + Random.Range(-terrainZ * adjustPercent, terrainZ * adjustPercent);

		// Get the y value (approx) of the current position
        Vector3 position = new Vector3(x, 0, z);
		float height = terrain.SampleHeight(position);
        position.y = height + 1.0f;

		// Set the position and scale
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

		float ct = gamestate.CurrentTimeInUnits ();
		float lifeExpectancy = LifeExpectancy(ct);
		float currentAge = Random.Range (0f, lifeExpectancy * 2f);
		agentScript.SetBirthdate(ct - currentAge);

		agents.Add(newAgent);
		return agentScript;
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

