using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;

public class HostGyroReceiver : MonoBehaviour
{
    public int listenPort = 5003;
    private UdpClient udpClient;
    private Thread receiveThread;
    private Quaternion latestRotation = Quaternion.identity;
    private bool newRotationAvailable = false;

    void Start()
    {
        udpClient = new UdpClient(listenPort);
        receiveThread = new Thread(ReceiveGyro);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void Update()
    {
        if (newRotationAvailable)
        {
            // Convert Android right-handed coordinates to Unity left-handed
            Quaternion convertedCoords = new Quaternion(latestRotation.x, latestRotation.y, -latestRotation.z, -latestRotation.w);

            // Adjust the base rotation (you may need to tweak the Euler angles depending on how the phone is held)
            transform.rotation = Quaternion.Euler(90, 0, 0) * convertedCoords;

            newRotationAvailable = false;
        }
    }

    void ReceiveGyro()
    {
        try
        {
            while (true)
            {
                System.Net.IPEndPoint anyIP = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref anyIP); // Blocks until data arrives
                string text = Encoding.UTF8.GetString(data);

                string[] parts = text.Split(',');
                if (parts.Length == 4)
                {
                    float x = float.Parse(parts[0]);
                    float y = float.Parse(parts[1]);
                    float z = float.Parse(parts[2]);
                    float w = float.Parse(parts[3]);

                    latestRotation = new Quaternion(x, y, z, w);
                    newRotationAvailable = true;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Gyro receiver stopped: " + e.Message);
        }
    }

    void OnDestroy()
    {
        if (udpClient != null) udpClient.Close();
        if (receiveThread != null) receiveThread.Abort();
    }
}