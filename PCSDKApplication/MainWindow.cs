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
    public partial class MainWindow : Form
    {
        // ABB Controller Information:
        private NetworkScanner scanner = null;
        private ControllerInfoCollection controllers = null;
        private Controller controller1 = null;
        private Controller controller2 = null;
        private ABB.Robotics.Controllers.RapidDomain.Task[] tasks = null;
        private NetworkWatcher networkwatcher = null;
        private bool isTesting = true;

        ///// ABB RAPID Information:
        private Bool completePathTraversalBool;
        private Bool completePathTraversalBackBool;
        private Bool goForwardBool;
        private Bool goBackwardBool;
        private Bool changeTCPOrientBool;
        private Bool isFirstBool;
        private Bool isClearedBool;
        private Bool isHomePosReset;
        private Num num_targets; // ABB num variable used to indicate the number of inputted coordinates
        private Num ptpIndexABB; // ABB num variable used to indicate the index of the next point to be traversed when point-to-point mode is enabled
        private RobTarget initRobotPos;
        private ABB.Robotics.Controllers.RapidDomain.String speed_RAPID; // ABB string used to indicate the value of the speed parameter
        private ABB.Robotics.Controllers.RapidDomain.String zone_RAPID; // ABB string used to indicate the value of the zone parameter
       
        public int ptpIndex = 1; // Index representing the next point to traverse to
        public int total_num; // Total number of input coordinates
        public int total_count;


        // TCP Server Information:
        private IPAddress localAddr;
        private IPAddress remoteAddr = IPAddress.Parse("192.168.86.250"); // Remote IP address of PC
        private int portNum; // Port number to which the TCP server and client connect (CHANGE LATER)
        private TcpListener serverABB; // TCP Server
        private Thread tcpServerThread; // TCP Server Listener Thread
        private TcpClient tcpClient; // TCP Client

        // TCP Message Booleans:
        private bool isConnected = false;
        private bool connectedToController1 = false;
        private bool connectedToController2 = false;
        private bool startedRAPIDProgram1 = false;
        private bool startedRAPIDProgram2 = false;
        private bool isPathViable = false;

        // TCP Message Holders:
        public string dataPoints;
        public string speedSetting;
        public string zoneSetting;

        public string contrErrMess = "";
        public string rapidErrMess = "";

        //private int contrNum; // Number representing the controller that the server is connected to
        //private bool sentControllers = false;


        public MainWindow()
        {
            InitializeComponent();
            findControllersFirst();
        }

        private void tcpButton_Click(object sender, EventArgs e)
        {
            establishServer();
        }

        public void establishServer()
        {

            localAddr = IPAddress.Parse(IPAddressTextBox.Text);
            portNum = (int) Int32.Parse(PortTextBox.Text);
            
            tcpServerThread = new Thread(new ThreadStart(ListenForData));
            tcpServerThread.IsBackground = true; 
            tcpServerThread.Start();

            Progress1.Text = "Connecting...";

        }

        private void ListenForData()
        {
            try
            {
                serverABB = new TcpListener(localAddr, portNum);
                serverABB.Start();

                //Include "Server is listening..." here

                System.Byte[] bytes = new byte[32768];

                while(true)
                {
                    using (tcpClient = serverABB.AcceptTcpClient())
                    {
                        if(tcpClient.Connected)
                        {
                            this.Invoke(new MethodInvoker(delegate () { Progress1.Text = "Connected to client."; }));

                            if(!isConnected)
                            {
                                SendMessage("Connected.");
                                isConnected = true; // Sets boolean to true
                                this.Invoke(new MethodInvoker(delegate () { StartProcess(); })); // Automatically connects to controller once connected
                            }

                        }
                        else
                        {
                            this.Invoke(new MethodInvoker(delegate () { Progress1.Text = "Disconnected from client."; }));
                        }

                        using (NetworkStream stream = tcpClient.GetStream())
                        {
                            int length;

                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                var incomingData = new byte[length];
                                Array.Copy(bytes, 0, incomingData, 0, length);

                                string clientMessage = Encoding.ASCII.GetString(incomingData);
                                this.Invoke(new MethodInvoker(delegate () { clientMessLabel.Text = clientMessage; })); // Displays client message
                                this.Invoke(new MethodInvoker(delegate () { processClientMessage(clientMessage); })); // Processes client message
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

        private void SendMessage(string serverMessage)
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
                errMessLabel.Text = "Socket exception: " + socketException;
                //MessageBox.Show("Socket exception: " + socketException);
            }

        }

        private void processClientMessage(string clientMessage)
        {
            // This method maps out the overall process flow of the path-planning operation:

            clientMessLabel.Text = clientMessage;

            if(!connectedToController1 && !connectedToController2)
            {
                if (clientMessage == "START_PROCESS")
                {
                    StartProcess();
                }
            }
            else
            {
                // The following code is run WHILE the application is connected to the controllers and WHILE the RAPID programs on the controllers are running

                // If client wishes to end the process:
                if (clientMessage == "END_PROCESS")
                {
                    DisconnectFromController(0);
                    DisconnectFromController(1);
                    SendMessage("PROCESS_ENDED");
                    servMessLabel.Text = "PROCESS_ENDED";
                }
                else if (clientMessage[0].ToString() == "#")
                {
                    receiveDataParams(clientMessage);
                    SendDataParamsRAPID(0);
                    SendDataParamsRAPID(1);

                }
                else if (clientMessage == "GO_FORWARD")
                {
                    goForward();
                }
                else if (clientMessage == "GO_BACKWARD")
                {
                    goBackward();
                }
                else if (clientMessage == "ROTATE_TCP")
                {
                    changeTCPOrient();
                }
                else if (clientMessage == "RUN_CPT")
                {
                    completelyTraverseForward();
                    Thread.Sleep(7000);
                    checkPTPIndexRAPID();
                    SendMessage("PTP" + ptpIndex.ToString());

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
                    completelyTraverseBack();
                    Thread.Sleep(7000);
                    checkPTPIndexRAPID();
                    SendMessage("PTP" + ptpIndex.ToString());

                    /*
                    completelyTraverseBack();
                    SendMessage("CPTB-T");
                    servMessLabel.Text = "CPTB-T";
                    */
                }
                else if(clientMessage == "RESET_HOME")
                {
                    setHomePositionRAPID();
                    SendMessage("Reset home position!");
                    servMessLabel.Text = "Reset home position!";
                }
                else if (clientMessage == "CLEAR_DATA")
                {
                    clearDataRAPID();
                }


                
            }
        }


        private void StartProcess()
        {
            if(isTesting)
            {
                connectToController(0); // Connecting to real-life ABB GoFa controller
                connectToController(1); // Connecting to virtual ABB GoFa controller (digital twin)
            }
            else
            {
                connectToControllers();
            }


            if (connectedToController1 && connectedToController2)
            {
                StartRapidProgram(0); // Starts RAPID Program for real-life ABB GoFa
                Thread.Sleep(1000);
                StartRapidProgram(1); // Starts RAPID Program for digital twin

                if (startedRAPIDProgram1 && startedRAPIDProgram2)
                {
                    SendMessage("PROCESS_STARTED");
                    servMessLabel.Text = "PROCESS_STARTED";
                    setInitDigitalTwinPos(); // Sets the initial position of the digital twin
                }
                else
                {
                    SendMessage("ERR " + rapidErrMess);
                    servMessLabel.Text = "Error with starting RAPID Program: " + rapidErrMess;
                }
            }
            else
            {
                SendMessage("ERR " + contrErrMess);
                servMessLabel.Text = contrErrMess;
            }
        }

        private void connectToController(int num)
        {
            // Method utilized by the HoloLens 2 to connect to an ABB controller

            ControllerInfo selectContr = controllers[num];
            Controller controller;

            if (selectContr.Availability == ABB.Robotics.Controllers.Availability.Available)
            {

                // Attempts to connect to the controller:
                try
                {
                    if(isTesting)
                    {
                        if (num == 0)
                        {
                            this.controller1 = Controller.Connect(selectContr.SystemId, ConnectionType.Standalone, false);
                            this.controller1.Logon(UserInfo.DefaultUser);
                            controller = this.controller1;
                        }
                        else
                        {
                            this.controller2 = Controller.Connect(selectContr.SystemId, ConnectionType.Standalone, false);
                            this.controller2.Logon(UserInfo.DefaultUser);
                            controller = this.controller2;
                        }
                    }
                    else
                    {
                        if (!selectContr.IsVirtual)
                        {
                            this.controller1 = Controller.Connect(selectContr.SystemId, ConnectionType.Standalone, false);
                            this.controller1.Logon(UserInfo.DefaultUser);
                            controller = this.controller1;
                        }
                        else
                        {
                            this.controller2 = Controller.Connect(selectContr.SystemId, ConnectionType.Standalone, false);
                            this.controller2.Logon(UserInfo.DefaultUser);
                            controller = this.controller2;
                        }
                    }
                    
                    // Operations that run when connected to the controller
                    if (controller.Connected == true)
                    {
                        if (!isTesting)
                        {
                            if (!selectContr.IsVirtual)
                            {
                                connectedToController1 = true;

                                // Updating UI:
                                contrConnectionStatus1.Text = "Connected";
                                contrConnectionStatus1.ForeColor = Color.Green;

                            }
                            else
                            {
                                connectedToController2 = true;

                                // Updating UI:
                                contrConnectionStatus2.Text = "Connected";
                                contrConnectionStatus2.ForeColor = Color.Green;
                            }
                        }
                        else
                        {
                            if (num == 0)
                            {
                                connectedToController1 = true;

                                // Updating UI:
                                contrConnectionStatus1.Text = "Connected";
                                contrConnectionStatus1.ForeColor = Color.Green;

                            }
                            else if (num == 1)
                            {
                                connectedToController2 = true;

                                // Updating UI:
                                contrConnectionStatus2.Text = "Connected";
                                contrConnectionStatus2.ForeColor = Color.Green;
                            }
                        }

                        // Updating UI:
                        if(connectedToController1 && connectedToController2)
                        {
                            startRapidProgButton.Enabled = true;
                            stopRAPIDProgButton.Enabled = true;
                        }
                        

                    }
                }
                catch (Exception ex)
                {
                    contrErrMess = "ERR " + ex.ToString();
                    //SendMessage("ERR " + ex.ToString());
                    //servMessLabel.Text = "ERR " + ex.ToString();
                }

            }
            else
            {
                string denyMessage = "Selected controller not available.";
                SendMessage(denyMessage);
                servMessLabel.Text = denyMessage;
            }
        }

        private void connectToControllers()
        {
            // Method utilized by the HoloLens 2 to connect to both the real and virtual ABB controllers

            try
            {
                ControllerInfo selectContr1 = controllers[0];
                ControllerInfo selectContr2 = controllers[1];

                if (selectContr1.Availability == ABB.Robotics.Controllers.Availability.Available && selectContr2.Availability == ABB.Robotics.Controllers.Availability.Available)
                {

                    // Attempts to connect to the controller:
                    try
                    {
                        
                        if (!selectContr1.IsVirtual)
                        {
                            this.controller1 = Controller.Connect(selectContr1.SystemId, ConnectionType.Standalone, false);
                            this.controller1.Logon(UserInfo.DefaultUser);

                            this.controller2 = Controller.Connect(selectContr2.SystemId, ConnectionType.Standalone, false);
                            this.controller2.Logon(UserInfo.DefaultUser);
                        }
                        else
                        {
                            this.controller1 = Controller.Connect(selectContr2.SystemId, ConnectionType.Standalone, false);
                            this.controller1.Logon(UserInfo.DefaultUser);

                            this.controller2 = Controller.Connect(selectContr1.SystemId, ConnectionType.Standalone, false);
                            this.controller2.Logon(UserInfo.DefaultUser);
                        }


                        // Operations that run when connected to the controller
                        if (this.controller1.Connected == true)
                        {
                            connectedToController1 = true;

                            // Updating UI:
                            contrConnectionStatus1.Text = "Connected";
                            contrConnectionStatus1.ForeColor = Color.Green;

                        }

                        if (this.controller2.Connected == true)
                        {
                            connectedToController2 = true;

                            // Updating UI:
                            contrConnectionStatus2.Text = "Connected";
                            contrConnectionStatus2.ForeColor = Color.Green;
                        }

                        if (connectedToController1 && connectedToController2)
                        {
                            // Updating UI:
                            startRapidProgButton.Enabled = true;
                            stopRAPIDProgButton.Enabled = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        contrErrMess = "ERR " + ex.ToString();
                        //SendMessage("ERR " + ex.ToString());
                        //servMessLabel.Text = "ERR " + ex.ToString();
                    }

                }
                else
                {
                    string denyMessage = "Selected controllers not available.";
                    SendMessage(denyMessage);
                    servMessLabel.Text = denyMessage;
                }
            }
            catch
            {
                string denyMessage = "The two controllers are not present on the network!";
                SendMessage(denyMessage);
                servMessLabel.Text = denyMessage;
            }

        }

        private void DisconnectFromController(int num)
        {
            Controller controller;
            
            if(num == 0)
            {
                controller = this.controller1;
            }
            else
            {
                controller = this.controller2;
            }


            if (controller != null)
            {
                // End any currently-running RAPID programs:
                if (controller.OperatingMode == ControllerOperatingMode.Auto)
                {
                    StopRapidProgram(num);
                }

                try
                {
                    controller.Logoff();
                    controller.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }

                if (num == 0)
                {
                    this.controller1 = null;
                    connectedToController1 = false;

                    // Updating Form UI:
                    contrConnectionStatus1.Text = "Not connected";
                    contrConnectionStatus1.ForeColor = Color.IndianRed;
                }
                else if (num == 1)
                {
                    this.controller2 = null;
                    connectedToController2 = false;

                    // Updating Form UI:
                    contrConnectionStatus2.Text = "Not connected";
                    contrConnectionStatus2.ForeColor = Color.IndianRed;
                }

                if (this.controller1 == null && this.controller2 == null)
                {
                    // Updating Form UI:
                    startRapidProgButton.Enabled = false;
                    stopRAPIDProgButton.Enabled = false;
                }

            }
            else
            {
                //MessageBox.Show("No controller is connected!");
            }
        }

        private void StartRapidProgram(int num)
        {
            Controller controller;

            if (num == 0)
            {
                controller = this.controller1;
            }
            else
            {
                controller = this.controller2;
            }

            try
            {
                if (controller.OperatingMode == ControllerOperatingMode.Auto)
                {
                    tasks = controller.Rapid.GetTasks(); // Gets all tasks from current controller
                    using (Mastership m = Mastership.Request(controller.Rapid))
                    {
                        UserAuthorizationSystem uas = controller.AuthenticationSystem; // Used to check for grants

                        if (uas.CheckDemandGrant(Grant.LoadRapidProgram) && uas.CheckDemandGrant(Grant.ExecuteRapid))
                        {
                            ABB.Robotics.Controllers.RapidDomain.Task aTask = tasks[0];
                            aTask.ResetProgramPointer(); // Resets the program pointer to the main entry point of the RobotStudio task

                            // Turns the robot's motors on if they were initially off:
                            if (controller.State == ControllerState.MotorsOff)
                            {
                                controller.State = ControllerState.MotorsOn;
                            }

                            StartResult result = controller.Rapid.Start(true); // Starts the RAPID task

                            // Conditional statement that runs if RAPID program was not able to be successfully started:
                            if (result.ToString() != "Ok")
                            {
                                //MessageBox.Show("Start Failed: " + result.ToString());
                                rapidErrMess = "Start Failed: " + result.ToString();
                            }
                            else
                            {
                                rapidErrMess = "";

                                // Updating UI:
                                if (num == 0)
                                {
                                    rapidProgramStatus1.Text = "Running!";
                                    rapidProgramStatus1.ForeColor = Color.Green;
                                    startedRAPIDProgram1 = true;

                                }
                                else if (num == 1)
                                {
                                    rapidProgramStatus2.Text = "Running!";
                                    rapidProgramStatus2.ForeColor = Color.Green;
                                    startedRAPIDProgram2 = true;
                                }

                                if (startedRAPIDProgram1 && startedRAPIDProgram2)
                                {
                                    goForwardButton.Enabled = true;
                                    goBackwardButton.Enabled = true;
                                    CPTButton.Enabled = true;
                                    CPTBButton.Enabled = true;
                                    clearDataButton.Enabled = true;
                                    sendDataButton.Enabled = true;
                                }
                            }
                        }
                        else
                        {
                            rapidErrMess = "You don't have the proper grants!";
                            //MessageBox.Show("Error: You don't have the proper grants!");
                        }

                    }
                }
                else
                {
                    rapidErrMess = "Automatic mode is required to start execution from a remote client.";
                    //MessageBox.Show("Automatic mode is required to start execution from a remote client.");
                }
            }
            catch (System.InvalidOperationException ex)
            {
                rapidErrMess = "Mastership is held by another client." + ex.Message;
                //MessageBox.Show("Mastership is held by another client." + ex.Message);
            }
            catch (System.Exception ex)
            {
                rapidErrMess = "Unexpected error occurred: " + ex.Message;
                //MessageBox.Show("Unexpected error occurred: " + ex.Message);
            }
        }

        private void StopRapidProgram(int num)
        {
            Controller controller;

            if (num == 0)
            {
                controller = this.controller1;
            }
            else
            {
                controller = this.controller2;
            }

            try
            {
                if (controller.OperatingMode == ControllerOperatingMode.Auto)
                {
                    using (Mastership m = Mastership.Request(controller.Rapid))
                    {
                        controller.Rapid.Stop();

                        // Updating UI:
                        if(num == 0)
                        {
                            rapidProgramStatus1.Text = "Not running";
                            rapidProgramStatus1.ForeColor = Color.IndianRed;
                            startedRAPIDProgram1 = false;
                        }
                        else if(num == 1)
                        {
                            rapidProgramStatus2.Text = "Not running";
                            rapidProgramStatus2.ForeColor = Color.IndianRed;
                            startedRAPIDProgram2 = false;
                        }

                        goForwardButton.Enabled = false;
                        goBackwardButton.Enabled = false;
                        CPTButton.Enabled = false;
                        CPTBButton.Enabled = false;
                        clearDataButton.Enabled = false;
                        sendDataButton.Enabled = false;

                        // Resetting UI:
                        positionListView.Items.Clear();
                        rotationListView.Items.Clear();
                        speedBox.Text = "";
                        zoneBox.Text = "";
                        ptpIndex = 1;
                        ptpIndexLabel.Text = "1";
                        pathViabilityLabelValue.Text = "NOT CHECKED";
                    }
                }
                else
                {
                    MessageBox.Show("Automatic mode is required to start execution from a remote client.");
                }
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show("Mastership is held by another client." + ex.Message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Unexpected error occurred: " + ex.Message);
            }
        }

        private void SendDataParamsRAPID(int num)
        {
            // This method sends the coordinate data and corresponding parameters to the selected controller
            // If the selected controller is the digital twin, the path is attempted to be automatically run

            string x_coor, y_coor, z_coor; // Placeholder strings for each input coordinate
            string w_rot, x_rot, y_rot, z_rot; // Placeholder strings for each input rotation


            if(isConnected)
            {
                total_count = total_num;
            }
            else
            {
                total_count = this.positionListView.Items.Count;
            }

            num_targets.Value = total_count; // Total number of targets
            ptpIndexABB.Value = ptpIndex;

            Controller controller;

            if(num == 0)
            {
                controller = this.controller1;
            }
            else
            {
                controller = this.controller2;
            }

            ABB.Robotics.Controllers.RapidDomain.Task tRob1 = controller.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                
                // RAPID Data:
                RapidData speedVal_RAPID = tRob1.GetRapidData("PointTraversal", "speedStr");
                RapidData zoneVal_RAPID = tRob1.GetRapidData("PointTraversal", "zoneStr");
                RapidData ptpIndex_RAPID = tRob1.GetRapidData("PointTraversal", "ptpIndex");
                RapidData targetsNum_RAPID = tRob1.GetRapidData("PointTraversal", "targetsNum");
                RapidData tgPos_RAPID = tRob1.GetRapidData("PointTraversal", "tgPos");
                RapidData tgRot_RAPID = tRob1.GetRapidData("PointTraversal", "tgRot");

                ///*** Setting Parameter + Setting Values: ***///
                speed_RAPID.Value = this.speedBox.Text; // RAPID String object
                zone_RAPID.Value = this.zoneBox.Text; // RAPID String object

                if((speedVal_RAPID.Value is ABB.Robotics.Controllers.RapidDomain.String) && (zoneVal_RAPID.Value is ABB.Robotics.Controllers.RapidDomain.String) && (ptpIndex_RAPID.Value is Num) && (targetsNum_RAPID.Value is Num))
                {
                    using(Mastership m = Mastership.Request(controller.Rapid))
                    {
                        UserAuthorizationSystem uas = controller.AuthenticationSystem;

                        if(uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            speedVal_RAPID.Value = speed_RAPID;
                            zoneVal_RAPID.Value = zone_RAPID;
                            ptpIndex_RAPID.Value = ptpIndexABB;
                            targetsNum_RAPID.Value = num_targets;
                        }
                    }
                }

                ///*** Looping through each inputted coordinate: ***///
                for (int i = 0; i < total_count; i++)
                {
                    // Extracting coordinate data from each listview element:
                    ListViewItem item_pos = this.positionListView.Items[i];
                    x_coor = item_pos.SubItems[0].Text;
                    y_coor = item_pos.SubItems[1].Text;
                    z_coor = item_pos.SubItems[2].Text;

                    // Creating new Pos datatype:
                    Pos rt = new Pos();
                    rt.FillFromString2("[" + x_coor + "," + y_coor + "," + z_coor + "]");

                    
                    ListViewItem item_rot = this.rotationListView.Items[i];
                    w_rot = item_rot.SubItems[0].Text;
                    x_rot = item_rot.SubItems[1].Text;
                    y_rot = item_rot.SubItems[2].Text;
                    z_rot = item_rot.SubItems[3].Text;

                    Orient rot = new Orient();
                    rot.FillFromString2("[" + w_rot + "," + x_rot + "," + y_rot + "," + z_rot + "]");

                    // Sending coordinate data to RAPID:
                    if (tgPos_RAPID.IsArray)
                    {
                        using (Mastership m = Mastership.Request(controller.Rapid))
                        {
                            UserAuthorizationSystem uas = controller.AuthenticationSystem;

                            if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                            {
                                tgPos_RAPID.WriteItem(rt, i);
                                tgRot_RAPID.WriteItem(rot, i);
                            }
                        }
                    }
                }
            }
        }

        /*
        private void HandleStartFlagChanged(object sender, DataValueChangedEventArgs e)
        {
            this.Invoke(new EventHandler<DataValueChangedEventArgs>(changeBoolean), new Object[] { this, e });
            //this.Invoke(new EventHandler<DataValueChangedEventArgs>(changeBoolean), sender, e);
        }

        private void changeBoolean(object sender, DataValueChangedEventArgs e)
        {
            //this.TCPWindowLabel.Text = "Works";
            testBool = true;
            //this.Invoke(new MethodInvoker(delegate () { this.TCPWindowLabel.Text = "Works"; }));
        }
        */

        // Movement-Specific RAPID Methods:
        private void goForward()
        {
            // Running the path on the real controller:
            ABB.Robotics.Controllers.RapidDomain.Task tRob1 = controller1.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData goForward_RAPID = tRob1.GetRapidData("PointTraversal", "goForward"); // Boolean from RAPID
                RapidData isTargetViableF_RAPID = tRob1.GetRapidData("PointTraversal", "isTargetViableF");

                if (goForward_RAPID.Value is Bool && isTargetViableF_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller1.Rapid))
                    {
                        UserAuthorizationSystem uas = controller1.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            Bool isTargetViableF_ABB = (Bool) isTargetViableF_RAPID.Value;
                            bool isTargetViableF = isTargetViableF_ABB.Value;
                            if(isTargetViableF)
                            {
                                goForwardBool.Value = true;
                                goForward_RAPID.Value = goForwardBool;
                                ptpIndex++; // Incrementing the PTP index
                                ptpIndexLabel.Text = ptpIndex.ToString();

                                if(isConnected)
                                {
                                    SendMessage("GOING_FORWARD");
                                    servMessLabel.Text = "GOING_FORWARD";
                                }
                                pathViabilityLabelValue.Text = "F-T";
                            }
                            else
                            {
                                pathViabilityLabelValue.Text = "F-F";
                                if (isConnected)
                                {
                                    SendMessage("FORWARD_NOT_POSSIBLE");
                                    servMessLabel.Text = "FORWARD_NOT_POSSIBLE";
                                }

                            }

                        }
                    }
                }
            }

            // Running the path on the virtual controller:
            tRob1 = controller2.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData goForward_RAPID = tRob1.GetRapidData("PointTraversal", "goForward"); // Boolean from RAPID
                RapidData isTargetViableF_RAPID = tRob1.GetRapidData("PointTraversal", "isTargetViableF");

                if (goForward_RAPID.Value is Bool && isTargetViableF_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller2.Rapid))
                    {
                        UserAuthorizationSystem uas = controller2.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            Bool isTargetViableF_ABB = (Bool)isTargetViableF_RAPID.Value;
                            bool isTargetViableF = isTargetViableF_ABB.Value;
                            if (isTargetViableF)
                            {
                                goForwardBool.Value = true;
                                goForward_RAPID.Value = goForwardBool;

                            }
                        }
                    }
                }


            }
        }


        private void goBackward()
        {

            // Running the path on the real controller:
            ABB.Robotics.Controllers.RapidDomain.Task tRob1 = controller1.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData goBackward_RAPID = tRob1.GetRapidData("PointTraversal", "goBackward"); // Boolean from RAPID
                RapidData isTargetViableB_RAPID = tRob1.GetRapidData("PointTraversal", "isTargetViableB");

                if (goBackward_RAPID.Value is Bool && isTargetViableB_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller1.Rapid))
                    {
                        UserAuthorizationSystem uas = controller1.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            Bool isTargetViableB_ABB = (Bool)isTargetViableB_RAPID.Value;
                            bool isTargetViableB = isTargetViableB_ABB.Value;
                            if (isTargetViableB)
                            {
                                goBackwardBool.Value = true;
                                goBackward_RAPID.Value = goBackwardBool;
                                ptpIndex--; // Decrementing the PTP index
                                ptpIndexLabel.Text = ptpIndex.ToString();

                                if (isConnected)
                                {
                                    SendMessage("GOING_BACKWARD");
                                    servMessLabel.Text = "GOING_BACKWARD";
                                }

                                pathViabilityLabelValue.Text = "B-T";

                            }
                            else
                            {

                                pathViabilityLabelValue.Text = "B-F";
                                if (isConnected)
                                {
                                    SendMessage("BACKWARD_NOT_POSSIBLE");
                                    servMessLabel.Text = "BACKWARD_NOT_POSSIBLE";
                                }

                            }

                        }
                    }
                }
            }

            // Running the path on the virtual controller:
            tRob1 = controller2.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData goBackward_RAPID = tRob1.GetRapidData("PointTraversal", "goBackward"); // Boolean from RAPID
                RapidData isTargetViableB_RAPID = tRob1.GetRapidData("PointTraversal", "isTargetViableB");

                if (goBackward_RAPID.Value is Bool && isTargetViableB_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller2.Rapid))
                    {
                        UserAuthorizationSystem uas = controller2.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            Bool isTargetViableB_ABB = (Bool)isTargetViableB_RAPID.Value;
                            bool isTargetViableB = isTargetViableB_ABB.Value;
                            if (isTargetViableB)
                            {
                                goBackwardBool.Value = true;
                                goBackward_RAPID.Value = goBackwardBool;
                            }
                            else
                            {

                            }


                        }
                    }
                }
            }
        }

        private void changeTCPOrient()
        {

            // Running the path on the real controller:
            ABB.Robotics.Controllers.RapidDomain.Task tRob1 = controller1.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData changeTCPOrient_RAPID = tRob1.GetRapidData("PointTraversal", "changeTCPOrient"); // Boolean from RAPID
                RapidData isRotationViable_RAPID = tRob1.GetRapidData("PointTraversal", "isRotationViable");

                if (changeTCPOrient_RAPID.Value is Bool && isRotationViable_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller1.Rapid))
                    {
                        UserAuthorizationSystem uas = controller1.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            Bool isRotationViable_ABB = (Bool)isRotationViable_RAPID.Value;
                            bool isRotationViable = isRotationViable_ABB.Value;
                            if (isRotationViable)
                            {
                                changeTCPOrientBool.Value = true;
                                changeTCPOrient_RAPID.Value = changeTCPOrientBool;

                                if (isConnected)
                                {
                                    SendMessage("ROTATING_TCP");
                                    servMessLabel.Text = "ROTATING_TCP";
                                }

                                pathViabilityLabelValue.Text = "R-T";

                            }
                            else
                            {

                                pathViabilityLabelValue.Text = "R-F";
                                if (isConnected)
                                {
                                    SendMessage("ROTATION_NOT_POSSIBLE");
                                    servMessLabel.Text = "ROTATION_NOT_POSSIBLE";
                                }

                            }

                        }
                    }
                }
            }

            // Running the path on the virtual controller:
            tRob1 = controller2.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData changeTCPOrient_RAPID = tRob1.GetRapidData("PointTraversal", "changeTCPOrient"); // Boolean from RAPID
                RapidData isRotationViable_RAPID = tRob1.GetRapidData("PointTraversal", "isRotationViable");

                if (changeTCPOrient_RAPID.Value is Bool && isRotationViable_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller2.Rapid))
                    {
                        UserAuthorizationSystem uas = controller2.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            Bool isRotationViable_ABB = (Bool)isRotationViable_RAPID.Value;
                            bool isRotationViable = isRotationViable_ABB.Value;
                            if (isRotationViable)
                            {
                                changeTCPOrientBool.Value = true;
                                changeTCPOrient_RAPID.Value = changeTCPOrientBool;
                            }


                        }
                    }
                }
            }
        }

        /*
        private void CheckPathViability()
        {
            // Updates the data:
            SendDataParamsRAPID(0);
            SendDataParamsRAPID(1);


            // Running the path on the virtual controller:
            ABB.Robotics.Controllers.RapidDomain.Task tRob1 = controller2.Rapid.GetTask("T_ROB1");

            if (tRob1 != null)
            {
                RapidData completeTraversal_RAPID = tRob1.GetRapidData("PointTraversal", "completeTraversal"); // Boolean from RAPID
                RapidData pathViable_RAPID = tRob1.GetRapidData("PointTraversal", "isPathViable");

                if (completeTraversal_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(controller2.Rapid))
                    {
                        UserAuthorizationSystem uas = controller2.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            completePathTraversalBool.Value = true;
                            completeTraversal_RAPID.Value = completePathTraversalBool;
                            //this.start_flag_RAPID_virt.ValueChanged += new EventHandler<DataValueChangedEventArgs>(HandleStartFlagChanged);
                            //this.start_flag_RAPID_virt.ValueChanged += HandleStartFlagChanged;
                            
                            // Waiting 7.5 seconds before checking the path:
                            Thread.Sleep(7500);

                            // Reads the path viability variable:
                            pathViable = (Bool)pathViable_RAPID.Value;
                            pathViabilityLabelValue.Text = pathViable.Value.ToString();
                            isPathViable = pathViable.Value;

                        }
                    }
                }

            }
        }

                
        private void executeValidatedCPTPath()
        {
            // Running the path on the real controller:
            ABB.Robotics.Controllers.RapidDomain.Task tRob1 = controller1.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData completeTraversal_RAPID = tRob1.GetRapidData("PointTraversal", "completeTraversal"); // Boolean from RAPID

                if(completeTraversal_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller1.Rapid))
                    {
                        UserAuthorizationSystem uas = controller1.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            completePathTraversalBool.Value = true;
                            completeTraversal_RAPID.Value = completePathTraversalBool;

                            ptpIndex = total_count + 1;
                            ptpIndexLabel.Text = ptpIndex.ToString();
                        }
                    }
                }
            }

            // Running the path on the virtual controller:
            tRob1 = controller2.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData completeTraversal_RAPID = tRob1.GetRapidData("PointTraversal", "completeTraversal"); // Boolean from RAPID
                RapidData isRunningOnReal_RAPID = tRob1.GetRapidData("PointTraversal", "isRunningOnReal"); // isRunningOnReal boolean from RAPID

                if (completeTraversal_RAPID.Value is Bool && isRunningOnReal_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller2.Rapid))
                    {
                        UserAuthorizationSystem uas = controller2.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            isRunningOnRealFlag.Value = true;
                            isRunningOnReal_RAPID.Value = isRunningOnRealFlag;

                            completePathTraversalBool.Value = true;
                            completeTraversal_RAPID.Value = completePathTraversalBool;
                        }
                    }
                }
            }

        }
        */

        private void completelyTraverseForward()
        {
            SendDataParamsRAPID(0);
            SendDataParamsRAPID(1);

            // Running the path on the real controller:
            ABB.Robotics.Controllers.RapidDomain.Task tRob1 = controller1.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData completeTraversal_RAPID = tRob1.GetRapidData("PointTraversal", "completeTraversal"); // Boolean from RAPID

                if (completeTraversal_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller1.Rapid))
                    {
                        UserAuthorizationSystem uas = controller1.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            completePathTraversalBool.Value = true;
                            completeTraversal_RAPID.Value = completePathTraversalBool;
                        }
                    }
                }
            }

            // Running the path on the virtual controller:
            tRob1 = controller2.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData completeTraversal_RAPID = tRob1.GetRapidData("PointTraversal", "completeTraversal"); // Boolean from RAPID

                if (completeTraversal_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller2.Rapid))
                    {
                        UserAuthorizationSystem uas = controller2.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            completeTraversal_RAPID.Value = completePathTraversalBool;
                        }
                    }
                }
            }

        }

        private void completelyTraverseBack()
        {
            SendDataParamsRAPID(0);
            SendDataParamsRAPID(1);

            // Running the path on the real controller:
            ABB.Robotics.Controllers.RapidDomain.Task tRob1 = controller1.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData completeTraversalBack_RAPID = tRob1.GetRapidData("PointTraversal", "completeTraversalBack"); // Boolean from RAPID

                if (completeTraversalBack_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller1.Rapid))
                    {
                        UserAuthorizationSystem uas = controller1.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            completePathTraversalBackBool.Value = true;
                            completeTraversalBack_RAPID.Value = completePathTraversalBackBool;
                            ptpIndex = 1;
                            ptpIndexLabel.Text = ptpIndex.ToString();
                        }
                    }
                }
            }

            // Running the path on the virtual controller:
            tRob1 = controller2.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData completeTraversalBack_RAPID = tRob1.GetRapidData("PointTraversal", "completeTraversalBack"); // Boolean from RAPID

                if (completeTraversalBack_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller2.Rapid))
                    {
                        UserAuthorizationSystem uas = controller2.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            completeTraversalBack_RAPID.Value = completePathTraversalBackBool;
                        }
                    }
                }
            }


        }

        // Miscellaneous RAPID Methods:
        private void checkPTPIndexRAPID()
        {
            // Real controller:
            ABB.Robotics.Controllers.RapidDomain.Task tRob1 = controller1.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData ptpIndex_RAPID = tRob1.GetRapidData("PointTraversal", "ptpIndex"); // Boolean from RAPID

                if (ptpIndex_RAPID.Value is Num)
                {
                    using (Mastership m = Mastership.Request(this.controller1.Rapid))
                    {
                        UserAuthorizationSystem uas = controller1.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            ptpIndexABB = (Num) ptpIndex_RAPID.Value;
                            ptpIndex = (int) ptpIndexABB.Value;
                            ptpIndexLabel.Text = ptpIndex.ToString();

                        }
                    }
                }
            }

        }


        private void clearDataRAPID()
        {
            // Real controller:
            ABB.Robotics.Controllers.RapidDomain.Task tRob1 = controller1.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData isCleared_RAPID = tRob1.GetRapidData("PointTraversal", "isCleared"); // Boolean from RAPID

                if (isCleared_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller1.Rapid))
                    {
                        UserAuthorizationSystem uas = controller1.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            isClearedBool.Value = true;
                            isCleared_RAPID.Value = isClearedBool;
                        }
                    }
                }
            }

            // Virtual controller:
            tRob1 = controller2.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData isCleared_RAPID = tRob1.GetRapidData("PointTraversal", "isCleared"); // Boolean from RAPID

                if (isCleared_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller2.Rapid))
                    {
                        UserAuthorizationSystem uas = controller2.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            isCleared_RAPID.Value = isClearedBool;
                        }
                    }
                }
            }
        }

        private void setHomePositionRAPID()
        {
            // Real controller:
            ABB.Robotics.Controllers.RapidDomain.Task tRob1 = controller1.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData isHomePosReset_RAPID = tRob1.GetRapidData("PointTraversal", "isHomePosReset"); // Boolean from RAPID

                if (isHomePosReset_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller1.Rapid))
                    {
                        UserAuthorizationSystem uas = controller1.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            isHomePosReset.Value = true;
                            isHomePosReset_RAPID.Value = isHomePosReset;
                        }
                    }
                }
            }

            // Virtual controller:
            tRob1 = controller2.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData isHomePosReset_RAPID = tRob1.GetRapidData("PointTraversal", "isHomePosReset"); // Boolean from RAPID

                if (isHomePosReset_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller2.Rapid))
                    {
                        UserAuthorizationSystem uas = controller2.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            isHomePosReset_RAPID.Value = isHomePosReset;
                        }
                    }
                }
            }

        }

        private void setInitDigitalTwinPos()
        {
            // Method to ensure that the initial position of the digital twin is set to the same initial position as the real GoFa robot

            // Real controller:
            ABB.Robotics.Controllers.RapidDomain.Task tRob1 = controller1.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData initialPosPTP_RAPID = tRob1.GetRapidData("PointTraversal", "initialPosPTP"); // Robtarget from RAPID

                if (initialPosPTP_RAPID.Value is RobTarget)
                {
                    using (Mastership m = Mastership.Request(this.controller1.Rapid))
                    {
                        UserAuthorizationSystem uas = controller1.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            initRobotPos = (RobTarget) initialPosPTP_RAPID.Value;
                        }
                    }
                }
            }
            
            // Virtual controller:
            tRob1 = controller2.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData initialPosPTP_RAPID = tRob1.GetRapidData("PointTraversal", "initialPosPTP"); // Robtarget from RAPID
                RapidData isFirst_RAPID = tRob1.GetRapidData("PointTraversal", "isFirst"); // Boolean from RAPID

                if (initialPosPTP_RAPID.Value is RobTarget && isFirst_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller2.Rapid))
                    {
                        UserAuthorizationSystem uas = controller2.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            // Setting initial position of digital twin:
                            initialPosPTP_RAPID.Value = initRobotPos;

                            // Initiating process of traversing digital twin to initial position:
                            isFirstBool.Value = true;
                            isFirst_RAPID.Value = isFirstBool;
                        }
                    }
                }
            }

        }



        // Unity-Specific Methods:
        private void receiveDataParams(string dataMessage)
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
                positionListView.Items.Clear();
                rotationListView.Items.Clear();

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
                    positionListView.Items.Add(coorText);


                    // Adding rotation values:
                    curRot = splitCoorData[1].Trim(dataDelimiters);
                    curRotArray = curRot.Split(',');

                    ListViewItem rotText = new ListViewItem(curRotArray[0]);
                    rotText.SubItems.Add(curRotArray[1]);
                    rotText.SubItems.Add(curRotArray[2]);
                    rotText.SubItems.Add(curRotArray[3]);
                    rotationListView.Items.Add(rotText);

                }

                // Interpreting parameter settings:
                paramSettings = splitDataParams[1];
                splitParams = paramSettings.Split('%');

                // Receiving parameter information:
                speedSetting = splitParams[0];
                zoneSetting = splitParams[1];
                Int32.TryParse(splitParams[2], out total_num);
                Int32.TryParse(splitParams[3], out ptpIndex);
                
                // Updating UI:
                speedBox.Text = speedSetting;
                zoneBox.Text = zoneSetting;
                pointCounterLabelValue.Text = splitParams[2];
                ptpIndexLabel.Text = splitParams[3];

                SendMessage("Received data!");

            }
        }

        // Windows Form App Methods:
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(isConnected)
            {
                SendMessage("DISCONNECTING");
            }

            if (this.controller1 != null)
            {
                DisconnectFromController(0);
            }

            if(this.controller2 != null)
            {
                DisconnectFromController(1);
            }
        }

        private void findControllersFirst()
        {
            this.scanner = new NetworkScanner();
            this.scanner.Scan(); // Scans the network for ABB controllers
            controllers = scanner.Controllers; // Collects all available controllers on the network
            ListViewItem item = null;

            foreach(ControllerInfo controllerInfo in controllers)
            {
                item = new ListViewItem(controllerInfo.IPAddress.ToString());
                item.SubItems.Add(controllerInfo.Id.ToString());
                item.SubItems.Add(controllerInfo.Availability.ToString());
                item.SubItems.Add(controllerInfo.IsVirtual.ToString());
                item.SubItems.Add(controllerInfo.SystemName);
                item.SubItems.Add(controllerInfo.Version.ToString());
                item.SubItems.Add(controllerInfo.ControllerName);
                this.listView1.Items.Add(item);
                item.Tag = controllerInfo;


                this.networkwatcher = new NetworkWatcher(scanner.Controllers);
                this.networkwatcher.Found += new
                EventHandler<NetworkWatcherEventArgs>(HandleFoundEvent);
                this.networkwatcher.Lost += new
                EventHandler<NetworkWatcherEventArgs>(HandleLostEvent);
                this.networkwatcher.EnableRaisingEvents = true;
            }


        }

        private void savePointsButton_Click(object sender, EventArgs e)
        {
            if (xBoxPos.Text != null && yBoxPos.Text != null && zBoxPos.Text != null)
            {
                decimal d1;
                decimal d2;
                decimal d3;

                bool canConvert1 = decimal.TryParse(xBoxPos.Text, out d1);
                bool canConvert2 = decimal.TryParse(yBoxPos.Text, out d2);
                bool canConvert3 = decimal.TryParse(zBoxPos.Text, out d3);

                if (canConvert1 && canConvert2 && canConvert3)
                {
                    ListViewItem item = new ListViewItem(xBoxPos.Text);
                    item.SubItems.Add(yBoxPos.Text);
                    item.SubItems.Add(zBoxPos.Text);
                    this.positionListView.Items.Add(item);
                    //item.Tag = 

                    xBoxPos.Text = null;
                    yBoxPos.Text = null;
                    zBoxPos.Text = null;
                }
                else
                {
                    MessageBox.Show("ERROR: You must enter in valid numbers.");
                }
            }
            else
            {
                MessageBox.Show("ERROR: You must enter in three coordinates.");
            }
        }


        private void positionListView_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem item = this.positionListView.SelectedItems[0];
            if (item != null)
            {
                string x_coor = item.SubItems[0].Text;
                string y_coor = item.SubItems[1].Text;
                string z_coor = item.SubItems[2].Text;

                // Opening new form:
                DataEditor dataEditor = new DataEditor(this);
                dataEditor.xTextBox.Text = x_coor;
                dataEditor.yTextBox.Text = y_coor;
                dataEditor.zTextBox.Text = z_coor;
                dataEditor.ShowDialog();
            }
        }

        
        void HandleFoundEvent(object sender, NetworkWatcherEventArgs e)
        {
            this.Invoke(new EventHandler<NetworkWatcherEventArgs>(AddControllerToListView), new Object[] { this, e });
        }

        void HandleLostEvent(object sender, NetworkWatcherEventArgs e)
        {
            this.Invoke(new EventHandler<NetworkWatcherEventArgs>(RemoveControllerFromListView), new Object[] { this, e });
        }
        
        private void connectToController(object sender, EventArgs e)
        {
            int num;

            try
            {
                ListViewItem item = this.listView1.SelectedItems[0];
                if (item == listView1.Items[0])
                {
                    num = 0;
                }
                else
                {
                    num = 1;
                }

                connectToController(num);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select a controller first.");
            }

        }

        private void AddControllerToListView(object sender, NetworkWatcherEventArgs e)
        {
            ControllerInfo controllerInfo = e.Controller;
            ListViewItem item = new ListViewItem(controllerInfo.IPAddress.ToString());
            item.SubItems.Add(controllerInfo.Id);
            item.SubItems.Add(controllerInfo.Availability.ToString());
            item.SubItems.Add(controllerInfo.IsVirtual.ToString());
            item.SubItems.Add(controllerInfo.SystemName);
            item.SubItems.Add(controllerInfo.Version.ToString());
            item.SubItems.Add(controllerInfo.ControllerName);
            this.listView1.Items.Add(item);
            item.Tag = controllerInfo;
        }

        private void RemoveControllerFromListView(object sender, NetworkWatcherEventArgs e)
        {
            ControllerInfo controllerInfo = e.Controller;

            foreach(ListViewItem item in this.listView1.Items)
            {
                if(controllerInfo == (ControllerInfo) item.Tag)
                {
                    this.listView1.Items.Remove(item);
                }
            }
        }

        private void disconnectFromController(object sender, EventArgs e)
        {
            int num;

            try
            {
                ListViewItem item = this.listView1.SelectedItems[0];
                if (!isTesting)
                {
                    ControllerInfo contrInfo = item.Tag as ControllerInfo;
                    if (!contrInfo.IsVirtual)
                    {
                        num = 0;
                    }
                    else
                    {
                        num = 1;
                    }
                }
                else
                {
                    if (item == listView1.Items[0])
                    {
                        num = 0;
                    }
                    else
                    {
                        num = 1;
                    }
                }

                DisconnectFromController(num);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select a controller first.");
            }

        }

        private void StartRapidProgram(object sender, EventArgs e)
        {
            int num;

            try
            {
                ListViewItem item = this.listView1.SelectedItems[0];
                
                if(!isTesting)
                {
                    ControllerInfo contrInfo = item.Tag as ControllerInfo;
                    if(!contrInfo.IsVirtual)
                    {
                        num = 0;
                    }
                    else
                    {
                        num = 1;
                    }
                }
                else
                {
                    if (item == listView1.Items[0])
                    {
                        num = 0;
                    }
                    else
                    {
                        num = 1;
                    }
                }
                

                StartRapidProgram(num);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select a controller first.");
            }

        }

        private void StopRapidProgram(object sender, EventArgs e)
        {
            int num;

            try
            {
                ListViewItem item = this.listView1.SelectedItems[0];
                if (!isTesting)
                {
                    ControllerInfo contrInfo = item.Tag as ControllerInfo;
                    if (!contrInfo.IsVirtual)
                    {
                        num = 0;
                    }
                    else
                    {
                        num = 1;
                    }
                }
                else
                {
                    if (item == listView1.Items[0])
                    {
                        num = 0;
                    }
                    else
                    {
                        num = 1;
                    }
                }

                StopRapidProgram(num);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select a controller first.");
            }

        }

        // Path-modifying button methods:
        private void goForwardButton_Click(object sender, EventArgs e)
        {
            SendDataParamsRAPID(0);
            SendDataParamsRAPID(1);
            goForward();
        }

        private void goBackwardButton_Click(object sender, EventArgs e)
        {
            SendDataParamsRAPID(0);
            SendDataParamsRAPID(1);
            goBackward();
        }

        private void CPTButton_Click(object sender, EventArgs e)
        {
            completelyTraverseForward();
            /*
            isPathViable = false;
            CheckPathViability();

            if (isPathViable)
            {
                executeValidatedCPTPath();
                isPathViable = false; // Resets path viability
            }
            */
        }

        private void CPTBButton_Click(object sender, EventArgs e)
        {
            completelyTraverseBack();
        }

        private void clearDataButton_Click(object sender, EventArgs e)
        {
            clearDataRAPID();

            // Resetting UI:
            positionListView.Items.Clear();
            rotationListView.Items.Clear();
            speedBox.Text = "";
            zoneBox.Text = "";
            ptpIndex = 1;
            ptpIndexLabel.Text = "1";
            pathViabilityLabelValue.Text = "NOT CHECKED";
        }

        private void sendDataButton_Click(object sender, EventArgs e)
        {
            SendDataParamsRAPID(0);
            SendDataParamsRAPID(1);
        }
    }


}


