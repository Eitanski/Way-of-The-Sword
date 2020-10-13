using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace GameServer
{
    class Server
    {
        public void Run()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111);

            Socket listener = new Socket(IPAddress.Parse("127.0.0.1").AddressFamily,
                         SocketType.Stream, ProtocolType.Tcp);
            try
            {

                listener.Bind(localEndPoint);
 
                listener.Listen(10);

                while (true)
                {

                    Console.WriteLine("Waiting connection ... ");
                    Socket clientSocket = listener.Accept();

                    byte[] bytes = new Byte[1024];
                    string data = null;

                    while (true)
                    {

                        int numByte = clientSocket.Receive(bytes);

                        data = Encoding.ASCII.GetString(bytes,
                                                   0, numByte);

                        

                        Console.WriteLine("Action executed: " + data);
                    }

                    
                    

                   
                    //clientSocket.Shutdown(SocketShutdown.Both);
                    //clientSocket.Close();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void SendMovementResponse()
        {

        }

        public void SendAttack1Response()
        {

        }

        public void SendAttack2Response()
        {

        }

        public void SendJumpResponse()
        {

        }

        public void ManageRequest(string req)
        {
            string[] chain = req.Split(new char[]{'&'});
            int code = int.Parse(chain[0]);
            switch(code)
            {
                case 100:
                    SendMovementResponse();
                    break;
                case 101:
                    SendAttack1Response();
                    break;
                case 102:
                    SendAttack2Response();
                    break;
                case 103:
                    SendJumpResponse();
                    break;
            }
        }
    
    }
}
