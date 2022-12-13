using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;


public class TCPClient : MonoBehaviour
{
    #region private members
    public TcpClient socketConnection;
    public Thread clientReceiveThread;
    #endregion

    // TCP Socket Information:
    private string localIPAddr = "127.0.0.1";
    private string remoteIPAddr = "172.20.10.9";
    private string IPAddrUsed = "";
    private int portUsed = 13;
    public bool keyboardBool = true;

    public TouchScreenKeyboard keyboard;
    public string keyboardText = "";

    // TCP Message Booleans:
    public bool connectedToServer = false;
    public bool processStarted = false;

    // Message parameters:
    public Queue tcpDataQueue = new Queue();
    public Queue tcpErrorQueue = new Queue();
    public string currentCommand = "";
    public string currentErr = "";

    // Data Information:
    [SerializeField]
    public PointPicking dataPointHolder;
    public SpeedSlider speedSlider;
    public ZoneSlider zoneSlider;

    // ABB Controller Parameters:
    public int contrNum;

    // Unity UI Buttons:
    public GameObject startProcessButton;
    public GameObject endProcessButton;

    // Unity UI Labels:
    public TextMeshProUGUI connectLabel;
    public TextMeshProUGUI errMessLabel;
    public TextMeshProUGUI clientMessLabel;
    public TextMeshProUGUI servMessLabel;
    public TextMeshProUGUI processStatusLabel;
    public TextMeshProUGUI IPAddrLabel;

    // Start() runs when program is first executed:
    void Start()
    {
        connectLabel.text = "Ready to connect!";
        connectLabel.color = Color.blue;
    }

    // Update() is called once per frame
    void Update()
    {

        // IP Address Keyboard Polling:
        if(keyboard != null)
        {
            keyboardText = keyboard.text;
            IPAddrLabel.text = keyboardText.ToString();
            
        }

        // Processes the messages sent from the server:
        if(tcpDataQueue.Count > 0)
        {
            lock (tcpDataQueue)
            {
                currentCommand = tcpDataQueue.Dequeue().ToString();
                processServerMessage(currentCommand);
            }
        }

        if(tcpErrorQueue.Count > 0)
        {
            lock(tcpErrorQueue)
            {
                currentErr = tcpErrorQueue.Dequeue().ToString();
                errMessLabel.text = currentErr;
            }
        }

    }

    public void ChangeIPAddr()
    {
        if(keyboardBool)
        {
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false); // Loads the keyboard
        }
        else
        {
            IPAddrUsed = keyboardText.ToString(); // Stores the keyboard text as the desired IP address to connect to
            IPAddrUsed = IPAddrUsed.Replace("\n", "").Replace("\r", "");
            connectLabel.text = "IP Address set!";
            keyboard = null; // Resets keyboard object
        }

        keyboardBool = !keyboardBool;
    }

    
    public void ConnectToRemoteTcpServer()
    {
        //if (IPAddrUsed == "")
        if(false)
        {
            errMessLabel.text = "Error Message: Please enter an IP Address!";
        }
        else
        {
            errMessLabel.text = "Error Message:";

            try
            {
                connectLabel.text = "Connecting...";
                //clientReceiveThread = new Thread(new ThreadStart(ListenForData(ipAddr, portNum));
                clientReceiveThread = new Thread(() => ListenForData(remoteIPAddr, portUsed));
                clientReceiveThread.IsBackground = true;
                clientReceiveThread.Start();
            }
            catch (Exception e)
            {
                connectLabel.text = "Not connected!";
                connectLabel.color = Color.red;
                errMessLabel.text = "Error Messsage: On client connection exception: " + e;
            }
        }
    }
    

    public void ConnectToLocalTcpServer()
    {
        try
        {
            connectLabel.text = "Connecting...";
            //clientReceiveThread = new Thread(new ThreadStart(ListenForData()));
            clientReceiveThread = new Thread(() => ListenForData(localIPAddr, portUsed));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            connectLabel.text = "Not connected!";
            connectLabel.color = Color.red;
            errMessLabel.text = "Error Message: On client connection exception: " + e;
        }
    }


    public void ListenForData(string ipAddr, int portNum)
    {
        try
        {
            //socketConnection = new TcpClient("localhost", 13); // Setting up new TCP Client
            socketConnection = new TcpClient(ipAddr, portNum);

            Byte[] bytes = new Byte[32768];
            while(true)
            {
                // Get a stream object for reading
                using(NetworkStream stream = socketConnection.GetStream()) // Starting new client stream
                {
                    int length;

                    // Read incoming stream into byte array:
                    while((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incomingData = new byte[length];
                        Array.Copy(bytes, 0, incomingData, 0, length);
                        
                        // Convert byte array to string message:
                        string serverMessage = Encoding.ASCII.GetString(incomingData);
                        Debug.Log(serverMessage);

                        // Places server message in queue:
                        lock(tcpDataQueue)
                        {
                            tcpDataQueue.Enqueue(serverMessage);
                        }
                    }
                }
            }
        }
        catch (SocketException e)
        {            
            lock(tcpErrorQueue)
            {
                tcpErrorQueue.Enqueue(e);
            }
        }
    }

    public void SendMessageToServer(string clientMessage)
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                // Create and convert string message to byte array.
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                
                // Write byte array to socketConnection stream.
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
            }

        }
        catch (SocketException socketException)
        {
            errMessLabel.text = "Error Message: Socket Exception: " + socketException;
            //Debug.Log("Socket exception: " + socketException);
        }
    }

    public void processServerMessage(string serverMessage)
    {
        /*** This method maps out the overall process flow ***/
        if (!connectedToServer)
        {
            if (serverMessage == "Connected.")
            {
                connectLabel.text = serverMessage;
                connectLabel.color = Color.green;
                connectedToServer = true;
                errMessLabel.text = "Error Message:";
            }
        }
        else if (serverMessage == "DISCONNECTING")
        {
            connectedToServer = false;
            processStarted = false;

            endProcessButton.SetActive(false);

            connectLabel.text = "Disconnected.";
            connectLabel.color = Color.red;
            processStatusLabel.text = "Process Status:";
            errMessLabel.text = "Error Message:";
        }
        else if(serverMessage.Substring(0, 3) == "ERR")
        {
            errMessLabel.text = "Error Message: " + serverMessage.Substring(4);
        }
        else if(!processStarted)
        {
            if(serverMessage == "PROCESS_STARTED")
            {
                processStatusLabel.text = "Process Status: Connected to controller";
                endProcessButton.SetActive(true);

                processStarted = true;
            }
        }
        else if (serverMessage == "PROCESS_ENDED")
        {
            endProcessButton.SetActive(false);
            processStarted = false;

            processStatusLabel.text = "Process Status: Disconnected from controller";

        }
        else if (serverMessage == "GOING_FORWARD")
        {
            dataPointHolder.ptpIndex++;

            dataPointHolder.pathViabilityLabel.text = "Path Viability: Going forward!";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: F-T";
        }
        else if (serverMessage == "GOING_BACKWARD")
        {
            dataPointHolder.ptpIndex--;

            dataPointHolder.pathViabilityLabel.text = "Path Viability: Going backward!";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: B-T";
        }
        else if (serverMessage == "ROTATING_TCP")
        {
            dataPointHolder.pathViabilityLabel.text = "Path Viability: Updating TCP!";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: U-T";
        }
        else if (serverMessage == "FORWARD_NOT_POSSIBLE")
        {
            dataPointHolder.pathViabilityLabel.text = "Path Viability: Forward movement not possible.";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: F-F";
        }
        else if (serverMessage == "BACKWARD_NOT_POSSIBLE")
        {
            dataPointHolder.pathViabilityLabel.text = "Path Viability: Backward movement not possible.";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: B-F";
        }
        else if (serverMessage == "ROTATION_NOT_POSSIBLE")
        {
            dataPointHolder.pathViabilityLabel.text = "Path Viability: Updating current position not possible!";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: U-F";
        }
        else if(serverMessage.Substring(0, 3) == "PTP")
        {
            dataPointHolder.ptpIndex = int.Parse(serverMessage.Substring(3));

            dataPointHolder.pathViabilityLabel.text = "Path Viability: Traversing completed!";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: T-C";
        }


        /*
        else if(serverMessage == "CPT-T")
        {
            dataPointHolder.ptpIndex = dataPointHolder.pointCounter + 1;

            dataPointHolder.pathViabilityLabel.text = "Path Viability: Traversing completely through!";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: CPT-T";
        }
        else if(serverMessage == "CPT-F")
        {
            dataPointHolder.pathViabilityLabel.text = "Path Viability: CPT not possible!";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: CPT-F";
        }
        else if(serverMessage == "CPTB-T")
        {
            dataPointHolder.ptpIndex = 1;

            dataPointHolder.pathViabilityLabel.text = "Path Viability: Traversing completely back!";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: CPTB-T";
        }
        */
    }
        

    // TCP Message-Specific Methods:
    public void StartProcess()
    {
        string clientMessage = "START_PROCESS";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;
    }

    public void EndProcess()
    {
        string clientMessage = "END_PROCESS";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;
    }

    public void GoForward()
    {
        string clientMessage = "GO_FORWARD";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;   
    }

    public void GoBackward()
    {
        string clientMessage = "GO_BACKWARD";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;
    }

    public void changeCurRot()
    {
        string clientMessage = "ROTATE_TCP";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;
    }

    public void runCPT()
    {
        SendDataMessage();
        string clientMessage = "RUN_CPT";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;

    }

    public void runCPTB()
    {
        //SendDataMessage();
        string clientMessage = "RUN_CPT_BACK";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;

    }

    public void SendDataMessage()
    {
        Vector3 curCoor;
        Quaternion curRot;

        string clientMessage = "#";

        for (int i = 0; i < dataPointHolder.pointCounter; i++)
        {
            curCoor = dataPointHolder.posArr[i];
            curCoor *= 1000f; // Converts the spatial coordinates of each data point to mm

            clientMessage += "[ " + curCoor.x + ", " + curCoor.y + ", " + curCoor.z + "]/";
            curRot = dataPointHolder.rotArr[i];
            clientMessage += "[ " + curRot.w + ", " + curRot.x + ", " + curRot.y + ", " + curRot.z + "];";
        }

        clientMessage += "@";
        clientMessage += speedSlider.speedVal.ToString();
        clientMessage += "%";
        clientMessage += zoneSlider.zoneVal.ToString();
        clientMessage += "%";
        clientMessage += dataPointHolder.pointCounter.ToString();
        clientMessage += "%";
        clientMessage += dataPointHolder.ptpIndex.ToString();

        if (processStarted)
        {
            SendMessageToServer(clientMessage);
            Debug.Log(clientMessage);
        }
        else
        {
            errMessLabel.text = "Error Message: You must connected to the server first!";
        }
    }



    public void resetHomePos()
    {
        string clientMessage = "RESET_HOME";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: Reset home position!";
    }

    public void ClearAllPoints()
    {
        string clientMessage = "CLEAR_DATA";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;
    }


}
