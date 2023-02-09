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
        public TCPServer tcpServer; // Instance of the class representing the TCP server
        public ABBInterface abbInterface; // Instance of the class representing the interface between the Windows Form App and ABB controllers

        public NetworkScanner scanner = null; // Network scanner variable
        public NetworkWatcher networkwatcher = null; // Network watcher variable

        // Boolean indicating whether the physical ABB robot or the digital version of the real controller are being used:
        // Select FALSE if testing is performed on the REAL robot
        // Select TRUE if testing is performed using the digital version of the real controller
        public bool isTesting = true;


        public MainWindow()
        {
            InitializeComponent();
         
            // Initializing TCP Server and ABB Interface:
            abbInterface = new ABBInterface(this);
            tcpServer = new TCPServer(this);

            findControllersFirst(); // Scans the network for controllers

        }

        public void tcpButton_Click(object sender, EventArgs e)
        {
            tcpServer.establishServer(); // Establishes the TCP Server
        }


        public void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Method to indicate to client that the TCP server is being closed, and disconnecting from all ABB controllers accordingly

            if(tcpServer.isConnected)
            {
                tcpServer.SendMessage("DISCONNECTING");
            }

            if (abbInterface.controller1 != null)
            {
                abbInterface.DisconnectFromController(0);
            }

            if(abbInterface.controller2 != null)
            {
                abbInterface.DisconnectFromController(1);
            }
        }

        public void findControllersFirst()
        {
            // Method to scan the network for ABB controllers

            scanner = new NetworkScanner();
            scanner.Scan(); // Scans the network for ABB controllers
            abbInterface.controllers = scanner.Controllers; // Collects all available controllers on the network
            ListViewItem item = null;

            // Adds each controller onto the controller listview in the form
            foreach(ControllerInfo controllerInfo in abbInterface.controllers)
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

                // Adding each controller to the event handler:
                this.networkwatcher = new NetworkWatcher(scanner.Controllers);
                this.networkwatcher.Found += new
                EventHandler<NetworkWatcherEventArgs>(HandleFoundEvent);
                this.networkwatcher.Lost += new
                EventHandler<NetworkWatcherEventArgs>(HandleLostEvent);
                this.networkwatcher.EnableRaisingEvents = true;
            }


        }

        public void savePointsButton_Click(object sender, EventArgs e)
        {
            // Method assigned to the button to save any point data that are entered through the form

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


        public void positionListView_DoubleClick(object sender, EventArgs e)
        {
            // Method to allow for position point data to be edited by simply clicking on its corresponding position in the point listview 

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
        
        public void connectToController(object sender, EventArgs e)
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

                abbInterface.connectToController(num);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select a controller first.");
            }

        }

        public void AddControllerToListView(object sender, NetworkWatcherEventArgs e)
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

        public void RemoveControllerFromListView(object sender, NetworkWatcherEventArgs e)
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

        public void disconnectFromController(object sender, EventArgs e)
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

                abbInterface.DisconnectFromController(num);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select a controller first.");
            }

        }

        public void StartRapidProgram(object sender, EventArgs e)
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
                

                abbInterface.StartRapidProgram(num);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select a controller first.");
            }

        }

        public void StopRapidProgram(object sender, EventArgs e)
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

                abbInterface.StopRapidProgram(num);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please select a controller first.");
            }

        }

        // Path-modifying button methods:
        public void goForwardButton_Click(object sender, EventArgs e)
        {
            abbInterface.SendDataParamsRAPID(0);
            abbInterface.SendDataParamsRAPID(1);
            abbInterface.goForward();
        }

        public void goBackwardButton_Click(object sender, EventArgs e)
        {
            abbInterface.SendDataParamsRAPID(0);
            abbInterface.SendDataParamsRAPID(1);
            abbInterface.goBackward();
        }

        public void CPTButton_Click(object sender, EventArgs e)
        {
            abbInterface.completelyTraverseForward();
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

        public void CPTBButton_Click(object sender, EventArgs e)
        {
            abbInterface.completelyTraverseBack();
        }

        public void clearDataButton_Click(object sender, EventArgs e)
        {
            abbInterface.clearDataRAPID();

            // Resetting UI:
            positionListView.Items.Clear();
            rotationListView.Items.Clear();
            speedBox.Text = "";
            zoneBox.Text = "";
            abbInterface.ptpIndex = 1;
            ptpIndexLabel.Text = "1";
            pathViabilityLabelValue.Text = "NOT CHECKED";
        }

        public void sendDataButton_Click(object sender, EventArgs e)
        {
            abbInterface.SendDataParamsRAPID(0);
            abbInterface.SendDataParamsRAPID(1);
        }
    }


}


