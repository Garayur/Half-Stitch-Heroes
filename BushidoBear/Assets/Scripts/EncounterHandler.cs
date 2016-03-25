using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EncounterHandler : MonoBehaviour {

	public List<BaseAICoordinator> coordinatorList = new List<BaseAICoordinator>();
	public int xmin = -6;
	public int xmax = 6;
	public int zmin = -1;
	public int zmax = 14;

	public GameObject leftWallPrefab;
	public GameObject rightWallPrefab;
	public GameObject farWallPrefab;
	public GameObject nearWallPrefab;

	protected CombatBoundaries boundaries;

	protected GameObject leftWall;
	protected GameObject rightWall;
	protected GameObject farWall;
	protected GameObject nearWall;

	protected bool encounterTriggered = false;

	protected void OnEnable() {
		BaseAICoordinator.CoordinatorDeath += CoordinatorDeathEvent;
	}

	protected void OnDisable() {
		BaseAICoordinator.CoordinatorDeath += CoordinatorDeathEvent;
	}

	void OnTriggerEnter(Collider other){
		if (!encounterTriggered) {
			encounterTriggered = true;
			StartEncounter ();
			StartCoroutine ("CheckCoordinatorList");
		}
	}

	void Start(){
		boundaries = new CombatBoundaries (xmin, xmax, zmin, zmax);
	}


	IEnumerator CheckCoordinatorList(){

		if (coordinatorList.Count <= 0)
			EndEncounter ();

		yield return new WaitForSeconds (0.5f);
		StartCoroutine ("CheckCoordinatorList");
	}

	protected void CoordinatorDeathEvent(BaseAICoordinator coordinator){
		coordinatorList.Remove (coordinator);
	}

	protected void SpawnBoundaries(){
		leftWall = (GameObject)Instantiate (leftWallPrefab, new Vector3 (boundaries.leftBoundary - 6.6f, 5.5f, 10), Quaternion.identity);
		rightWall = (GameObject)Instantiate (rightWallPrefab, new Vector3 (boundaries.rightBoundary + 6.6f, 5.5f, 10), Quaternion.identity);
		farWall = (GameObject)Instantiate (farWallPrefab, new Vector3 (0, 5.5f, boundaries.farBoundary + 4.5f), Quaternion.identity);
		nearWall = (GameObject)Instantiate (nearWallPrefab, new Vector3 (0, 5.5f, boundaries.closeBoundary - 9f), Quaternion.identity);
	}

	protected void RemoveBoundaries(){
		Destroy (leftWall);
		Destroy (rightWall);
		Destroy (farWall);
		Destroy (nearWall);
	}

	protected void StartEncounter(){
		SpawnBoundaries ();
		foreach(BaseAICoordinator coordinator in coordinatorList){
			coordinator.AssignBoundaries (boundaries);
		}
		Camera.main.GetComponent<OrthographicGroupCamera>().AssignBoundaries(boundaries);
	}

	protected void EndEncounter(){
		StopCoroutine ("CheckCoordinateList");
		RemoveBoundaries ();
		Camera.main.GetComponent<OrthographicGroupCamera>().ClearBoundaries();
		Destroy(this);
	}
}
