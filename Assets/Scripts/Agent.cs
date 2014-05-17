using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : MonoBehaviour 
{

	/// Public variables
	public float particleSpeed;
	public bool using3d;
	public bool showNetwork;

	/// Private variables
	private ArrayList friends;
	private ArrayList dummies;

	Color c1;
	Color c2;
	float particleCalibrate;
	int linePositions;
	static bool running;
	Vector3 currentDirection;
	Vector3 lastPosition;
	Grid grid;

	public void Start() {
		friends = new ArrayList();
		dummies = new ArrayList();
		particleSpeed = 1.0f;
		particleCalibrate = 0.1f;
		
		c1 = Color.green;
		c2 = Color.green;
		running = true;
		//linePositions = 20;

		using3d = false;
		showNetwork = false;

		lastPosition = gameObject.transform.position;
		grid = GameObject.Find ("GridOrigin").GetComponent<Grid>();

		CalculateSpeed();		
	}

	public void CalculateSpeed() {
		float dirX = (Random.Range(-1.0f, 1.0f) * particleSpeed);
		float dirY = 0;
		if (using3d)
			dirY = (Random.Range(-1.0f, 1.0f) * particleSpeed);
		float dirZ = (Random.Range(-1.0f, 1.0f) * particleSpeed);
		currentDirection = new Vector3(dirX, dirY, dirZ);
	}

	private Mesh mesh;
  private Vector3[] verts;
  private Vector2[] uvs;
  private int[] tris;
  public float gravity = 20.0F;

	public void Update() {
		if (! running) 
			return;

		float x = gameObject.transform.position.x;
		float y = gameObject.transform.position.z;

		if (!grid.InsideTerrain(gameObject.transform.position))
		    return;

		CharacterController controller = gameObject.GetComponent<CharacterController>();
		Vector3 currentCalibration = new Vector3(0, 0, 0);
		currentCalibration.x += (Random.Range(-1.0f, 1.0f) * particleCalibrate);
		if (using3d)
			currentCalibration.y += (Random.Range(-1.0f, 1.0f) * particleCalibrate);
		else if (! controller.isGrounded)
			currentCalibration.y -= gravity * Time.deltaTime;
			// currentCalibration.y = 0; //(Random.Range(-1.0f, 1.0f));
		currentCalibration.z += (Random.Range(-1.0f, 1.0f) * particleCalibrate);
		
		currentDirection += currentCalibration;
		// gameObject.rigidbody.AddForce(currentDirection);
		// gameObject.transform.position += currentDirection;
		// NOTE: Much faster than SimpleMove
		controller.Move(currentDirection * Time.deltaTime);
		//controller.SimpleMove(currentDirection);
		
		for (int j = 0; j < dummies.Count; j++) {
			GameObject dummyObj = (GameObject)dummies[j];
			Destroy(dummyObj.GetComponent("LineRenderer"));
		}
		Destroy(gameObject.GetComponent("LineRenderer"));

		// Calculate the patch

  	GameState gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		if (gameState.showNetwork) {


			// Do mesh here instead
	  //   if ( !mesh )
	  //   {
			// 		GameObject network = new GameObject(gameObject.name + " network");
			// 		network.transform.parent = gameObject.transform;
			// 		MeshFilter filter = (MeshFilter)network.AddComponent("MeshFilter");
			// 		MeshRenderer renderer = (MeshRenderer)network.AddComponent("MeshRenderer");
			// 		// MeshFilter filter = gameObject.GetComponent<MeshFilter>();
			// 		// MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
			// 		renderer.material.color = Color.green;
	  //       mesh = new Mesh();
	  //       // MeshFilter f = GameObject.Find("Network").GetComponent<MeshFilter>();
	  //       filter.mesh = mesh;
	  //       mesh.name = gameObject.name + "Mesh";
	  //   }
	  //   mesh.Clear(); 

	  //   Vector3[] vertices = new Vector3[friends.Count * 4];
	  //   Vector2[] uvs = new Vector2[friends.Count * 4];
	  //   int[] triangles = new int[friends.Count * 6];

			// for (int i = 0; i < friends.Count; i++) {
			// 	GameObject friend = (GameObject)friends[i];
			// 	if (friend != null && gameObject.transform.position != null && friend.transform.position != null) {
			// 		GameObject dummy = (GameObject)dummies[i];

			// 		Vector3 start = gameObject.transform.position;
			// 		Vector3 end = friend.transform.position;

			// 		Vector3[] quads = MakeQuad(start, end, 1.0f);
			// 		vertices[i * 4] = quads[0];
			// 		vertices[i * 4 + 1] = quads[1];
			// 		vertices[i * 4 + 2] = quads[2];
			// 		vertices[i * 4 + 3] = quads[3];

			// 		uvs[i * 4] = new Vector2(0, 1);
			// 		uvs[i * 4 + 1] = new Vector2(1, 1);
			// 		uvs[i * 4 + 2] = new Vector2(0, 0);
			// 		uvs[i * 4 + 3] = new Vector2(1, 0);

			// 		triangles[i * 6] = i * 4;
			// 		triangles[i * 6 + 1] = i * 4 + 1;
			// 		triangles[i * 6 + 2] = i * 4 + 2;
			// 		triangles[i * 6 + 3] = i * 4 + 1;
			// 		triangles[i * 6 + 4] = i * 4 + 2;
			// 		triangles[i * 6 + 5] = i * 4 + 3;
			// 	}
			// }
			// mesh.vertices = vertices;
			// mesh.uv = uvs;
			// mesh.triangles = triangles;
			// mesh.RecalculateNormals();
			// mesh.RecalculateBounds();

			// Material material = new Material();
			// material.color = Color.green;
			for (int i = 0; i < friends.Count; i++) {
				GameObject friend = (GameObject)friends[i];
				if (friend != null && gameObject.transform.position != null && friend.transform.position != null) {
					GameObject dummy = (GameObject)dummies[i];
					LineRenderer lineRenderer = (LineRenderer)dummy.GetComponent("LineRenderer");
					if (lineRenderer == null)
						lineRenderer = (LineRenderer)dummy.AddComponent("LineRenderer");
					lineRenderer.material.color = Color.green;
					// lineRenderer.SetColors(c1, c2);
					lineRenderer.SetWidth(0.5f,0.5f);

					// Do positions
					lineRenderer.SetPosition(0, gameObject.transform.position);
					// var diffX = friend.transform.position.x - gameObject.transform.position.x;
					// var diffY = friend.transform.position.y - gameObject.transform.position.y;
					// var diffZ = friend.transform.position.z - gameObject.transform.position.z;
					// for (var k = 1; k < linePositions; k++) {
					// 	var pos : Vector3 = Vector3(gameObject.transform.position.x + k * diffX, 
					// 		gameObject.transform.position.y + k * diffY, 
					// 		gameObject.transform.position.z + k * diffZ);
					// 	lineRenderer.SetPosition(k, pos);
					// }
					lineRenderer.SetPosition(1, friend.transform.position);
				}
			}
		}

		// Turn on cells visited
		if (grid.enabled)
			grid.TurnOnCell(gameObject.transform.position);
	}

	Vector3[] MakeQuad(Vector3 s, Vector3 e, float w) {
		w = w / 2;
		Vector3[] q = new Vector3[4];

		Vector3 n = Vector3.Cross(s, e);
		Vector3 l = Vector3.Cross(n, e-s);
		l.Normalize();
		
		q[0] = transform.InverseTransformPoint(s + l * w);
		q[1] = transform.InverseTransformPoint(s + l * -w);
		q[2] = transform.InverseTransformPoint(e + l * w);
		q[3] = transform.InverseTransformPoint(e + l * -w);

		return q;
	}

	bool HasDroppedAltitude() {
		return ((lastPosition.y - gameObject.transform.position.y) > 0.1f);
	}

	bool HasRaisedAltitude() {
		return ((gameObject.transform.position.y - lastPosition.y) > 0.1f);
	}

  void OnControllerColliderHit(ControllerColliderHit hit) {
  	GameState gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
  	Grid grid = GameObject.Find("GridOrigin").GetComponent<Grid>();
		if (hit.collider.gameObject.name.Equals("Terrain")) {
			
	  	TerrainData terrainData = Terrain.activeTerrain.terrainData;
	  	float terrainX = gameObject.transform.position.x + terrainData.size.x / 2f;
	  	float terrainY = gameObject.transform.position.z + terrainData.size.z / 2f;
	  	float height = terrainData.GetHeight((int)grid.GetHeightmapX((int)terrainX) , (int)grid.GetHeightmapY((int)terrainY));
	  	float steepness = terrainData.GetSteepness((int)grid.GetHeightmapX((int)terrainX) , (int)grid.GetHeightmapY((int)terrainY));

	  	// Over correction
	  	if ((gameObject.transform.position.y < 2f || height < 1f)) {
	  		/*
	  		if (HasDroppedAltitude())
	  			currentDirection /= 4f;
  			if (HasRaisedAltitude())
  				currentDirection *= 4f;
  				*/
	  		// Debug.Log("Ever get here?");
	  		// currentDirection.x = -currentDirection.x;
		  	// currentDirection.z = -currentDirection.z;
	  	}
	  	lastPosition = gameObject.transform.position;
		}
		if (hit.collider.gameObject.name.Equals("CubeBottomBorder") || hit.collider.gameObject.name.Equals("CubeTopBorder")) {
			currentDirection.z = -currentDirection.z;
		}
		else if (hit.collider.gameObject.name.Equals("CubeLeftBorder") || hit.collider.gameObject.name.Equals("CubeRightBorder")) {
			currentDirection.x = -currentDirection.x;
		}
		if (gameState.showNetwork && hit.collider.gameObject.CompareTag("Agent")) {
			if (hit.collider.gameObject != null) {
				if (friends == null)
					friends = new ArrayList();
				if (dummies == null)
					dummies = new ArrayList();

				// Have we already met this agent?
				bool found = false;
				foreach (GameObject value in friends) {
					if (value == hit.collider.gameObject)
						found = true;
				}
				if (found)
					return;
				
				// Does this agent contain us as a friend?
				Agent agent = (Agent)hit.collider.gameObject.GetComponent("Agent");
				if (agent  != null && agent.friends != null) {
					foreach (GameObject value in agent.friends) {
						if (value == gameObject)
							found = true;
					}
				}
				if (found)
					return;

				friends.Add(hit.collider.gameObject);
				GameObject dynamicObjects = GameObject.Find("DynamicObjects");
				GameObject dummy = new GameObject(gameObject.name + " link with " + hit.collider.gameObject.name);
				dummy.transform.parent = dynamicObjects.transform;
				LineRenderer lineRenderer = (LineRenderer)dummy.AddComponent("LineRenderer");
				lineRenderer.SetPosition(0, new Vector3(0f, 0f, 0f));
				lineRenderer.SetPosition(1, new Vector3(0f, 0f, 0f));
				dummies.Add(dummy);
			}
		}
	}


	public void ToggleRunning() {
		running = !running;		
	}

	public void ResetZAxis() {
		Vector3 pos = gameObject.transform.position;
		pos.z = 0;
		gameObject.transform.position = pos;
	}
}
