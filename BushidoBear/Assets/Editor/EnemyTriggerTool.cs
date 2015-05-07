using UnityEngine;
using UnityEditor;
using System.Collections;

public class EnemyTriggerTool : EditorWindow 
{
	private GameObject currentSelection = null;

	[MenuItem ("Custom Tools/Enemy Trigger Tool")]
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(EnemyTriggerTool));
	}
	
	void OnGUI () 
	{
		if (GUILayout.Button("Create Spawn Node"))
		{
			CreateSpawnNde ();
		}
	}

	void CreateSpawnNde ()
	{
		if (currentSelection == null)
		{
			currentSelection = new GameObject ("EnemyTrigger");
			currentSelection.AddComponent<EnemyTrigger>();

			currentSelection.AddComponent<BoxCollider>();
			currentSelection.GetComponent<BoxCollider>().isTrigger = true;
		}
		GameObject spawnNode = new GameObject ("Spawn Node");
		spawnNode.AddComponent<EnemySpawner> ();

		currentSelection.GetComponent<EnemyTrigger> ().Spawners = spawnNode;
	}
}
