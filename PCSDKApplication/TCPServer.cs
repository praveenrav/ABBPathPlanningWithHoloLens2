using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PCSDKApplication
{
    public class TCPServer
    {
        MainWindow mainWindow;

        // TCP Server Information:
        public IPAddress ipAddr; // IP Address of TCP socket
        //public IPAddress remoteAddr = IPAddress.Parse("192.168.86.250"); // Remote IP address of PC
        public int portNum; // Port number of TCP socket
        public TcpListener serverABB; // TCP Server
        public Thread tcpServerThread; // TCP Server Listener Thread
        public TcpClient tcpClient; // TCP Client

        // TCP Message Booleans:
        public bool isConnected = false;

        // TCP Message Holders:
        public string dataPoints; // String containing information of given data points
        public string speedSetting; // String representing selected speed
        public string zoneSetting; // String representing selected zone
        public string[] speedVals = { "v50", "v100", "v150", "v200", "v300", "v400", "v500", "v600", "v800", "v1000"}; // String array containing all possible speed values
        public string[] zoneVals = { "fine", "z0", "z1", "z5", "z10", "z15", "z20", "z30", "z40", "z50", "z60", "z80", "z100", "z150", "z200"}; // String array containing all possible zone values

        public TCPServer(MainWindow mainWin)
        {
            mainWindow = mainWin;
        }

        public void establishServer()
        {
            // Method to establish the TCP Server

            // Retrieves inputted IP address and port number:
            ipAddr = IPAddress.Parse(mainWindow.IPAddressTextBox.Text);
            portNum = (int)Int32.Parse(mainWindow.PortTextBox.Text);

            // Starts TCP Server thread:
            tcpServerThread = new Thread(new ThreadStart(ListenForData));
            tcpServerThread.IsBackground = true;
            tcpServerThread.Start();

            // Updates UI:
            mainWindow.Progress1.Text = "Connecting...";

        }

        public void ListenForData()
        {
            /* Method to listen for messages from Unity client */

            try
            {
                serverABB = new TcpListener(ipAddr, portNum); // Initializes TcpListener on the given socket
                serverABB.Start(); // Initiates the process of listening for messages from Unity client

                //Include "Server is listening..." here

                System.Byte[] bytes = new byte[32768]; // Stores information received from Unity client

                while (true)
                {
                    using (tcpClient = serverABB.AcceptTcpClient())
                    {
                        if (tcpClient.Connected)
                        {
                            mainWindow.Invoke(new MethodInvoker(delegate () { mainWindow.Progress1.Text = "Connected to client."; }));

                            // If server had just become connected to the client:
                            if (!isConnected)
                            {
                                SendMessage("Connected."); // Sends message to Unity client indicating success of client-server connection
                                isConnected = true; // Sets connection boolean to true
                                mainWindow.Invoke(new MethodInvoker(delegate () { mainWindow.abbInterface.StartProcess(); })); // Automatically connects to controller once connected
                            }

                        }
                        else
                        {
                            mainWindow.Invoke(new MethodInvoker(delegate () { mainWindow.Progress1.Text = "Disconnected from client."; })); // Updates UI
                        }

                        using (NetworkStream stream = tcpClient.GetStream())
                        {
                            int length;

                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                // Stores incoming information into previously-initialized byte array:
                                var incomingData = new byte[length];
                                Array.Copy(bytes, 0, incomingData, 0, length);

                                string clientMessage = Encoding.ASCII.GetString(incomingData);
                                mainWindow.Invoke(new MethodInvoker(delegate () { mainWindow.clientMessLabel.Text = clientMessage; })); // Displays client message
                                mainWindow.Invoke(new MethodInvoker(delegate () { processClientMessage(clientMessage); })); // Processes client message
                            }
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                MessageBox.Show("SocketException" + socketException.ToString());
            }
        }

        public void SendMessage(string serverMessage)
        {
            /* Method to send messages to Unity client: */

            // If no client is connected:
            if (tcpClient == null)
            {
                MessageBox.Show("No client is connected!");
                return;
            }

            try
            {
                NetworkStream stream = tcpClient.GetStream();

                if (stream.CanWrite)
                {
                    byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                    stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                }
            }
            catch (SocketException socketException)
            {
                mainWindow.errMessLabel.Text = "Socket exception: " + socketException;
                //MessageBox.Show("Socket exception: " + socketException);
            }

        }

        public void processClientMessage(string clientMessage)
        {
            /* This method maps out the overall process flow of the path-planning operation: */

            mainWindow.clientMessLabel.Text = clientMessage;
            
            // Starts process:
            if (!mainWindow.abbInterface.connectedToController1 && !mainWindow.abbInterface.connectedToController2)
            {
                if (clientMessage == "START_PROCESS")
                {
                    mainWindow.abbInterface.StartProcess();
                }
            }
            else
            {
                // The following code is run WHILE the application is connected to the controllers and WHILE the RAPID programs on the controllers are running

                // If client wishes to end the process:
                if (clientMessage == "END_PROCESS")
                {
                    mainWindow.abbInterface.DisconnectFromController(0);
                    mainWindow.abbInterface.DisconnectFromController(1);
                    SendMessage("PROCESS_ENDED");
                    mainWindow.servMessLabel.Text = "PROCESS_ENDED";
                }
                // If client sends data:
                else if (clientMessage[0].ToString() == "#")
                {
                    // Receives all data and sends data to both the real (or simulation of real) and virtual controllers
                    receiveDataParams(clientMessage);
                    mainWindow.abbInterface.SendDataParamsRAPID(0);
                    mainWindow.abbInterface.SendDataParamsRAPID(1);

                }
                // If user wishes to move forward in the path:
                else if (clientMessage == "GO_FORWARD")
                {
                    mainWindow.abbInterface.goForward();
                }
                // If user wishes to move backwards in the path:
                else if (clientMessage == "GO_BACKWARD")
                {
                    mainWindow.abbInterface.goBackward();
                }
                // If user wishes to change the TCP's current position and/or orientation:
                else if (clientMessage == "CHANGE_TCP")
                {
                    mainWindow.abbInterface.changeTCP();
                }
                // If user wishes to completely traverse the given path forwards:
                else if (clientMessage == "RUN_CPT")
                {
                    mainWindow.abbInterface.completelyTraverseForward(); // Attempting to completely traverse forward
                    Thread.Sleep(7000); // Pauses the thread for 7 seconds to allow for the path traversal operations to be completed
                    mainWindow.abbInterface.checkPTPIndexRAPID(); // Checking current PTP index
                    SendMessage("PTP" + mainWindow.abbInterface.ptpIndex.ToString()); // Sending updated PTP index to Unity client

                }
                // If user wishes to completely traverse the given path backwards:
                else if (clientMessage == "RUN_CPT_BACK")
                {
                    mainWindow.abbInterface.completelyTraverseBack(); // Attempting to completely traverse backwards
                    Thread.Sleep(7000); // Pauses the thread for 7 seconds to allow for the path traversal operations to be completed
                    mainWindow.abbInterface.checkPTPIndexRAPID(); // Checking current PTP index
                    SendMessage("PTP" + mainWindow.abbInterface.ptpIndex.ToString()); // Sending updated PTP index to Unity client

                }
                // If user wishes to reset the home position of the robots:
                else if (clientMessage == "RESET_HOME")
                {
                    mainWindow.abbInterface.setHomePositionRAPID(); // Sets the robots' home positions
                    SendMessage("Reset home position!"); // Sends message to client indicating success of resetting home position
                    mainWindow.servMessLabel.Text = "Reset home position!"; // Updaitng UI
                }
                // If user wishes to clear all data:
                else if (clientMessage == "CLEAR_DATA")
                {
                    mainWindow.abbInterface.clearDataRAPID();
                }



            }
        }

        public void receiveDataParams(string dataMessage)
        {
            /* Method to receive data and parameters from Unity */

            string[] splitDataParams; // String array representing the data + parameters split into two different elements in an array

            string paramSettings; // String representing paramter information
            string[] splitParams; // String array representing parameters split into different elements in an array

            string[] splitData; // String array representing string coordinate data split into different elements in an array

            string curCoor; // String representing each data point
            string[] splitCoorData; // Coordinate data divided into corresponding positional and rotational components

            string curPos; // String representing position data of current point
            string[] curPosArray;  // String array representing a single data point whose individual spatial coordinates are separated into different elements in an array

            string curRot; // String representing rotation data of current point
            string[] curRotArray; // String array representing a single data point whose individual rotational coordinates are separated into different elements in an array

            char[] dataDelimiters = { '[', ']' };

            if (dataMessage[0].ToString() == "#") // Sees if string has appropriate start character
            {
                dataMessage = dataMessage.Substring(1); // Removes the start character
                splitDataParams = dataMessage.Split('@'); // Splits the coordinate data and parameter data

                // Interpreting data points:
                dataPoints = splitDataParams[0];

                splitData = dataPoints.Split(';');

                // Clearing the list views:
                mainWindow.positionListView.Items.Clear();
                mainWindow.rotationListView.Items.Clear();

                for (int i = 0; i < (splitData.Length - 1); i++)
                {
                    curCoor = splitData[i];
                    splitCoorData = curCoor.Split('/');

                    // Adding position values:
                    curPos = splitCoorData[0].Trim(dataDelimiters);
                    curPosArray = curPos.Split(',');

                    ListViewItem coorText = new ListViewItem(curPosArray[0]);
                    coorText.SubItems.Add(curPosArray[1]);
                    coorText.SubItems.Add(curPosArray[2]);
                    mainWindow.positionListView.Items.Add(coorText);


                    // Adding rotation values:
                    curRot = splitCoorData[1].Trim(dataDelimiters);
                    curRotArray = curRot.Split(',');

                    ListViewItem rotText = new ListViewItem(curRotArray[0]);
                    rotText.SubItems.Add(curRotArray[1]);
                    rotText.SubItems.Add(curRotArray[2]);
                    rotText.SubItems.Add(curRotArray[3]);
                    mainWindow.rotationListView.Items.Add(rotText);

                }

                // Interpreting parameter settings:
                paramSettings = splitDataParams[1];
                splitParams = paramSettings.Split('%');

                // Receiving parameter information:
                Int32.TryParse(splitParams[0], out mainWindow.abbInterface.speedInt);
                Int32.TryParse(splitParams[1], out mainWindow.abbInterface.zoneInt);
                Int32.TryParse(splitParams[2], out mainWindow.abbInterface.total_num);
                Int32.TryParse(splitParams[3], out mainWindow.abbInterface.ptpIndex);

                // Updating UI:
                mainWindow.speedBox.Text = speedVals[mainWindow.abbInterface.speedInt];
                mainWindow.zoneBox.Text = zoneVals[mainWindow.abbInterface.zoneInt];
                mainWindow.pointCounterLabelValue.Text = splitParams[2];
                mainWindow.ptpIndexLabel.Text = splitParams[3];

                SendMessage("Received data!"); // Sends confirmation message to Unity client

            }
        }



    }
}
