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
    private string IPAddrUsed = "10.0.16.21"; // Placeholder string representing inputted IP address - set to the remote IP address of the computer by default
    private int portUsed = 13; // Port number of TCP socket used
    public bool keyboardBool = true;

    public TouchScreenKeyboard keyboard; // Game object representing the keyboard in AR
    public string keyboardText = ""; // Placeholder string representing the text typed into the keyboard

    // TCP Message Booleans:
    public bool connectedToServer = false; // Boolean indicating server connection status
    public bool processStarted = false; // Boolean indicating process initiation status

    // Message parameters:
    public Queue tcpDataQueue = new Queue(); // Queue representing general messages sent from the server
    public Queue tcpErrorQueue = new Queue(); // Queue representing error messages sent from the server
    public string currentCommand = ""; // Placeholder string representing the current command sent by the server
    public string currentErr = ""; // Placeholder string representing the current error message sent from the server

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
        // Initializes UI:
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
            IPAddrLabel.text = keyboardText.ToString(); // Stores the inputted IP address
            
        }

        // Processes the messages sent from the server:
        if(tcpDataQueue.Count > 0)
        {
            lock (tcpDataQueue)
            {
                currentCommand = tcpDataQueue.Dequeue().ToString();
                processServerMessage(currentCommand); // Processes current command sent from the server
            }
        }

        // Error messages sent from the server:
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
            IPAddrUsed = IPAddrUsed.Replace("\n", "").Replace("\r", ""); // Removes all new line and carriage returns present in the inputted string
            connectLabel.text = "IP Address set!"; // Updates UI
            keyboard = null; // Resets keyboard object
        }

        keyboardBool = !keyboardBool;
    }

    public void ConnectToTCPServer(string IPAddress)
    {
        // Generic method to connect to the TCP server using the given IP Address

        errMessLabel.text = "Error Message:"; // Resets the error message UI

        try
        {
            connectLabel.text = "Connecting...";
            //clientReceiveThread = new Thread(new ThreadStart(ListenForData(ipAddr, portNum));
            clientReceiveThread = new Thread(() => ListenForData(IPAddress, portUsed)); // Creates a new thread to listen to TCP server messages
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start(); // Starts the thread
        }
        catch (Exception e)
        {
            // Updates UI with the received error:
            connectLabel.text = "Not connected!";
            connectLabel.color = Color.red;
            errMessLabel.text = "Error Messsage: On client connection exception: " + e;
        }
    }

    public void ConnectToRemoteTcpServer()
    {
        // Method to connect to the remote TCP server

        ConnectToTCPServer(IPAddrUsed);
    }
    

    public void ConnectToLocalTcpServer()
    {
        // Method to connect to the TCP server on local host (127.0.0.1)

        ConnectToTCPServer("127.0.0.1");
    }


    public void ListenForData(string ipAddr, int portNum)
    {
        try
        {
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
        // Method to send message to TCP server

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
                // Updates UI:
                connectLabel.text = serverMessage;
                connectLabel.color = Color.green;
                connectedToServer = true;
                errMessLabel.text = "Error Message:";
            }
        }
        else if (serverMessage == "DISCONNECTING")
        {
            // Updates booleans:
            connectedToServer = false;
            processStarted = false;

            // Updates UI:
            endProcessButton.SetActive(false);

            connectLabel.text = "Disconnected.";
            connectLabel.color = Color.red;
            processStatusLabel.text = "Process Status:";
            errMessLabel.text = "Error Message:";
        }
        else if(serverMessage.Substring(0, 3) == "ERR")
        {
            // Updates UI:
            errMessLabel.text = "Error Message: " + serverMessage.Substring(4);
        }
        else if(!processStarted)
        {
            if(serverMessage == "PROCESS_STARTED")
            {
                // Updates UI:
                processStatusLabel.text = "Process Status: Connected to controller";
                endProcessButton.SetActive(true);

                // Updates boolean:
                processStarted = true;
            }
        }
        else if (serverMessage == "PROCESS_ENDED")
        {
            // Updates UI:
            endProcessButton.SetActive(false);
            processStarted = false;

            processStatusLabel.text = "Process Status: Disconnected from controller";

        }
        else if (serverMessage == "GOING_FORWARD") // If forwards movement occurs
        {
            dataPointHolder.ptpIndex++; // Increments PTP index

            // Updates UI:
            dataPointHolder.pathViabilityLabel.text = "Path Viability: Going forward!";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: F-T";
        }
        else if (serverMessage == "GOING_BACKWARD") // If backwards movement occurs
        {
            dataPointHolder.ptpIndex--; // Decrements PTP index

            // Updates UI:
            dataPointHolder.pathViabilityLabel.text = "Path Viability: Going backward!";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: B-T";
        }
        else if (serverMessage == "CHANGING_TCP") // If TCP is changing
        {
            // Updates UI:
            dataPointHolder.pathViabilityLabel.text = "Path Viability: Updating TCP!";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: U-T";
        }
        else if (serverMessage == "FORWARD_NOT_POSSIBLE") // If requested forward movement is not possible
        {
            // Updates UI:
            dataPointHolder.pathViabilityLabel.text = "Path Viability: Forward movement not possible.";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: F-F";
        }
        else if (serverMessage == "BACKWARD_NOT_POSSIBLE") // If requested backward movement is not possible
        {
            // Updates UI:
            dataPointHolder.pathViabilityLabel.text = "Path Viability: Backward movement not possible.";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: B-F";
        }
        else if (serverMessage == "TCPCHANGE_NOT_POSSIBLE") // If requested TCP change is not possible
        {
            // Updates UI:
            dataPointHolder.pathViabilityLabel.text = "Path Viability: Updating TCP not possible!";
            dataPointHolder.pathViabilityLabelminiMenu.text = "Path Viability: U-F";
        }
        else if(serverMessage.Substring(0, 3) == "PTP") // Receives updated PTP index
        {
            dataPointHolder.ptpIndex = int.Parse(serverMessage.Substring(3)); // Updates PTP index

            // Updates UI:
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
        // Indicates to server to start process

        string clientMessage = "START_PROCESS";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;
    }

    public void EndProcess()
    {
        // Indicates to server to end process

        string clientMessage = "END_PROCESS";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;
    }

    public void GoForward()
    {
        // Indicates to server to attempt to go forwards in the path

        string clientMessage = "GO_FORWARD";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;   
    }

    public void GoBackward()
    {
        // Indicates to server to attempt to go backwards in the path

        string clientMessage = "GO_BACKWARD";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;
    }

    public void changeTCP()
    {
        // Indicates to server to attempt to change the current position and/or orientation of the TCP

        string clientMessage = "CHANGE_TCP";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;
    }

    public void runCPT()
    {
        // Indicates to server to attempt to completely traverse the given path forwards

        //SendDataMessage();
        string clientMessage = "RUN_CPT";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;

    }

    public void runCPTB()
    {
        // Indicates to server to attempt to completely traverse the given path backwards

        //SendDataMessage();
        string clientMessage = "RUN_CPT_BACK";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;

    }

    public void SendDataMessage()
    {
        // Sends all relevant data to the server, which includes points' spatial coordinates, rotations, parameter data, total number of points, and PTP index value

        // Placeholder variables:
        Vector3 curCoor;
        Quaternion curRot;

        string clientMessage = "#"; // Character to indicate transmission of data

        // Including all spatial and rotational coordinate data:
        for (int i = 0; i < dataPointHolder.pointCounter; i++)
        {
            curCoor = dataPointHolder.posArr[i];
            curCoor *= 1000f; // Converts the spatial coordinates of each data point to mm

            clientMessage += "[ " + curCoor.x + ", " + curCoor.y + ", " + curCoor.z + "]/";
            curRot = dataPointHolder.rotArr[i];
            clientMessage += "[ " + curRot.w + ", " + curRot.x + ", " + curRot.y + ", " + curRot.z + "];";
        }

        clientMessage += "@";
        clientMessage += speedSlider.sliderIndexValue; // Including index of selected speed
        clientMessage += "%";
        clientMessage += zoneSlider.sliderIndexValue; // Including index of selected zone
        clientMessage += "%";
        clientMessage += dataPointHolder.pointCounter.ToString(); // Including total number of points
        clientMessage += "%";
        clientMessage += dataPointHolder.ptpIndex.ToString(); // Including PTP index

        // Attempts to send data to the server and indicates to user if connection to the server is not established:
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
        // Indicates to the server to reset the home position of the robot

        string clientMessage = "RESET_HOME";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: Reset home position!";
    }

    public void ClearAllPoints()
    {
        // Indicates to the server to clear all data

        string clientMessage = "CLEAR_DATA";
        SendMessageToServer(clientMessage);
        clientMessLabel.text = "Client Message: " + clientMessage;
    }


}
