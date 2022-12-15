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
        ///// ABB RAPID Information:
        public Bool completePathTraversalBool;
        public Bool completePathTraversalBackBool;
        public Bool goForwardBool;
        public Bool goBackwardBool;
        public Bool changeTCPOrientBool;
        public Bool isFirstBool;
        public Bool isClearedBool;
        public Bool isHomePosReset;
        public Num num_targets; // ABB num variable used to indicate the number of inputted coordinates
        public Num ptpIndexABB; // ABB num variable used to indicate the index of the next point to be traversed when point-to-point mode is enabled
        public RobTarget initRobotPos;
        public ABB.Robotics.Controllers.RapidDomain.String speed_RAPID; // ABB string used to indicate the value of the speed parameter
        public ABB.Robotics.Controllers.RapidDomain.String zone_RAPID; // ABB string used to indicate the value of the zone parameter

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
                    if (isTesting)
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
                        if (connectedToController1 && connectedToController2)
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
                        if (num == 0)
                        {
                            rapidProgramStatus1.Text = "Not running";
                            rapidProgramStatus1.ForeColor = Color.IndianRed;
                            startedRAPIDProgram1 = false;
                        }
                        else if (num == 1)
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


            if (isConnected)
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
                RapidData speedVal_RAPID = tRob1.GetRapidData("PointTraversal", "speedStr");
                RapidData zoneVal_RAPID = tRob1.GetRapidData("PointTraversal", "zoneStr");
                RapidData ptpIndex_RAPID = tRob1.GetRapidData("PointTraversal", "ptpIndex");
                RapidData targetsNum_RAPID = tRob1.GetRapidData("PointTraversal", "targetsNum");
                RapidData tgPos_RAPID = tRob1.GetRapidData("PointTraversal", "tgPos");
                RapidData tgRot_RAPID = tRob1.GetRapidData("PointTraversal", "tgRot");

                ///*** Setting Parameter + Setting Values: ***///
                speed_RAPID.Value = this.speedBox.Text; // RAPID String object
                zone_RAPID.Value = this.zoneBox.Text; // RAPID String object

                if ((speedVal_RAPID.Value is ABB.Robotics.Controllers.RapidDomain.String) && (zoneVal_RAPID.Value is ABB.Robotics.Controllers.RapidDomain.String) && (ptpIndex_RAPID.Value is Num) && (targetsNum_RAPID.Value is Num))
                {
                    using (Mastership m = Mastership.Request(controller.Rapid))
                    {
                        UserAuthorizationSystem uas = controller.AuthenticationSystem;

                        if (uas.CheckDemandGrant(Grant.ModifyRapidDataValue) && uas.CheckDemandGrant(Grant.ModifyRapidPosition))
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
                            Bool isTargetViableF_ABB = (Bool)isTargetViableF_RAPID.Value;
                            bool isTargetViableF = isTargetViableF_ABB.Value;
                            if (isTargetViableF)
                            {
                                goForwardBool.Value = true;
                                goForward_RAPID.Value = goForwardBool;
                                ptpIndex++; // Incrementing the PTP index
                                ptpIndexLabel.Text = ptpIndex.ToString();

                                if (isConnected)
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
                            ptpIndexABB = (Num)ptpIndex_RAPID.Value;
                            ptpIndex = (int)ptpIndexABB.Value;
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

