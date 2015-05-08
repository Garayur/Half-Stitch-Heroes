using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyTrigger : MonoBehaviour 
{
	private List<EnemySpawner> spawners = new List<EnemySpawner>();

    //Properties
    //========================================================================================
    public GameObject Spawners { set { spawners.Add(value.GetComponent<EnemySpawner>()); } }

    public string[] SpawnerNames
    {
        get
        {
            OrganizeSpawnList();

            string[] temp = new string[spawners.Count];

            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = spawners[i].gameObject.name;
            }

            return temp;
        }
    }
    //========================================================================================

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

    private void OrganizeSpawnList()
    {
        List<EnemySpawner> tempList = new List<EnemySpawner>();

        foreach (EnemySpawner e in spawners)
        {
            if (e != null)
            {
                tempList.Add(e);
            }
        }

        spawners = new List<EnemySpawner>();
        spawners = tempList;
    }

    public void DestroyAllNodes()
    {
        foreach (EnemySpawner e in spawners)
        {
            if (e != null)
            {
                Object.DestroyImmediate(e.gameObject);
            }
        }
    }

    public GameObject GetNodeByIndex(int index)
    {
        OrganizeSpawnList();
        GameObject[] temp = new GameObject[spawners.Count];

        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = spawners[i].gameObject;
        }

        return temp[index];
    }

    public void HighlightSelectedNodeByIndex(int index , bool b)
    {
        OrganizeSpawnList();
        spawners[index].HighLight(b);
    }
}
