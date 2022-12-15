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
        public IPAddress localAddr;
        public IPAddress remoteAddr = IPAddress.Parse("192.168.86.250"); // Remote IP address of PC
        public int portNum; // Port number to which the TCP server and client connect (CHANGE LATER)
        public TcpListener serverABB; // TCP Server
        public Thread tcpServerThread; // TCP Server Listener Thread
        public TcpClient tcpClient; // TCP Client

        // TCP Message Booleans:
        public bool isConnected = false;

        // TCP Message Holders:
        public string dataPoints;
        public string speedSetting;
        public string zoneSetting;


        public TCPServer(MainWindow mainWin)
        {
            mainWindow = mainWin;
        }

        public void establishServer()
        {
            localAddr = IPAddress.Parse(mainWindow.IPAddressTextBox.Text);
            portNum = (int)Int32.Parse(mainWindow.PortTextBox.Text);

            tcpServerThread = new Thread(new ThreadStart(ListenForData));
            tcpServerThread.IsBackground = true;
            tcpServerThread.Start();

            mainWindow.Progress1.Text = "Connecting...";

        }

        public void ListenForData()
        {
            try
            {
                serverABB = new TcpListener(localAddr, portNum);
                serverABB.Start();

                //Include "Server is listening..." here

                System.Byte[] bytes = new byte[32768];

                while (true)
                {
                    using (tcpClient = serverABB.AcceptTcpClient())
                    {
                        if (tcpClient.Connected)
                        {
                            mainWindow.Invoke(new MethodInvoker(delegate () { mainWindow.Progress1.Text = "Connected to client."; }));

                            if (!isConnected)
                            {
                                SendMessage("Connected.");
                                isConnected = true; // Sets boolean to true
                                mainWindow.Invoke(new MethodInvoker(delegate () { mainWindow.abbInterface.StartProcess(); })); // Automatically connects to controller once connected
                            }

                        }
                        else
                        {
                            mainWindow.Invoke(new MethodInvoker(delegate () { mainWindow.Progress1.Text = "Disconnected from client."; }));
                        }

                        using (NetworkStream stream = tcpClient.GetStream())
                        {
                            int length;

                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
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
            // This method maps out the overall process flow of the path-planning operation:

            mainWindow.clientMessLabel.Text = clientMessage;

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
                else if (clientMessage[0].ToString() == "#")
                {
                    receiveDataParams(clientMessage);
                    mainWindow.abbInterface.SendDataParamsRAPID(0);
                    mainWindow.abbInterface.SendDataParamsRAPID(1);

                }
                else if (clientMessage == "GO_FORWARD")
                {
                    mainWindow.abbInterface.goForward();
                }
                else if (clientMessage == "GO_BACKWARD")
                {
                    mainWindow.abbInterface.goBackward();
                }
                else if (clientMessage == "ROTATE_TCP")
                {
                    mainWindow.abbInterface.changeTCPOrient();
                }
                else if (clientMessage == "RUN_CPT")
                {
                    mainWindow.abbInterface.completelyTraverseForward();
                    Thread.Sleep(7000);
                    mainWindow.abbInterface.checkPTPIndexRAPID();
                    SendMessage("PTP" + mainWindow.abbInterface.ptpIndex.ToString());

                    /*
                    isPathViable = false;
                    CheckPathViability();

                    if (isPathViable)
                    {
                        executeValidatedCPTPath();
                        isPathViable = false; // Resets path viability

                        SendMessage("CPT-T");
                        servMessLabel.Text = "CPT-T";
                    }
                    else
                    {
                        SendMessage("CPT-F");
                        servMessLabel.Text = "CPT-F";
                    }
                    */
                }
                else if (clientMessage == "RUN_CPT_BACK")
                {
                    mainWindow.abbInterface.completelyTraverseBack();
                    Thread.Sleep(7000);
                    mainWindow.abbInterface.checkPTPIndexRAPID();
                    SendMessage("PTP" + mainWindow.abbInterface.ptpIndex.ToString());

                    /*
                    completelyTraverseBack();
                    SendMessage("CPTB-T");
                    servMessLabel.Text = "CPTB-T";
                    */
                }
                else if (clientMessage == "RESET_HOME")
                {
                    mainWindow.abbInterface.setHomePositionRAPID();
                    SendMessage("Reset home position!");
                    mainWindow.servMessLabel.Text = "Reset home position!";
                }
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

            string paramSettings;
            string[] splitParams; // String array representing parameters split into different elements in an array

            string[] splitData; // String array representing string coordinate data split into different elements in an array

            string curCoor;
            string[] splitCoorData;

            string curPos;
            string[] curPosArray;  // String array representing a single data point whose individual spatial coordinates are separated into different elements in an array

            string curRot;
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
                speedSetting = splitParams[0];
                zoneSetting = splitParams[1];
                Int32.TryParse(splitParams[2], out mainWindow.abbInterface.total_num);
                Int32.TryParse(splitParams[3], out mainWindow.abbInterface.ptpIndex);

                // Updating UI:
                mainWindow.speedBox.Text = speedSetting;
                mainWindow.zoneBox.Text = zoneSetting;
                mainWindow.pointCounterLabelValue.Text = splitParams[2];
                mainWindow.ptpIndexLabel.Text = splitParams[3];

                SendMessage("Received data!");

            }
        }



    }
}
