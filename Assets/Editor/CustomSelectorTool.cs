using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class CustomSelectorTool : EditorWindow
{
        // Custom tool to moving blocks to the correct location in the hierarchy
    [MenuItem("Custom/Platform Selector")]
    public static void OpenCustomWindow()           // initializes the window to make sure only one instance of the window can be open.
    {
        var window = EditorWindow.GetWindow(typeof(CustomSelectorTool));
        var title = new GUIContent();
        title.text = "Select a group of platforms!";
        window.titleContent = title;
        
    }

    public int selectedBlock;               // keeps track of the user's selection
    public string[] options = new string[] { "Spade", "Heart", "Diamond", "Club" };      // the possible types of blocks to select.
    private string[] Color = new string[] { "Blue", "Red", "Yellow", "Green" };
    private void OnGUI()
    {
        selectedBlock = EditorGUILayout.Popup("Type of Block", selectedBlock, options);
        if(Selection.gameObjects.Length == 0) { return; }
        GameObject TheParent = Selection.gameObjects[0];
        if (GUILayout.Button("Select Platforms"))
        {
            if(TheParent != null)
            {
                SelectBlocks(TheParent.transform);
            }
        }
    }

    private void SelectBlocks(Transform TheParent)
    {
        List<GameObject> selectedObj = new List<GameObject>();
        string CurrentOption = options[selectedBlock].ToLower();
        string SpriteDirectory = "Assets/Art/Platforms/" + CurrentOption + "_platform.png";         //Directory for all the art. Choose the one based on the current option.
        Sprite BlockSprite = (Sprite)AssetDatabase.LoadAssetAtPath(SpriteDirectory, typeof(Sprite));// Retrieve the sprite
        Transform RealParent = TheParent.Find(Color[selectedBlock]);
        if(RealParent == null) { return; }
        foreach (Transform child in TheParent)
        {
            SpriteRenderer ChildRenderer = child.GetComponent<SpriteRenderer>();
            if (ChildRenderer != null && ChildRenderer.sprite == BlockSprite)
                child.parent = RealParent;                                                          // assign block to its correct parent
        }
    }
}
