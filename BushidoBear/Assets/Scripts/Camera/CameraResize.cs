using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraResize : MonoBehaviour
{
    public string tagToFollow;

    private const float DISTANCE_MARGIN = 20.0f;

    private List<Transform> players = new List<Transform>();
    private Vector3 middlePoint = Vector3.zero;
    private float distanceBetweenPlayers = 0;
    private float cameraDistance = 0;
    private float aspectRatio;
    private float tanFov = 0;

    void Start()
    {
        aspectRatio = Screen.width / Screen.height;
        tanFov = Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2);

        foreach(GameObject o in GameObject.FindGameObjectsWithTag(tagToFollow))
        {
            players.Add(o.transform);
        }
    }

    void Update()
    {
        // Position the camera in the center.
        Vector3 newCameraPos = Camera.main.transform.position;
        newCameraPos.x = middlePoint.x;
        Camera.main.transform.position = newCameraPos;

        // Find the middle point between players.
        Vector3 vectorBetweenPlayers = Vector3.zero;
        foreach (Transform t in players)
        {
            vectorBetweenPlayers += t.position;
        }
        vectorBetweenPlayers = vectorBetweenPlayers / players.Count;
        middlePoint = vectorBetweenPlayers;

        // Calculate the new distance.
        distanceBetweenPlayers = vectorBetweenPlayers.magnitude;
        cameraDistance = (distanceBetweenPlayers / (float)players.Count / aspectRatio) / tanFov;

        // Set camera to new position.
        Vector3 dir = (Camera.main.transform.position - middlePoint).normalized;
        Camera.main.transform.position = middlePoint + dir * (cameraDistance + DISTANCE_MARGIN);
    }
}
