using UnityEngine;
using UnityEditor;

public class StreamManagerConfigurator : EditorWindow
{
    // Variables to track our progress through the steps
    private GameObject streamManagerObject;
    private UnityPythonClient pythonClientScript;

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
        EditorGUILayout.HelpBox("Follow the steps below to configure the Stream Manager and Camera in your scene.", MessageType.Info);
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
            
            Debug.Log("✅ Step 1 Complete: Stream Manager created.");
        }

        EditorGUILayout.Space();

        // --- STEP 2: Attach the Script ---
        // Only enable this button if Step 1 is complete (object exists)
        GUI.enabled = streamManagerObject != null; 
        
        if (GUILayout.Button("Step 2: Attach UnityPythonClient Script"))
        {
            // Attach the script to the GameObject
            pythonClientScript = Undo.AddComponent<UnityPythonClient>(streamManagerObject);
            Debug.Log("✅ Step 2 Complete: Script attached.");
        }

        EditorGUILayout.Space();

        // --- STEP 3: Configure Settings ---
        // Only enable this button if Step 2 is complete (script is attached)
        GUI.enabled = pythonClientScript != null; 

        if (GUILayout.Button("Step 3: Apply Default Settings"))
        {
            if (Camera.main != null)
            {
                pythonClientScript.targetCamera = Camera.main;
            }
            else
            {
                Debug.LogWarning("⚠️ No Main Camera found in the scene to assign.");
            }

            pythonClientScript.pythonIP = "127.0.0.1";
            pythonClientScript.pythonPort = 5000;
            pythonClientScript.quality = 50; 

            // Mark the object as dirty so Unity knows to save the changes
            EditorUtility.SetDirty(pythonClientScript);
            
            Debug.Log("✅ Step 3 Complete: Settings applied successfully.");
        }
        
        EditorGUILayout.Space();
        
        // Reset GUI enabled state so Step 4 is always clickable
        GUI.enabled = true; 

        // --- STEP 4: Configure Camera Scripts ---
        if (GUILayout.Button("Step 4: Attach Movement & Gyro Scripts to Camera"))
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                GameObject camObj = mainCam.gameObject;
                
                // Attach CameraMovement if missing
                if (camObj.GetComponent<CameraMovement>() == null)
                {
                    Undo.AddComponent<CameraMovement>(camObj);
                    Debug.Log("✅ Attached CameraMovement to the Main Camera.");
                }

                // Attach HostGyroReceiver if missing
                if (camObj.GetComponent<HostGyroReceiver>() == null)
                {
                    Undo.AddComponent<HostGyroReceiver>(camObj);
                    Debug.Log("✅ Attached HostGyroReceiver to the Main Camera.");
                }

                Debug.Log("✅ Step 4 Complete: Camera scripts configured.");
            }
            else
            {
                Debug.LogError("❌ No Main Camera found! Make sure your camera has the 'MainCamera' tag in the Inspector.");
            }
        }
    }
}