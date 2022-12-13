using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace PCSDKApplication
{
    public partial class TCPServerWindow : Form
    {

        private const int portNum = 13;

        private TcpListener tcpListener;
        private Thread tcpListenerThread;
        private TcpClient connectedTcpClient;

        MainWindow mainWindow;

        public string[] curCoor;
        public string cur_x;
        public string cur_y;
        public string cur_z;

        public TCPServerWindow(MainWindow mainWin)
        {
            mainWindow = mainWin;
            InitializeComponent();
            establishServer();
        }

        public void establishServer()
        {
            tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
            tcpListenerThread.IsBackground = true;
            tcpListenerThread.Start();

            Progress1.Text = "Connecting...";
        }

        private void ListenForIncomingRequests()
        {
            try
            {
                Int32 port = 8052;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                tcpListener = new TcpListener(localAddr, portNum);
                tcpListener.Start();

                Console.WriteLine("Server is listening");
                Byte[] bytes = new byte[1024];

                while (true)
                {
                    using (connectedTcpClient = tcpListener.AcceptTcpClient())
                    {
                        if(connectedTcpClient.Connected)
                        {
                            this.Invoke(new MethodInvoker(delegate(){Progress1.Text = "Connected to client.";}));
                        }

                        using (NetworkStream stream = connectedTcpClient.GetStream())
                        {
                            int length;

                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                var incomingData = new byte[length];
                                Array.Copy(bytes, 0, incomingData, 0, length);

                                string clientMessage = Encoding.ASCII.GetString(incomingData);
                                this.Invoke(new MethodInvoker(delegate () { clientMessages.Text = clientMessage; }));
                                //MessageBox.Show("Client message received as: " + clientMessage);

                            }
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                MessageBox.Show("SocketException " + socketException.ToString());
            }
        }

        private void SendMessage()
        {
            if (connectedTcpClient == null)
            {
                MessageBox.Show("No client is connected!");
                return;
            }

            try
            {
                NetworkStream stream = connectedTcpClient.GetStream();
                if (stream.CanWrite)
                {
                    //string serverMessage = "This is a message from your server.";
                    string serverMessage = TCPMessageBox.Text;
                    // Convert string message to byte array
                    byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                    // Write byte array to socketConnection stream.
                    stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                    //MessageBox.Show("Server sent his message - should be received by client");
                }
            }
            catch (SocketException socketException)
            {
                MessageBox.Show("Socket exception: " + socketException);
            }
        }

        private void sendMsgBtn_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void SendData_Click(object sender, EventArgs e)
        {
            string dataMsg = clientMessages.Text;
            string curDataPoint;
            char[] delimiters = {'[', ']'};

            if (!dataMsg.Equals("0"))
            {

                string[] dataPointList = dataMsg.Split(';');

                for(int i = 0; i < (dataPointList.Length - 1); i++)
                {
                    // Extracts each data point from the list:
                    curDataPoint = dataPointList[i].Trim(delimiters);
                    curCoor = curDataPoint.Split(',');

                    ListViewItem coorText = new ListViewItem(curCoor[0]);
                    coorText.SubItems.Add(curCoor[1]);
                    coorText.SubItems.Add(curCoor[2]);
                    mainWindow.positionListView.Items.Add(coorText);

                }

            }
            
            //this.Invoke(new MethodInvoker(delegate () { mainWin     } ));


            this.Dispose();
        }
    }
}
