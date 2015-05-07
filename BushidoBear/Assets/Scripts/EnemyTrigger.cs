using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyTrigger : MonoBehaviour 
{
	[SerializeField]
	private List<EnemySpawner> spawners = new List<EnemySpawner>();

	public GameObject Spawners { set{spawners.Add (value.GetComponent<EnemySpawner>());} }

    void OnTriggerEnter()
    {
        foreach (EnemySpawner e in spawners)
        {
            if (e != null)
            {
                e.Spawn();
            }
        }
    }

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, (new Vector3(transform.localScale.x * GetComponent<BoxCollider>().size.x, 
                                                         transform.localScale.y * GetComponent<BoxCollider>().size.y, 
                                                         transform.localScale.z * GetComponent<BoxCollider>().size.z)));

        

		foreach (EnemySpawner e in spawners)
		{
			if (e != null)
			{
				Gizmos.DrawLine(transform.position, e.transform.position);
			}
		}
	}
}
