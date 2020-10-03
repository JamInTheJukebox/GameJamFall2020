using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(Environment))]
public class EnvironmentGenerator : Editor
{
    public override void OnInspectorGUI()       // overrides default inspector view
    {
        DrawDefaultInspector();

        Environment Plat = (Environment)target;

        if(GUILayout.Button("Create Platforms"))
        {
            Plat.SpawnPlatforms();
        }
    }
}
