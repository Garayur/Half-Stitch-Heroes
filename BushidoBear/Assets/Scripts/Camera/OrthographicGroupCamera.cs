using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrthographicGroupCamera : MonoBehaviour {

	private CombatBoundaries boundaries = new CombatBoundaries();
	private bool useBoundaries = false;
	private List<Transform> players = new List<Transform>();
	private Vector3 centroid;
	private float minCameraSize = 7;
	private float maxPlayerDistanceWidth;
	private float maxPlayerDistanceHeight;
	private Vector3 cameraPosition;
	private float yOffset = 6.0f;
	private float xBuffer = 2.0f;

	void LateUpdate () {
		CalculateCentroid ();
		CalculateMaxPlayerWidth ();
		UpdateCameraSize ();
		UpdateCameraPosition (); 
	}

	protected void UpdateCameraSize(){

		float adjustedWidth = maxPlayerDistanceWidth / (Camera.main.aspect * 2);

		if (adjustedWidth < minCameraSize)
			Camera.main.orthographicSize = minCameraSize;
		else
			Camera.main.orthographicSize = adjustedWidth;
	}

	protected void UpdateCameraPosition(){
		float cameraWidth = Camera.main.orthographicSize * Camera.main.aspect;
		cameraPosition = Camera.main.transform.position;

		Vector3 rotatedCentroid = centroid;
		rotatedCentroid = Quaternion.AngleAxis (-Camera.main.transform.eulerAngles.x, Vector3.right) * centroid;
		rotatedCentroid.y += yOffset;

		if (useBoundaries) {
			cameraPosition.x = Mathf.Clamp (centroid.x, boundaries.leftBoundary + cameraWidth / 2, boundaries.rightBoundary - cameraWidth / 2);
		} 
		else {
			cameraPosition.x = centroid.x;
		}
		cameraPosition.y = Mathf.Clamp (rotatedCentroid.y, boundaries.closeBoundary, boundaries.farBoundary);

		Camera.main.transform.position = cameraPosition;
	}
		

	protected void CalculateCentroid(){
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
		{
			players.Add(player.transform);
		}
			
		centroid = Vector3.zero;
		foreach (Transform t in players)
		{
			centroid += t.position;
		}
		centroid = centroid / players.Count;
		centroid.y = 0;
	}

	protected void CalculateMaxPlayerWidth(){
		float tempWidth = 0;
		maxPlayerDistanceWidth = 0;
		foreach (Transform transform in players) {
			foreach (Transform transform2 in players) {
				if (transform != transform2) {
					tempWidth = Mathf.Abs (transform.position.x - transform2.position.x);
						if(tempWidth > maxPlayerDistanceWidth)
						maxPlayerDistanceWidth = tempWidth + xBuffer;
				}
			}
		}
	}

	public void AssignBoundaries(CombatBoundaries boundaries){
		this.boundaries = boundaries;
		useBoundaries = true;
	}

	public void ClearBoundaries(){
		useBoundaries = false;
	}
}
