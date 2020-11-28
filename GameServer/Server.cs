using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;

namespace GameServer
{
    class Server
    {
        private Player player1 = new Player();
        private Player player2;
        private int count = 0;
        public void Run()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111);

            Socket listener = new Socket(IPAddress.Parse("127.0.0.1").AddressFamily,
                         SocketType.Stream, ProtocolType.Tcp);
            try
            {

                listener.Bind(localEndPoint);
                listener.Listen(10);

                Console.WriteLine("Waiting connection ... ");
                Socket temp = listener.Accept();

                Thread newClient = new Thread(new ParameterizedThreadStart(HandleClient));
                newClient.Start();

                    //clientSocket.Shutdown(SocketShutdown.Both);
                    //clientSocket.Close();
                
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }


        public void HandleClient(Object clientSocket)
        {
            while (true)
                ManageRequest(Receive((Socket)clientSocket), (Socket)clientSocket);
        }

        private string Receive(Socket soc)
        {
            byte[] buffer = new byte[1024];
            string part, chain = "";

            int numByte = soc.Receive(buffer, 3, SocketFlags.None), len;
            part = Encoding.ASCII.GetString(buffer, 0, numByte);
            while(part != "e")
            {
                chain += part + "&";
                numByte = soc.Receive(buffer, 1, SocketFlags.None); // read len
                len = int.Parse(Encoding.ASCII.GetString(buffer, 0, numByte));
                numByte = soc.Receive(buffer, len, SocketFlags.None);
                part = Encoding.ASCII.GetString(buffer, 0, numByte);
            }

            return chain.Substring(0, chain.Length - 1); 
        }
        
        private void Send(string msg, Socket clientSocket)
        {
            try
            {
                byte[] message = Encoding.ASCII.GetBytes(msg + "1" + "e");
                //Console.WriteLine("Sent: " + msg);
                clientSocket.Send(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        public void SendMovementResponse(Player player, string dir, Socket clientSocket)
        {
            if (player.Stun)
            {
                Send("0" + "200" + "2" + "p1" + "1" + dir, clientSocket);
            }
            else
            {
                player.Move(dir);
                Send("1" + "200" + "2" + "p1" + "1" + dir, clientSocket);
            } 
        }

        public void SendAttack1Response(Player player, Socket clientSocket)
        {
            if (player.Stun || player.Air)
            {
                Send("0" + "201" + "2" + "p1", clientSocket);
            }
            else
            {
                player.Stun = true;
                Send("1" + "201" + "2" + "p1", clientSocket);
            }
            
        }

        public void SendAttack2Response(Player player, Socket clientSocket)
        {
            if (player.Stun || player.Air)
            {
                Send("0" + "202" + "2" + "p1", clientSocket);
            }
            else
            {
                player.Stun = true;
                Send("1" + "202" + "2" + "p1", clientSocket);
            }
        }

        public void SendJumpResponse(Player player, Socket clientSocket)
        {
            if (player.Stun || player.Air)
                Send("0" + "203" + "2" + "p1", clientSocket);
            else
            {
                Send("1" + "203" + "2" + "p1", clientSocket);
                player.Air = true;
            }
        }

        public void ManageRequest(string req, Socket clientSocket)
        {
            //Console.WriteLine(++count + " received: " + req + "  server stun: " + player1.Stun + " server air: " + player1.Air);
            string[] chain = req.Split(new char[]{'&'});
            int code = int.Parse(chain[0]);
            switch(code)
            {
                case 100:
                    SendMovementResponse(player1, chain[2], clientSocket);
                    break;
                case 101:
                    SendAttack1Response(player1, clientSocket);
                    break;
                case 102:
                    SendAttack2Response(player1, clientSocket);
                    break;
                case 103:
                    SendJumpResponse(player1, clientSocket);
                    break;
                case 300:
                    player1.Stun = false;
                    break;
                case 400:
                    player1.Air = false;
                    break;
            }
        }
    
    }
}
