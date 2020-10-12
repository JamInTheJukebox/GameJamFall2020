
using UnityEngine;
// To implement a custom window needs to be included in UnityEditor
using UnityEditor;
using System.Collections;

// EditorWindow is another very useful Editor class. We can rely on it to define our own window
public class PreviewPlaybackWindow : EditorWindow
{
    // MenuItem allows us to open this window in the menu bar
    [MenuItem("Window/Preview Playback Window")]
    static void OpenPreviewPlaybackWindow()
    {
        EditorWindow.GetWindow<PreviewPlaybackWindow>(false, "Playback");
        // Another useful way of writing is as follows
        // allows us to access the properties of the window, such as defining the minimum size, etc.
        //EditorWindow window = EditorWindow.GetWindow<PreviewPlaybackWindow>( false, "Playback" );
        //window.minSize = new Vector2(100.0f, 100.0f);
    }

    float m_PlaybackModifier;
    float m_LastTime;

    void OnEnable()
    {
        // We can register our own editor Update function accordingly.
        EditorApplication.update -= OnUpdate;
        EditorApplication.update += OnUpdate;
    }

    void OnDisable()
    {
        EditorApplication.update -= OnUpdate;
    }

    public class PreviewTime
    {
        public static float Time
        {
            get
            {
                if (Application.isPlaying == true)
                {
                    return UnityEngine.Time.timeSinceLevelLoad;
                }

                return EditorPrefs.GetFloat("PreviewTime", 0f);
            }
            set
            {
                EditorPrefs.SetFloat("PreviewTime", value);
            }
        }
    }

    void OnUpdate()
    {
        if (m_PlaybackModifier != 0f)
        {

            // m_PlaybackModifier is a variable used to control the preview playback rate
            // When it is not 0, it means you need to refresh the interface, update time
            PreviewTime.Time += (Time.realtimeSinceStartup - m_LastTime) * m_PlaybackModifier;

            // When the preview time changes, we need to make sure to redraw this window so that we can see its updates immediately
            // Unity will only redraw if it thinks the window needs to be redrawn (for example, if we move the window)
            // So we can call the Repaint function to force a redraw immediately
            Repaint();

            // Since the preview time has changed, we also hope that we can redraw the interface of the Scene view immediately.
            SceneView.RepaintAll();
        }

        m_LastTime = Time.realtimeSinceStartup;
    }

    void OnGUI()
    {
        // draw individual buttons to control the preview time
        float seconds = Mathf.Floor(PreviewTime.Time % 60);
        float minutes = Mathf.Floor(PreviewTime.Time / 60);

        GUILayout.Label("Preview Time: " + minutes + ":" + seconds.ToString("00"));
        GUILayout.Label("Playback Speed: " + m_PlaybackModifier);

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("|<", GUILayout.Height(30)))
            {
                PreviewTime.Time = 0f;
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("<<", GUILayout.Height(30)))
            {
                m_PlaybackModifier = -5f;
            }

            if (GUILayout.Button("<", GUILayout.Height(30)))
            {
                m_PlaybackModifier = -1f;
            }

            if (GUILayout.Button("||", GUILayout.Height(30)))
            {
                m_PlaybackModifier = 0f;
            }

            if (GUILayout.Button(">", GUILayout.Height(30)))
            {
                m_PlaybackModifier = 1f;
            }

            if (GUILayout.Button(">>", GUILayout.Height(30)))
            {
                m_PlaybackModifier = 5f;
            }
        }
        GUILayout.EndHorizontal();
    }
}