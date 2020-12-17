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
        private Dictionary<Player, TcpClient> players = new Dictionary<Player, TcpClient>();
        private List<Socket> sockets = new List<Socket>();
        private int count = 0;
        public void Run()
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), 11111);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;

            serverSocket.Start();
            Console.WriteLine("Server Started");

            counter = 0;
            while (true)
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine("Client No:" + Convert.ToString(counter) + " started!");
                Console.WriteLine("Connected to " + clientSocket.Client.RemoteEndPoint.ToString());
                Thread newClient = new Thread(new ParameterizedThreadStart(HandleClient));
                newClient.Start(clientSocket);

            }

        }


        public void HandleClient(Object clientSocket)
        {
            while (true)
                ManageRequest(Receive((TcpClient)clientSocket), (TcpClient)clientSocket);
        }

        private string Receive(TcpClient clientSocket)
        {
            byte[] buffer = new byte[1024];
            string part, chain = "";

            NetworkStream stream = clientSocket.GetStream();

            int numByte = stream.Read(buffer, 0, 3), len;
            part = Encoding.ASCII.GetString(buffer, 0, numByte);
            while(part != "e")
            {
                chain += part + "&";
                numByte = stream.Read(buffer, 0, 1); // read len
                len = int.Parse(Encoding.ASCII.GetString(buffer, 0, numByte));
                numByte = stream.Read(buffer, 0, len);
                part = Encoding.ASCII.GetString(buffer, 0, numByte);
            }

            return chain.Substring(0, chain.Length - 1); 
        }
        


        private void Send(string msg, NetworkStream stream)
        {
            try
            {
                byte[] message = Encoding.ASCII.GetBytes(msg + "1" + "e");
                //Console.WriteLine("Sent: " + msg);
                stream.Write(message, 0, message.Length);
                stream.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        public void SendMovementResponse(Player player, string dir, NetworkStream stream)
        {
            if (player.Stun)
            {
                Send("0" + "200" + "2" + "p1" + "1" + dir, stream);
            }
            else
            {
                player.Move(dir);
                Send("1" + "200" + "2" + "p1" + "1" + dir, stream);
            } 
        }

        public void SendAttack1Response(Player player, NetworkStream stream)
        {
            if (player.Stun || player.Air)
            {
                Send("0" + "201" + "2" + "p1", stream);
            }
            else
            {
                player.Stun = true;
                Send("1" + "201" + "2" + "p1", stream);
            }
            
        }

        public void SendAttack2Response(Player player, NetworkStream stream)
        {
            if (player.Stun || player.Air)
            {
                Send("0" + "202" + "2" + "p1", stream);
            }
            else
            {
                player.Stun = true;
                Send("1" + "202" + "2" + "p1", stream);
            }
        }

        public void SendJumpResponse(Player player, NetworkStream stream)
        {
            if (player.Stun || player.Air)
                Send("0" + "203" + "2" + "p1", stream);
            else
            {
                Send("1" + "203" + "2" + "p1", stream);
                player.Air = true;
            }
        }

        public void ManageRequest(string req, TcpClient clientSocket)
        {
            NetworkStream stream = clientSocket.GetStream();
            //Console.WriteLine(++count + " received: " + req + "  server stun: " + player1.Stun + " server air: " + player1.Air);
            string[] chain = req.Split(new char[]{'&'});
            int code = int.Parse(chain[0]);
            switch(code)
            {
                case 100:
                    SendMovementResponse(player1, chain[2], stream);
                    break;
                case 101:
                    SendAttack1Response(player1, stream);
                    break;
                case 102:
                    SendAttack2Response(player1, stream);
                    break;
                case 103:
                    SendJumpResponse(player1, stream);
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
