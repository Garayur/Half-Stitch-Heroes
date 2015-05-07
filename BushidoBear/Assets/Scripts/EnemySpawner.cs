using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour 
{

    public void Spawn()
    {
        print("Creature Spawned");
    }

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, 0.2f);
	}
}
