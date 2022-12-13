using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PCSDKApplication
{
    internal class TCPServer
    {

        private const int portNum = 13;

        private TcpListener tcpListener;
        private Thread tcpListenerThread;
        private TcpClient connectedTcpClient;

        public void establishServer()
        {
            tcpListenerThread = new Thread(new ThreadStart(ListenForIncomingRequests));
            tcpListenerThread.IsBackground = true;
            tcpListenerThread.Start();
        }

        private void ListenForIncomingRequests()
        {
            try
            {
                Int32 port = 8052;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                tcpListener = new TcpListener(localAddr, port);
                tcpListener.Start();

                Console.WriteLine("Server is listening");
                Byte[] bytes = new byte[1024];

                while(true)
                {
                    using(connectedTcpClient = tcpListener.AcceptTcpClient())
                    {
                        Console.WriteLine(connectedTcpClient.Connected);

                        using(NetworkStream stream = connectedTcpClient.GetStream())
                        {
                            int length;

                            while((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                var incomingData = new byte[length];
                                Array.Copy(bytes, 0, incomingData, 0, length);

                                string clientMessage = Encoding.ASCII.GetString(incomingData);
                                Console.WriteLine("Client message received as: " + clientMessage);

                            }
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Console.WriteLine("SocketException " + socketException.ToString());
            }
        }

        private void SendMessage()
        {
            if(connectedTcpClient == null)
            {
                return;
            }

            try
            {
                NetworkStream stream = connectedTcpClient.GetStream();
                if(stream.CanWrite)
                {
                    string serverMessage = "This is a message from your server.";
                    // Convert string message to byte array
                    byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                    // Write byte array to socketConnection stream.
                    stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                    Console.WriteLine("Server sent his message - should be received by client");
                }
            }
            catch(SocketException socketException)
            {
                Console.WriteLine("Socket exception: " + socketException);
            }
        }
    }
}
