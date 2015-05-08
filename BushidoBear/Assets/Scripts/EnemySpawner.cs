using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour 
{
    bool isHighlighted = false;

    public void Spawn()
    {
        print("Creature Spawned");
    }

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, 0.2f);

        if (isHighlighted)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.4f);
        }
	}

    internal void HighLight(bool b)
    {
        isHighlighted = b;
    }
}
