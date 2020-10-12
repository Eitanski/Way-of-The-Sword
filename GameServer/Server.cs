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
    
    }
}
