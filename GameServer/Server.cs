using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;

namespace GameServer
{
    class Server
    {
        private Player player1 = new Player();
        private Player player2;
        private Socket clientSocket;
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
                    clientSocket = listener.Accept();
                    string data;
                    
                    while (true)
                    {
                        data = receive(clientSocket);
                        ManageRequest(data);
                        //Console.WriteLine("Action executed: " + data);
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

        private string receive(Socket soc)
        {
            byte[] bytes = new Byte[1024];
            int numByte = soc.Receive(bytes);
            return Encoding.ASCII.GetString(bytes, 0, numByte);
        }
        
        private void Send(string msg)
        {
            try
            {
                byte[] message = Encoding.ASCII.GetBytes(msg);
                //Console.WriteLine("Sent: " + msg);
                clientSocket.Send(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void SendMovementResponse(Player player, string dir)
        {
            if (player.CanMove(dir))
            {
                player.Move(dir);
                Send("1" + "&" + "200" + "&" + "p1" + "&" + dir);
            }
            else
                Send("0" + "&" + "200" + "&" + "p1" + "&" + dir);
        }

        public void SendAttack1Response(Player player)
        {
            
        }

        public void SendAttack2Response(Player player)
        {

        }

        public void SendJumpResponse(Player player)
        {

        }

        public void ManageRequest(string req)
        {
            //Console.WriteLine("received: " + req);
            string[] chain = req.Split(new char[]{'&'});
            int code = int.Parse(chain[0]);
            switch(code)
            {
                case 100:
                    SendMovementResponse(player1, chain[2]);
                    break;
                case 101:
                    SendAttack1Response(player1);
                    break;
                case 102:
                    SendAttack2Response(player1);
                    break;
                case 103:
                    SendJumpResponse(player1);
                    break;
            }
        }
    
    }
}
