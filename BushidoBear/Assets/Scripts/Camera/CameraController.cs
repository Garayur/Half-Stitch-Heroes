using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    List<ViewEffector> highNooners;
    bool noPlayers;
    public float centerX;

	// Use this for initialization
	void Start ()
    {
        //create list of all players
        highNooners = new List<ViewEffector>();
        ViewEffector[] ve = GameObject.FindObjectsOfType<ViewEffector>();
        foreach (ViewEffector v in ve)
        {
            AddViewEffector(v);
        }

        //chect that players are in the scene
        if (highNooners.Count == 0)
        {
            print("No Players to track...");
            noPlayers = true;
        }
        
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        if (!noPlayers)
        {
            //find center of all players
            float lowest = Mathf.Infinity;
            float highest = Mathf.NegativeInfinity;
            foreach (ViewEffector v in highNooners)
            {
                if (lowest > v.transform.position.x)
                {
                    lowest = v.transform.position.x;
                }
                if (highest < v.transform.position.x)
                {
                    highest = v.transform.position.x;
                }
            }
            centerX = (lowest + highest) / 2.0f;

            Vector3 modifiedPos = new Vector3(centerX, transform.position.y, transform.position.z);

            transform.position = modifiedPos;
        }
    }

    public void AddViewEffector(ViewEffector v)
    {
        highNooners.Add(v);
        noPlayers = false;
    }

    public void RemoveViewEffectors(ViewEffector v)
    {
        highNooners.Remove(v);
        if (highNooners.Count == 0)
        {
            noPlayers = true;
            print("Last Player Removed");
        }
    }
}
