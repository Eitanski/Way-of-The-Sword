using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Game1
{
    static class Communicator
    {
        private static TcpClient client = new TcpClient();

        private static NetworkStream stream;

        private static Queue<string> messages = new Queue<string>();

        private static Mutex mutex = new Mutex();

        public static void clientShutDown()
        {
            client.Dispose(); 
            client.Close();
        }
        public static void Setup()
        {
            try
            {
                client.Connect("127.0.0.1", 11111);
                stream = client.GetStream();
                Console.WriteLine("client connected to: " + client.Client.RemoteEndPoint.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static bool CanDo()
        {
            return messages.Count != 0;
        }

        public static string GetAction()
        {
            mutex.WaitOne();
            string message = messages.Dequeue();
            mutex.ReleaseMutex();
            return message;
        }
        public static void Send(string msg)
        {
            try
            {
                byte[] message = Encoding.ASCII.GetBytes(msg + "1" + "e");
                stream.Write(message,0,message.Length);
                stream.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void Receive() // returns the response in a string array
        {
            byte[] buffer = new byte[1024];
            string part, chain = "";
            int numByte, len;

            while (true)
            {
                numByte = stream.Read(buffer, 0, 1); // 0 or 1
                part = Encoding.ASCII.GetString(buffer, 0, numByte);
                chain += part + "&";
                numByte = stream.Read(buffer, 0, 3); // action type
                part = Encoding.ASCII.GetString(buffer, 0, numByte);
                while (part != "e")
                {
                    chain += part + "&";
                    numByte = stream.Read(buffer, 0, 1); // read len
                    len = int.Parse(Encoding.ASCII.GetString(buffer, 0, numByte));
                    numByte = stream.Read(buffer, 0, len);
                    part = Encoding.ASCII.GetString(buffer, 0, numByte);
                }

                Console.WriteLine("received client: " + chain);
                if (chain[0] == '1')
                {
                    mutex.WaitOne();
                    messages.Enqueue(chain.Substring(0, chain.Length - 1));
                    mutex.ReleaseMutex();
                }
                chain = "";
            }
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
