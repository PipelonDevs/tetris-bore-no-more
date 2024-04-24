using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using Unity.Sentis;

public class DataCollector : MonoBehaviour
{
    public static DataCollector Instance { get; private set; }



    [SerializeField]
    private int port = 1000; // Default port, can be adjusted in the Unity Inspector
    private Socket socket;
    private Thread receiveThread;
    private bool isCollectingData = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        SetupSocket();
    }

    private void SetupSocket()
    {
        if(socket == null)
        {
            IPAddress ip = IPAddress.Any;
            IPEndPoint endPoint = new IPEndPoint(ip, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(endPoint);
            Debug.Log($"Listening on port '{port}'...");
            isCollectingData = true;
        }
    }

    public void StartReceiving(string filePath)
    {


        receiveThread = new Thread(() =>
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fileStream))

                try
            {
                byte[] receiveBufferByte = new byte[1024];
                while (isCollectingData)
                {
                    if (socket.Available > 0)
                    {
                        int numberOfBytesReceived = socket.Receive(receiveBufferByte);
                        if (numberOfBytesReceived > 0)
                        {
                            byte[] messageByte = new byte[numberOfBytesReceived];
                            Array.Copy(receiveBufferByte, messageByte, numberOfBytesReceived);
                            string message = System.Text.Encoding.ASCII.GetString(messageByte);
                                writer.WriteLine(message);
                                writer.Flush();
                         }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error during UDP data acquisition: {ex.Message}");
            }
        })
        {
            IsBackground = true
        };
        receiveThread.Start();
    }

    public void StopAcquisition()
    {
        isCollectingData = false;
        if (socket != null)
        {
            socket.Close();
            socket = null;
        }
    }

    private void OnApplicationQuit()
    {
        StopAcquisition();
    }


}