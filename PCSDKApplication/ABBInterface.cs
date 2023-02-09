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
    public class ABBInterface
    {
        MainWindow mainWindow;

        // ABB Controller Information:
        public ControllerInfoCollection controllers = null;
        public Controller controller1 = null;
        public Controller controller2 = null;

        public ABB.Robotics.Controllers.RapidDomain.Task[] tasks = null;

        ///// ABB RAPID Information:
        public Bool completePathTraversalBool;
        public Bool completePathTraversalBackBool;
        public Bool goForwardBool;
        public Bool goBackwardBool;
        public Bool changeTCPBool;
        public Bool isFirstBool;
        public Bool isClearedBool;
        public Bool isHomePosReset;
        public Num speedNum; // ABB num variable used to indicate the index of the desired speed value in the RAPID speeddata array
        public Num zoneNum; // ABB num variable used to indicate the index of the desired zone value in the RAPID zonedata array
        public Num num_targets; // ABB num variable used to indicate the number of inputted coordinates
        public Num ptpIndexABB; // ABB num variable used to indicate the index of the next point to be traversed when point-to-point mode is enabled
        public RobTarget initRobotPos;

        // TCP Message Booleans:
        public bool connectedToController1 = false;
        public bool connectedToController2 = false;
        public bool startedRAPIDProgram1 = false;
        public bool startedRAPIDProgram2 = false;
        public bool isPathViable = false;

        // Error message strings:
        public string contrErrMess = "";
        public string rapidErrMess = "";

        public int speedInt; // Integer representing index of the speed value in the speed value array
        public int zoneInt; // Integer representing index of the zone value in the zone value array

        public int ptpIndex = 1; // Index representing the next point to traverse to
        public int total_num; // Total number of input coordinates as obtained by the Unity client
        public int total_count; // Total number of input coordinates
        

        public ABBInterface(MainWindow mainWin)
        {
            mainWindow = mainWin;
        }

        public void StartProcess()
        {
            if (mainWindow.isTesting)
            {
                connectToController(0); // Connecting to real-life ABB GoFa controller
                connectToController(1); // Connecting to virtual ABB GoFa controller (digital twin)
            }
            else
            {
                connectToControllers(); // Connects to the real and virtual ABB GoFa controllers
            }


            if (connectedToController1 && connectedToController2)
            {
                StartRapidProgram(0); // Starts RAPID Program for real-life ABB GoFa
                Thread.Sleep(1000); // Causes a 1 second delay between the initiation of the RAPID programs on the two controllers (to allow for the home position on the digital twin to be properly set)
                StartRapidProgram(1); // Starts RAPID Program for digital twin

                if (startedRAPIDProgram1 && startedRAPIDProgram2)
                {
                    mainWindow.tcpServer.SendMessage("PROCESS_STARTED"); // Sending message to Unity client
                    mainWindow.servMessLabel.Text = "PROCESS_STARTED"; // Updating UI

                    setInitDigitalTwinPos(); // Sets the initial position of the digital twin
                }
                else
                {
                    mainWindow.tcpServer.SendMessage("ERR " + rapidErrMess); // Sending error message to Unity client
                    mainWindow.servMessLabel.Text = "Error with starting RAPID Program: " + rapidErrMess; // Updating UI
                }
            }
            else
            {
                mainWindow.tcpServer.SendMessage("ERR " + contrErrMess); // Sending error message to Unity client
                mainWindow.servMessLabel.Text = contrErrMess; // Updating UI
            }
        }

        public void connectToController(int num)
        {
            // Method utilized by the HoloLens 2 to connect to an ABB controller

            ControllerInfo selectContr = controllers[num];
            Controller controller;

            if (selectContr.Availability == ABB.Robotics.Controllers.Availability.Available)
            {

                // Attempts to connect to the controller:
                try
                {
                    if (mainWindow.isTesting)
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
                        if (!mainWindow.isTesting)
                        {
                            if (!selectContr.IsVirtual)
                            {
                                connectedToController1 = true;

                                // Updating UI:
                                mainWindow.contrConnectionStatus1.Text = "Connected";
                                mainWindow.contrConnectionStatus1.ForeColor = Color.Green;

                            }
                            else
                            {
                                connectedToController2 = true;

                                // Updating UI:
                                mainWindow.contrConnectionStatus2.Text = "Connected";
                                mainWindow.contrConnectionStatus2.ForeColor = Color.Green;
                            }
                        }
                        else
                        {
                            if (num == 0)
                            {
                                connectedToController1 = true;

                                // Updating UI:
                                mainWindow.contrConnectionStatus1.Text = "Connected";
                                mainWindow.contrConnectionStatus1.ForeColor = Color.Green;

                            }
                            else if (num == 1)
                            {
                                connectedToController2 = true;

                                // Updating UI:
                                mainWindow.contrConnectionStatus2.Text = "Connected";
                                mainWindow.contrConnectionStatus2.ForeColor = Color.Green;
                            }
                        }

                        // Updating UI:
                        if (connectedToController1 && connectedToController2)
                        {
                            mainWindow.startRapidProgButton.Enabled = true;
                            mainWindow.stopRAPIDProgButton.Enabled = true;
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
                mainWindow.tcpServer.SendMessage(denyMessage);
                mainWindow.servMessLabel.Text = denyMessage;
            }
        }


        public void connectToControllers()
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
                        // If the first controller in the listview is the real ABB GoFa robot:
                        if (!selectContr1.IsVirtual)
                        {
                            this.controller1 = Controller.Connect(selectContr1.SystemId, ConnectionType.Standalone, false);
                            this.controller1.Logon(UserInfo.DefaultUser);

                            this.controller2 = Controller.Connect(selectContr2.SystemId, ConnectionType.Standalone, false);
                            this.controller2.Logon(UserInfo.DefaultUser);
                        }
                        // If the first controller in the listview is the digital twin:
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
                            connectedToController1 = true; // Sets controller connection boolean of the real, physical robot to true

                            // Updating UI:
                            mainWindow.contrConnectionStatus1.Text = "Connected";
                            mainWindow.contrConnectionStatus1.ForeColor = Color.Green;

                        }

                        if (this.controller2.Connected == true)
                        {
                            connectedToController2 = true; // Sets controller connection boolean of the digital twin to true

                            // Updating UI:
                            mainWindow.contrConnectionStatus2.Text = "Connected";
                            mainWindow.contrConnectionStatus2.ForeColor = Color.Green;
                        }

                        if (connectedToController1 && connectedToController2)
                        {
                            // Updating UI:
                            mainWindow.startRapidProgButton.Enabled = true;
                            mainWindow.stopRAPIDProgButton.Enabled = true;
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
                    mainWindow.tcpServer.SendMessage(denyMessage);
                    mainWindow.servMessLabel.Text = denyMessage;
                }
            }
            catch
            {
                string denyMessage = "The two controllers are not present on the network!";
                mainWindow.tcpServer.SendMessage(denyMessage);
                mainWindow.servMessLabel.Text = denyMessage;
            }

        }

        public void DisconnectFromController(int num)
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
                    mainWindow.contrConnectionStatus1.Text = "Not connected";
                    mainWindow.contrConnectionStatus1.ForeColor = Color.IndianRed;
                }
                else if (num == 1)
                {
                    this.controller2 = null;
                    connectedToController2 = false;

                    // Updating Form UI:
                    mainWindow.contrConnectionStatus2.Text = "Not connected";
                    mainWindow.contrConnectionStatus2.ForeColor = Color.IndianRed;
                }

                if (this.controller1 == null && this.controller2 == null)
                {
                    // Updating Form UI:
                    mainWindow.startRapidProgButton.Enabled = false;
                    mainWindow.stopRAPIDProgButton.Enabled = false;
                }

            }
            else
            {
                //MessageBox.Show("No controller is connected!");
            }
        }

        public void StartRapidProgram(int num)
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
                                rapidErrMess = "Start Failed: " + result.ToString();
                            }
                            else
                            {
                                rapidErrMess = "";

                                // Updating UI:
                                if (num == 0)
                                {
                                    mainWindow.rapidProgramStatus1.Text = "Running!";
                                    mainWindow.rapidProgramStatus1.ForeColor = Color.Green;
                                    startedRAPIDProgram1 = true;

                                }
                                else if (num == 1)
                                {
                                    mainWindow.rapidProgramStatus2.Text = "Running!";
                                    mainWindow.rapidProgramStatus2.ForeColor = Color.Green;
                                    startedRAPIDProgram2 = true;
                                }

                                if (startedRAPIDProgram1 && startedRAPIDProgram2)
                                {
                                    mainWindow.goForwardButton.Enabled = true;
                                    mainWindow.goBackwardButton.Enabled = true;
                                    mainWindow.CPTButton.Enabled = true;
                                    mainWindow.CPTBButton.Enabled = true;
                                    mainWindow.clearDataButton.Enabled = true;
                                    mainWindow.sendDataButton.Enabled = true;
                                }
                            }
                        }
                        else
                        {
                            rapidErrMess = "You don't have the proper grants!";
                        }

                    }
                }
                else
                {
                    rapidErrMess = "Automatic mode is required to start execution from a remote client.";
                }
            }
            catch (System.InvalidOperationException ex)
            {
                rapidErrMess = "Mastership is held by another client." + ex.Message;
            }
            catch (System.Exception ex)
            {
                rapidErrMess = "Unexpected error occurred: " + ex.Message;
            }
        }

        public void StopRapidProgram(int num)
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
                        if (num == 0)
                        {
                            mainWindow.rapidProgramStatus1.Text = "Not running";
                            mainWindow.rapidProgramStatus1.ForeColor = Color.IndianRed;
                            startedRAPIDProgram1 = false;
                        }
                        else if (num == 1)
                        {
                            mainWindow.rapidProgramStatus2.Text = "Not running";
                            mainWindow.rapidProgramStatus2.ForeColor = Color.IndianRed;
                            startedRAPIDProgram2 = false;
                        }

                        mainWindow.goForwardButton.Enabled = false;
                        mainWindow.goBackwardButton.Enabled = false;
                        mainWindow.CPTButton.Enabled = false;
                        mainWindow.CPTBButton.Enabled = false;
                        mainWindow.clearDataButton.Enabled = false;
                        mainWindow.sendDataButton.Enabled = false;

                        // Resetting UI:
                        mainWindow.positionListView.Items.Clear();
                        mainWindow.rotationListView.Items.Clear();
                        mainWindow.speedBox.Text = "";
                        mainWindow.zoneBox.Text = "";
                        ptpIndex = 1;
                        mainWindow.ptpIndexLabel.Text = "1";
                        mainWindow.pathViabilityLabelValue.Text = "NOT CHECKED";
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

        public void SendDataParamsRAPID(int num)
        {
            // This method sends the coordinate data and corresponding parameters to the selected controller
            // If the selected controller is the digital twin, the path is attempted to be automatically run

            string x_coor, y_coor, z_coor; // Placeholder strings for each input coordinate
            string w_rot, x_rot, y_rot, z_rot; // Placeholder strings for each input rotation


            if (mainWindow.tcpServer.isConnected)
            {
                total_count = total_num;
            }
            else
            {
                total_count = this.mainWindow.positionListView.Items.Count;
            }

            num_targets.Value = total_count; // Total number of targets
            ptpIndexABB.Value = ptpIndex;

            Controller controller;

            if (num == 0)
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
                RapidData speedVal_RAPID = tRob1.GetRapidData("PointTraversal", "speedVal");
                RapidData zoneVal_RAPID = tRob1.GetRapidData("PointTraversal", "zoneVal");
                RapidData ptpIndex_RAPID = tRob1.GetRapidData("PointTraversal", "ptpIndex");
                RapidData targetsNum_RAPID = tRob1.GetRapidData("PointTraversal", "targetsNum");
                RapidData tgPos_RAPID = tRob1.GetRapidData("PointTraversal", "tgPos");
                RapidData tgRot_RAPID = tRob1.GetRapidData("PointTraversal", "tgRot");

                ///*** Setting Parameter + Setting Values: ***///
                speedNum.Value = speedInt; // RAPID Num Object
                zoneNum.Value = zoneInt; // RAPID Num Object

                if ((speedVal_RAPID.Value is Num) && (zoneVal_RAPID.Value is Num) && (ptpIndex_RAPID.Value is Num) && (targetsNum_RAPID.Value is Num))
                {
                    using (Mastership m = Mastership.Request(controller.Rapid))
                    {
                        UserAuthorizationSystem uas = controller.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            speedVal_RAPID.Value = speedNum;
                            zoneVal_RAPID.Value = zoneNum;
                            ptpIndex_RAPID.Value = ptpIndexABB;
                            targetsNum_RAPID.Value = num_targets;
                        }
                    }
                }

                ///*** Looping through each inputted coordinate: ***///
                for (int i = 0; i < total_count; i++)
                {
                    // Extracting coordinate data from each listview element:
                    ListViewItem item_pos = mainWindow.positionListView.Items[i];
                    x_coor = item_pos.SubItems[0].Text;
                    y_coor = item_pos.SubItems[1].Text;
                    z_coor = item_pos.SubItems[2].Text;

                    // Creating new Pos datatype:
                    Pos rt = new Pos();
                    rt.FillFromString2("[" + x_coor + "," + y_coor + "," + z_coor + "]");


                    ListViewItem item_rot = mainWindow.rotationListView.Items[i];
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
        public void goForward()
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
                            Bool isTargetViableF_ABB = (Bool)isTargetViableF_RAPID.Value;
                            bool isTargetViableF = isTargetViableF_ABB.Value;
                            if (isTargetViableF)
                            {
                                goForwardBool.Value = true;
                                goForward_RAPID.Value = goForwardBool;
                                ptpIndex++; // Incrementing the PTP index
                                mainWindow.ptpIndexLabel.Text = ptpIndex.ToString();

                                if (mainWindow.tcpServer.isConnected)
                                {
                                    mainWindow.tcpServer.SendMessage("GOING_FORWARD");
                                    mainWindow.servMessLabel.Text = "GOING_FORWARD";
                                }
                                mainWindow.pathViabilityLabelValue.Text = "F-T";
                            }
                            else
                            {
                                mainWindow.pathViabilityLabelValue.Text = "F-F";
                                if (mainWindow.tcpServer.isConnected)
                                {
                                    mainWindow.tcpServer.SendMessage("FORWARD_NOT_POSSIBLE");
                                    mainWindow.servMessLabel.Text = "FORWARD_NOT_POSSIBLE";
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


        public void goBackward()
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
                                mainWindow.ptpIndexLabel.Text = ptpIndex.ToString();

                                if (mainWindow.tcpServer.isConnected)
                                {
                                    mainWindow.tcpServer.SendMessage("GOING_BACKWARD");
                                    mainWindow.servMessLabel.Text = "GOING_BACKWARD";
                                }

                                mainWindow.pathViabilityLabelValue.Text = "B-T";

                            }
                            else
                            {

                                mainWindow.pathViabilityLabelValue.Text = "B-F";
                                if (mainWindow.tcpServer.isConnected)
                                {
                                    mainWindow.tcpServer.SendMessage("BACKWARD_NOT_POSSIBLE");
                                    mainWindow.servMessLabel.Text = "BACKWARD_NOT_POSSIBLE";
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

                        }
                    }
                }
            }
        }

        public void changeTCP()
        {

            // Running the path on the real controller:
            ABB.Robotics.Controllers.RapidDomain.Task tRob1 = controller1.Rapid.GetTask("T_ROB1");
            if (tRob1 != null)
            {
                RapidData changeTCP_RAPID = tRob1.GetRapidData("PointTraversal", "changeTCP"); // Boolean from RAPID
                RapidData isTCPChangeViable_RAPID = tRob1.GetRapidData("PointTraversal", "isTCPChangeViable");

                if (changeTCP_RAPID.Value is Bool && isTCPChangeViable_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller1.Rapid))
                    {
                        UserAuthorizationSystem uas = controller1.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            Bool isTCPChangeViable_ABB = (Bool)isTCPChangeViable_RAPID.Value;
                            bool isTCPChangeViable = isTCPChangeViable_ABB.Value;
                            if (isTCPChangeViable)
                            {
                                changeTCPBool.Value = true;
                                changeTCP_RAPID.Value = changeTCPBool;

                                if (mainWindow.tcpServer.isConnected)
                                {
                                    mainWindow.tcpServer.SendMessage("CHANGING_TCP");
                                    mainWindow.servMessLabel.Text = "CHANGING_TCP";
                                }

                                mainWindow.pathViabilityLabelValue.Text = "U-T";

                            }
                            else
                            {

                                mainWindow.pathViabilityLabelValue.Text = "U-F";
                                if (mainWindow.tcpServer.isConnected)
                                {
                                    mainWindow.tcpServer.SendMessage("TCPCHANGE_NOT_POSSIBLE");
                                    mainWindow.servMessLabel.Text = "TCPCHANGE_NOT_POSSIBLE";
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
                RapidData changeTCP_RAPID = tRob1.GetRapidData("PointTraversal", "changeTCP"); // Boolean from RAPID
                RapidData isTCPChangeViable_RAPID = tRob1.GetRapidData("PointTraversal", "isTCPChangeViable");

                if (changeTCP_RAPID.Value is Bool && isTCPChangeViable_RAPID.Value is Bool)
                {
                    using (Mastership m = Mastership.Request(this.controller2.Rapid))
                    {
                        UserAuthorizationSystem uas = controller2.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
                        {
                            Bool isTCPChangeViable_ABB = (Bool)isTCPChangeViable_RAPID.Value;
                            bool isTCPChangeViable = isTCPChangeViable_ABB.Value;
                            if (isTCPChangeViable)
                            {
                                changeTCPBool.Value = true;
                                changeTCP_RAPID.Value = changeTCPBool;
                            }

                        }
                    }
                }
            }
        }

        public void completelyTraverseForward()
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

        public void completelyTraverseBack()
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
                            mainWindow.ptpIndexLabel.Text = ptpIndex.ToString();
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
        public void checkPTPIndexRAPID()
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
                            ptpIndexABB = (Num)ptpIndex_RAPID.Value;
                            ptpIndex = (int)ptpIndexABB.Value;
                            mainWindow.ptpIndexLabel.Text = ptpIndex.ToString();

                        }
                    }
                }
            }

        }


        public void clearDataRAPID()
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

        public void setHomePositionRAPID()
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

        public void setInitDigitalTwinPos()
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
                            initRobotPos = (RobTarget)initialPosPTP_RAPID.Value;
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



    }



}

