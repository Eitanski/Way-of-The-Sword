using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Game1
{
    static class Communicator
    {
        private static Socket client;
        public static void clientShutDown()
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        public static void Setup()
        {
            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111);

                client = new Socket(IPAddress.Parse("127.0.0.1").AddressFamily,
                           SocketType.Stream, ProtocolType.Tcp);

                client.Connect(localEndPoint);

                Console.WriteLine("client connected to -> {0} ",
                              client.RemoteEndPoint.ToString());
                
                //byte[] messageReceived = new byte[1024];
                //int byteRecv = client.Receive(messageReceived);
                //Console.WriteLine("Message from Server -> {0}",
                //      Encoding.ASCII.GetString(messageReceived,
                //                                 0, byteRecv));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void Send(string msg)
        {
            try
            {
                byte[] message = Encoding.ASCII.GetBytes(msg);
                client.Send(message);
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e.Message); 
            }
        }

        public static void SendMovementRequest(string where)
        {
            Send("100" + "&" + "p1" + "&" + where);
        }
    }
}
