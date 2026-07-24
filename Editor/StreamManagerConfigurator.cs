using UnityEngine;
using UnityEditor;

public class StreamManagerConfigurator : EditorWindow
{
    [MenuItem("Tools/Setup Stream Manager")]
    public static void SetupScene()
    {
        // 1. Setup the Stream Manager GameObject
        GameObject streamManager = GameObject.Find("Stream Manager");
        if (streamManager == null)
        {
            streamManager = new GameObject("Stream Manager");
            streamManager.AddComponent<UnityPythonClient>();
            Debug.Log("✅ Created 'Stream Manager' and attached UnityPythonClient.");
        }
        else
        {
            Debug.LogWarning("⚠️ 'Stream Manager' already exists in the scene.");
        }

        // 2. Setup the Main Camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            GameObject camObj = mainCamera.gameObject;
            
            // Attach CameraMovement if it's missing
            if (camObj.GetComponent<CameraMovement>() == null)
            {
                camObj.AddComponent<CameraMovement>();
                Debug.Log("✅ Attached CameraMovement to the Main Camera.");
            }

            // Attach HostGyroReceiver if it's missing
            if (camObj.GetComponent<HostGyroReceiver>() == null)
            {
                camObj.AddComponent<HostGyroReceiver>();
                Debug.Log("✅ Attached HostGyroReceiver to the Main Camera.");
            }
        }
        else
        {
            Debug.LogError("❌ No Main Camera found! Make sure your camera has the 'MainCamera' tag in the Inspector.");
        }
    }
}
