using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System;

public class UnityPythonClient : MonoBehaviour
{
    public Camera targetCamera;

    [Header("Python Server Settings")]
    public string pythonIP = "127.0.0.1";
    public int pythonPort = 5000;
    [Range(10, 100)] public int quality = 50;

    private TcpClient client;
    private NetworkStream stream;
    private RenderTexture renderTexture;
    private Texture2D texture;

    // Default starting resolution (matches index 0 in Python)
    private int currentWidth = 600;
    private int currentHeight = 540;

    void Start()
    {
        InitializeTextures(currentWidth, currentHeight);
        ConnectToPython();
        StartCoroutine(SendFrames());
    }

    void InitializeTextures(int w, int h)
    {
        // Clean up old textures if they exist to prevent memory leaks
        if (renderTexture != null) renderTexture.Release();
        if (texture != null) Destroy(texture);

        renderTexture = new RenderTexture(w, h, 24);
        texture = new Texture2D(w, h, TextureFormat.RGB24, false);
        currentWidth = w;
        currentHeight = h;

        Debug.Log($"Camera Capture Resolution set to: {w}x{h}");
    }

    void ConnectToPython()
    {
        try
        {
            client = new TcpClient(pythonIP, pythonPort);
            stream = client.GetStream();
            Debug.Log("Connected to Python Relay!");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to connect to Python: " + e.Message);
        }
    }

    void Update()
    {
        // Check if Python sent a resolution update packet (8 bytes)
        if (client != null && client.Connected && stream != null && stream.DataAvailable)
        {
            byte[] buffer = new byte[8];
            int bytesRead = stream.Read(buffer, 0, 8);

            if (bytesRead == 8)
            {
                int newWidth = BitConverter.ToInt32(buffer, 0);
                int newHeight = BitConverter.ToInt32(buffer, 4);

                if (newWidth != currentWidth || newHeight != currentHeight)
                {
                    InitializeTextures(newWidth, newHeight);
                }
            }
        }
    }

    IEnumerator SendFrames()
    {
        var eof = new WaitForEndOfFrame();
        while (true)
        {
            yield return eof;

            if (client == null || !client.Connected) continue;

            // 1. Capture the camera viewport
            targetCamera.targetTexture = renderTexture;
            targetCamera.Render();
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            targetCamera.targetTexture = null;
            RenderTexture.active = null;

            // 2. Encode to JPEG
            byte[] jpgBytes = texture.EncodeToJPG(quality);

            // 3. Create a 4-byte length header (Little Endian)
            byte[] lengthHeader = BitConverter.GetBytes(jpgBytes.Length);

            try
            {
                // 4. Send header, then payload
                stream.Write(lengthHeader, 0, lengthHeader.Length);
                stream.Write(jpgBytes, 0, jpgBytes.Length);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Disconnected from Python: " + e.Message);
                client.Close();
                break; // Stop coroutine if connection dies
            }
        }
    }

    void OnApplicationQuit()
    {
        if (client != null)
            client.Close();
    }
}