using UnityEngine;
using UnityEditor;

public class StreamManagerConfigurator : EditorWindow
{
    // Variables to track our progress through the steps
    private GameObject streamManagerObject;
    private UnityPythonClient pythonClientScript;
    private CameraMovement cameraMovementScript;
    private HostGyroReceiver gyroReceiverScript;

    // This creates the menu item in the top toolbar
    [MenuItem("Tools/Stream Manager Configurator")]
    public static void ShowWindow()
    {
        // Opens the window and sets its title
        GetWindow<StreamManagerConfigurator>("Stream Configurator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Stream Manager Setup", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Follow the steps below to configure the Stream Manager in your scene.", MessageType.Info);
        EditorGUILayout.Space();

        // --- STEP 1: Create the empty GameObject ---
        if (GUILayout.Button("Step 1: Create 'Stream Manager' GameObject"))
        {
            // Create a new empty GameObject
            streamManagerObject = new GameObject("Stream Manager");
            
            // Allow this action to be undone via Ctrl+Z
            Undo.RegisterCreatedObjectUndo(streamManagerObject, "Create Stream Manager");
            
            // Select it in the hierarchy
            Selection.activeGameObject = streamManagerObject; 
            
            Debug.Log("Step 1 Complete: Stream Manager created.");
        }

        EditorGUILayout.Space();

        // --- STEP 2: Attach the Scripts ---
        // Only enable this button if Step 1 is complete (object exists)
        GUI.enabled = streamManagerObject != null; 
        
        if (GUILayout.Button("Step 2: Attach All Scripts"))
        {
            // Attach the main streaming script
            pythonClientScript = streamManagerObject.AddComponent<UnityPythonClient>();
            
            // Attach the Camera Movement script
            cameraMovementScript = streamManagerObject.AddComponent<CameraMovement>();
            
            // Attach the Gyro Receiver script
            gyroReceiverScript = streamManagerObject.AddComponent<HostGyroReceiver>();
            
            Debug.Log("Step 2 Complete: UnityPythonClient, CameraMovement, and HostGyroReceiver attached.");
        }

        EditorGUILayout.Space();

        // --- STEP 3: Configure Settings ---
        // Only enable this button if Step 2 is complete (main script is attached)
        GUI.enabled = pythonClientScript != null; 

        if (GUILayout.Button("Step 3: Apply Default Settings"))
        {
            // Configure UnityPythonClient settings
            if (Camera.main != null)
            {
                pythonClientScript.targetCamera = Camera.main;
            }
            else
            {
                Debug.LogWarning("No Main Camera found in the scene to assign.");
            }

            pythonClientScript.pythonIP = "127.0.0.1";
            pythonClientScript.pythonPort = 5000;
            pythonClientScript.quality = 50;

            // Mark the object as dirty so Unity knows to save the changes
            EditorUtility.SetDirty(pythonClientScript);
            
            // If you need to set default variables for CameraMovement or HostGyroReceiver in the future, 
            // you can do it right here and call EditorUtility.SetDirty() for them as well.
            
            Debug.Log("Step 3 Complete: Settings applied successfully.");
        }
        
        // Reset GUI enabled state so it doesn't affect other Editor windows
        GUI.enabled = true; 
    }
}