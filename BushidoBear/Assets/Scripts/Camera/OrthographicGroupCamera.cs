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

	private float aspectRatio;

	void Start () {
		aspectRatio = (float)Screen.width / (float)Screen.height;
	}

	void Update () {
		CalculateCentroid ();
		CalculateMaxPlayerHeight ();
		CalculateMaxPlayerWidth ();
		UpdateCameraSize ();
		UpdateCameraPosition (); 
	}

	protected void UpdateCameraSize(){
		float widthAdjustedOrthographicSize = maxPlayerDistanceWidth / aspectRatio;

		if (widthAdjustedOrthographicSize > maxPlayerDistanceHeight) {
			if (widthAdjustedOrthographicSize < minCameraSize)
				Camera.main.orthographicSize = minCameraSize;
			else
				Camera.main.orthographicSize = widthAdjustedOrthographicSize;
		} 
		else {
			if(	maxPlayerDistanceHeight < minCameraSize)
				Camera.main.orthographicSize = minCameraSize;
			else
				Camera.main.orthographicSize = maxPlayerDistanceHeight;
		}
	}

	protected void UpdateCameraPosition(){
		float cameraWidth = Camera.main.orthographicSize * aspectRatio;
		cameraPosition = Camera.main.transform.position;

		Vector3 rotatedCentroid = centroid;
		rotatedCentroid = Quaternion.AngleAxis (-15, Vector3.right) * centroid;
		rotatedCentroid.y += 6;

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
	}

	protected void CalculateMaxPlayerWidth(){
		float tempWidth = 0;
		foreach (Transform transform in players) {
			foreach (Transform transform2 in players) {
				if (transform != transform2) {
					tempWidth = Mathf.Abs (transform.position.x - transform2.position.x);
						if(tempWidth > maxPlayerDistanceWidth)
							maxPlayerDistanceWidth = tempWidth;
				}
			}
		}
	}

	protected void CalculateMaxPlayerHeight(){
		float tempHeight = 0;
		Vector3 position1;
		Vector3 position2;
		foreach (Transform transform in players) {
			position1 = transform.position;
			position1.x = 0;
			foreach (Transform transform2 in players) {
				if (transform != transform2) {
					position2 = transform2.position;
					position2.x = 0;
					tempHeight = Vector3.Distance(position1, position2) * 2;
					if(tempHeight > maxPlayerDistanceHeight)
						maxPlayerDistanceHeight = tempHeight;
				}
			}
		}
	}
}
