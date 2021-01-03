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
        private Dictionary<TcpClient, Player> players = new Dictionary<TcpClient,Player>();
        public void Run()
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), 11111);
            TcpClient clientSocket = default(TcpClient);

            serverSocket.Start();
            Console.WriteLine("Server Started");

            while (true)
            {
                clientSocket = serverSocket.AcceptTcpClient();
                players.Add(clientSocket, new Player());
                SendId(players[clientSocket], clientSocket.GetStream());
                UpdateId();
                Console.WriteLine("Client No: " + players[clientSocket].id.ToString() + " started!");
                Console.WriteLine("Connected to " + clientSocket.Client.RemoteEndPoint.ToString());
                Thread newClient = new Thread(new ParameterizedThreadStart(HandleClient));
                newClient.Start(clientSocket);
            }
        }
        
        public void UpdateId()
        {
            foreach (Player client in players.Values)
            {
                Disperse("600", client, client.Position.X.ToString().Length + client.Position.X.ToString() + "1" + (client.Direction ? 1 : 0));
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
        
        private void Disperse(string code, Player player, string data = "")
        {
            foreach(KeyValuePair<TcpClient,Player> val in players)
            {
                Send(code + player.chunk + data , val.Key.GetStream());
            }
        }

        private void Send(string msg, NetworkStream stream)
        {
            try
            {
                byte[] message = Encoding.ASCII.GetBytes(msg + "1" + "e");
                //Console.WriteLine("Sent from server: " + msg + "1" + "e");
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
            if (!player.Stun)
            {
                player.Move(dir);
                Disperse("200", player, "1" + dir);
            }
        }

        public void SendAttack1Response(Player player, NetworkStream stream)
        {
            if (player.CanDo())
            {
                player.Stun = true;
                Disperse("201", player);
            }         
        }

        public void SendAttack2Response(Player player, NetworkStream stream)
        {
            if (player.CanDo())
            {
                player.Stun = true;
                Disperse("202", player);
            }
        }

        public void SendJumpResponse(Player player, NetworkStream stream)
        {
            if (player.CanDo())
            {
                player.Air = true;
                Disperse("203", player);
            }
        }

        public void SendId(Player player, NetworkStream stream)
        {
            Send("600" + player.chunk, stream);
        }

        public void ManageRequest(string req, TcpClient clientSocket)
        {
            NetworkStream stream = clientSocket.GetStream();
            string[] chain = req.Split(new char[]{'&'});
            int code = int.Parse(chain[0]);
            //Console.WriteLine("request from client: " + req);
            switch (code)
            {
                case 100:
                    SendMovementResponse(players[clientSocket], chain[2], stream);
                    break;
                case 101:
                    SendAttack1Response(players[clientSocket], stream);
                    break;
                case 102:
                    SendAttack2Response(players[clientSocket], stream);
                    break;      
                case 103:
                    SendJumpResponse(players[clientSocket], stream);
                    break;
                case 300:
                    players[clientSocket].Stun = false;
                    break;
                case 400:
                    players[clientSocket].Air = false;
                    break;
            }

        }
    
    }
}
