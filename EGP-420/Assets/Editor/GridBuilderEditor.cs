using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridScript))]
public class GridBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridScript myTarget = (GridScript)target;
        DrawDefaultInspector();

        //execute in edit mode
        if (GUILayout.Button("Generate New Level"))
        {
            myTarget.OnValidate();
        }
        if (GUILayout.Button("Destroy Level"))
        {
            Debug.Log("Descroy button pressed");
            myTarget.grid.reset();
        }
    }
}