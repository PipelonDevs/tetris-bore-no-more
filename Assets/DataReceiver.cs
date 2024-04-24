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
using System.Linq;
using UnityEngine.UIElements;
using System.Globalization;

public class DataReceiver : MonoBehaviour
{
    [SerializeField]
    private int port = 1000; // Default port, can be adjusted in the Unity Inspector
    private Socket socket;
    private Thread receiveThread;
    private bool isCollectingData = false;
    public static DataReceiver Instance { get; private set; }
    Classifier classifier;

    TensorFloat inputTensor;
    List<string> messageBuffer = new List<string>();

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
        classifier = FindObjectOfType<Classifier>();
        SetupSocket();
        StartReceiving();
    }

    private void SetupSocket()
    {
        if (socket == null)
        {
            IPAddress ip = IPAddress.Any;
            IPEndPoint endPoint = new IPEndPoint(ip, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(endPoint);
            Debug.Log($"Listening on port '{port}'...");
            isCollectingData = true;
        }
    }

    public void StartReceiving()
    {


        receiveThread = new Thread(() =>
        {



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
                                messageBuffer.Add(message);
                                

                                // Check if we have collected 25 messages
                                if (messageBuffer.Count == 25)
                                {
                                    Debug.Log("25 messages received, classifying...");
                                    float[] data = new float[25*40];
                                int index = 0; // This index will track the position in the data array.

                                foreach (var msg in messageBuffer)
                                {
                                    // Split the message into individual string representations of floats.
                                    string[] values = msg.Split(',');
                                    values = values[16..56].ToArray();

                                    // Parse each value and place it into the data array.
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        // Use float.TryParse to safely parse the float and handle any format errors.
                                        if (float.TryParse(values[i], NumberStyles.Any, CultureInfo.InvariantCulture, out float parsedValue))
                                        {
                                            data[index] = parsedValue;
                                            index++;
                                        }
                                        else
                                        {
                                            Debug.LogError("Failed to parse float value: " + values[i]);
                                            // Handle the error appropriately, perhaps by setting a default value or skipping.
                                        }
                                    }
                                }


                                inputTensor = new TensorFloat(new TensorShape(1,25, 40), data);

                                // Call classifier with the tensor
                                classifier.onDataAcquired?.Invoke(inputTensor);

                                    // Clear the buffer for the next batch of messages
                                    messageBuffer.Clear();
                                }
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
