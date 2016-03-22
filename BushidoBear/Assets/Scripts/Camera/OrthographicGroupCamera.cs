using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrthographicGroupCamera : MonoBehaviour {

	public float leftBoundary;
	public float rightBoundary;
	public float topBoundary;
	public float bottomBoundary;

	private List<Transform> players = new List<Transform>();
	private Vector3 centroid;
	private float minCameraSize = 7;
	private float maxPlayerDistanceWidth;
	private float maxPlayerDistanceHeight;
	private Vector3 cameraPosition;
	private float yOffset = 6.0f;
	private float xBuffer = 2.0f;

	void Update () {
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

		cameraPosition.x = Mathf.Clamp (centroid.x, leftBoundary + cameraWidth / 2, rightBoundary - cameraWidth / 2);
		cameraPosition.y = Mathf.Clamp (rotatedCentroid.y, bottomBoundary, topBoundary);
	
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
}
