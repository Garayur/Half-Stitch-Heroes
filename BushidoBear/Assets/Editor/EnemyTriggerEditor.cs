using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(EnemyTrigger))]
public class EnemyTriggerEditor : Editor 
{
    private EnemyTrigger myTarget;
    private bool isSpawnListDisplayed = false;
    private string lastTooltip = " ";
    private int lastSelected = 0;

    public override void OnInspectorGUI()
    {
        myTarget = (EnemyTrigger)target;

        DisplaySpawnNodes();

        GUILayout.Space(10);
        if (GUILayout.Button("Create Spawn Node"))
        {
            CreateSpawnNde();
        }
    }

    private void DisplaySpawnNodes()
    {
        string[] spawners = myTarget.SpawnerNames;
        //GUIStyle style = new GUIStyle();

        //style.alignment = TextAnchor.MiddleCenter;

        GUILayout.Space(10);
        isSpawnListDisplayed = EditorGUILayout.Foldout(isSpawnListDisplayed, "Connected Spawners");
        if (isSpawnListDisplayed)
        {
            for (int i = 0; i < spawners.Length; i++)
            {
                if(GUILayout.Button(new GUIContent(spawners[i], "Highlighting Selected Node" + i.ToString()), GUILayout.Width(85)))
                {
                    Selection.activeGameObject = myTarget.GetNodeByIndex(i);
                    myTarget.HighlightSelectedNodeByIndex(i, false);
                    GUILayout.Space(5);
                }

                if (Event.current.type == EventType.Repaint && GUI.tooltip != lastTooltip)
                {
                    if (lastTooltip != "")
                    {
                        myTarget.HighlightSelectedNodeByIndex(lastSelected, false);
                    }

                    if (GUI.tooltip != "")
                    {
                        myTarget.HighlightSelectedNodeByIndex(i, true);
                        lastSelected = i;
                    }

                    lastTooltip = GUI.tooltip;
                }
            }
        }
    }

    private void CreateSpawnNde()
    {
        GameObject spawnNode = new GameObject("Spawn Node");
        spawnNode.transform.position = myTarget.gameObject.transform.position;
        spawnNode.AddComponent<EnemySpawner>();

        myTarget.Spawners = spawnNode;
    }

    private void OnDestroy()
    {
        if (Application.isEditor)
        {
            if (myTarget == null)
            {
                myTarget.DestroyAllNodes();
            }
        }
    }
}
