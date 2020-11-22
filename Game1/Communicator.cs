﻿using System;
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
                byte[] message = Encoding.ASCII.GetBytes(msg + "1" + "e");
                client.Send(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static string[] Receive() // returns the response in a string array
        {
            byte[] buffer = new byte[1024];
            string part, chain = "";

            int numByte = client.Receive(buffer, 1, SocketFlags.None), len;
            part = Encoding.ASCII.GetString(buffer, 0, numByte);
            chain += part + "&";
            numByte = client.Receive(buffer, 3, SocketFlags.None);
            part = Encoding.ASCII.GetString(buffer, 0, numByte);
            while (part != "e")
            {
                chain += part + "&";
                numByte = client.Receive(buffer, 1, SocketFlags.None); // read len
                len = int.Parse(Encoding.ASCII.GetString(buffer, 0, numByte));
                numByte = client.Receive(buffer, len, SocketFlags.None);
                part = Encoding.ASCII.GetString(buffer, 0, numByte);
            }

            Console.WriteLine("received client: " + chain);

            return chain.Substring(0, chain.Length - 1).Split(new char[] { '&' });
        }

        public static void SendEndofStun()
        {
            Send("300" + "2" + "p1");
        }

        public static void SendEndofAir()
        {
            Send("400" + "2" + "p1");
        }
        
        public static void SendMovementRequest(string where)
        {
            Send("100" + "2" + "p1" + "1" + where);
        }

        public static void SendAttack1Request()
        {
            Send("101" + "2" + "p1");
        }

        public static void SendAttack2Request()
        {
            Send("102" + "2" + "p1");
        }

        public static void SendJumpRequest()
        {
            Send("103" + "2" + "p1");
        }
    }
}
