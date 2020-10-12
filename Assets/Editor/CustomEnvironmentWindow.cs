using UnityEditor;
using UnityEngine;

public class CustomEnvironmentWindow : EditorWindow
{
    // custom tool to spawn all types of blocks, maybe even enemies in the future!

    [MenuItem("Custom/Platform Creator")]
    public static void OpenCustomWindow()           // initializes the window to make sure only one instance of the window can be open.
    {
        var window = EditorWindow.GetWindow(typeof(CustomEnvironmentWindow));
        var title = new GUIContent();
        title.text = "Create some platforms!";
        window.titleContent = title;
    }

    public int selectedBlock;               // keeps track of the user's selection
    static int BlockColumns;                // number of columns
    static int BlockRows;                   // number of rows
    public string[] options = new string[] { "Spade", "Heart", "Diamond", "Club" };      // the possible types of blocks to spawn.
    public float X_OFFSET = 0.4061f;        // the amount of offset one block has to another block(Based on the sprite's normal size)
    public float Y_OFFSET = 0.37100878f;    // the amount of offset one block has to another block(Based on the sprite's normal size)

    private void OnGUI()
    {
        selectedBlock = EditorGUILayout.Popup("Type of Block", selectedBlock, options);
        BlockColumns = EditorGUILayout.IntField("Number of Clones along X: ", BlockColumns);
        BlockColumns = (int)Mathf.Clamp(BlockColumns, 1, 100);
        BlockRows = EditorGUILayout.IntField("Number of Clones along Y: ", BlockRows);
        BlockRows = (int)Mathf.Clamp(BlockRows, 1, 100);

        if (GUILayout.Button("Create Platforms"))
        {
            if(selectedBlock >= 0 | selectedBlock < 4)
            {
                CreateBlocks();
            }
            else
            {
                // do something else here. Spawn other blocks!
            }
        }
    }

    private void CreateBlocks()
    {
        Vector2 SpawnPosition = SceneView.lastActiveSceneView.camera.transform.position;            //Current position in the sceneview, so a gameobject doesn't spawn far away
        string CurrentOption = options[selectedBlock].ToLower();                                    //Current Choice on what block we want to spawn.
        Transform Environment = GameObject.Find("Environment").transform;                           //we will place all environment platforms here.
        if(Environment == null) { Debug.LogError("CustomEnvironmentWindow.cs: There must be an environment gameobject to proceed!"); return; }  // return if null
        string SpriteDirectory = "Assets/Art/Platforms/" + CurrentOption + "_platform.png";         //Directory for all the art. Choose the one based on the current option.
        Sprite BlockSprite = (Sprite)AssetDatabase.LoadAssetAtPath(SpriteDirectory, typeof(Sprite));// Retrieve the sprite
        GameObject PlatformPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Platforms/Platform.prefab", typeof(GameObject));  // retreive a platform prefab. This will parent all our blocks
        GameObject BlockPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Platforms/Block-1x1.prefab", typeof(GameObject));    // retreive a single block prefab
        GameObject Platform = PrefabUtility.InstantiatePrefab(PlatformPrefab, Environment) as GameObject;   //Spawn platform
        Platform.name = $"Platform {BlockRows}x{BlockColumns}";                                     // give the platform an appropriate name
        Transform BlockPlacement = Platform.transform.Find("Blocks").transform;                     // the parent that will hold all the blocks
        for (int i = 0; i < BlockRows; i++)
        {
            for(int j = 0; j < BlockColumns; j++)
            {
                // spawn blocks and parent them to the blockplacement gameobject.
                GameObject clone = PrefabUtility.InstantiatePrefab(BlockPrefab, BlockPlacement) as GameObject;

                clone.transform.position = SpawnPosition + Vector2.right * X_OFFSET * j + Vector2.up * Y_OFFSET * i;
                clone.GetComponent<SpriteRenderer>().sprite = BlockSprite;
                //clone.GetComponent<SpriteRenderer>().sortingOrder = i;
            }
        }
    }
}
