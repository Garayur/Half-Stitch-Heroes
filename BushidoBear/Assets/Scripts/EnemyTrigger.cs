using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyTrigger : MonoBehaviour 
{
	[SerializeField]
	private List<EnemySpawner> spawners = new List<EnemySpawner>();

	public GameObject Spawners { set{spawners.Add (value.GetComponent<EnemySpawner>());} }



	void OnDrawGizmos ()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawCube (transform.position, Vector3.one);

		foreach (EnemySpawner e in spawners)
		{
			if (e != null)
			{
				Gizmos.DrawLine(transform.position, e.transform.position);
			}
		}
	}
}
