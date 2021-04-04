using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace GameServer
{
    class Server
    {
        private Dictionary<TcpClient, Player> players = new Dictionary<TcpClient,Player>();
        private HitboxManager hitboxManager = new HitboxManager();
        private Mutex collisionMutex = new Mutex();

        private string[] setter = new string[] {
            "Run_Left", 
             "Run_Right",
             "Jump_Right", 
             "Jump_Left", 
             "Fall_Right", 
             "Fall_Left", 
             "Idle_Right", 
             "Idle_Left", 
             "Attack1_Right",
             "Attack_Right",
             "Attack1_Left",
             "Attack_Left",
             "Hurt_Right",
             "Hurt_Left",
             "Death_Right",
             "Death_Left"};        
        public void Run()
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), 11111);
            TcpClient clientSocket = default(TcpClient);
            string[] tmpChampMsg;

            serverSocket.Start();
            Console.WriteLine("Server Started");

            while (true)
            {
                clientSocket = serverSocket.AcceptTcpClient();
                tmpChampMsg = Receive(clientSocket).Split(new char[] { '&' });
                if(tmpChampMsg[1] == "1")
                    players.Add(clientSocket, new Feng() { nickName = tmpChampMsg[2]});
                else
                    players.Add(clientSocket, new Knight() { nickName = tmpChampMsg[2]});
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
                Disperse("600", client, client.Position.X.ToString().Length + client.Position.X.ToString() + "1" + (client.Direction ? 1 : 0) + "1" + (client.champ == Player.Champions.Feng ? "1" : "0") + client.nickName.Length + client.nickName + client.Health.ToString().Length + client.Health);
            }
        }

        public void HandleClient(Object clientSocket)
        {
            Player player = players[(TcpClient)clientSocket];
            while (player.Alive)
                ManageRequest(Receive((TcpClient)clientSocket), (TcpClient)clientSocket);
        }

        private string Receive(TcpClient clientSocket)
        {
            byte[] buffer = new byte[1024];
            string part, chain = "";
            try
            {
                NetworkStream stream = clientSocket.GetStream();

                int numByte = stream.Read(buffer, 0, 3), len;
                part = Encoding.ASCII.GetString(buffer, 0, numByte);
                while (part != "e")
                {
                    chain += part + "&";
                    numByte = stream.Read(buffer, 0, 1); // read len
                    len = int.Parse(Encoding.ASCII.GetString(buffer, 0, numByte));
                    numByte = stream.Read(buffer, 0, len);
                    part = Encoding.ASCII.GetString(buffer, 0, numByte);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return chain.Substring(0, chain.Length - 1); 
        }

        public void DetectCollsions(Player attacker)
        {
            Vector2 tmpDef = new Vector2();
            Vector2 tmpAtk = new Vector2();
            bool attain = false; ;
            foreach (Player defender in players.Values)
            {
                if (defender.id == attacker.id || defender.Stun) continue;
                foreach (var redBox in attacker.Hitboxes[attacker.CurrentAnimation][attacker.CurrentFrame][1])
                {
                    tmpAtk = attacker.Position + redBox.Offset;
                    foreach (var greenBox in defender.Hitboxes[defender.CurrentAnimation][defender.CurrentFrame][0])
                    {
                        tmpDef = defender.Position + greenBox.Offset;
                        if (tmpAtk.X + redBox.Width >= tmpDef.X && tmpDef.X + greenBox.Width >= tmpAtk.X &&  // check for x
                           tmpAtk.Y + redBox.Height >= tmpDef.Y && tmpDef.Y + greenBox.Height >= tmpAtk.Y)   // check for y
                        {
                            SendHurt(defender);
                            defender.Stun = true;
                            defender.Health -= 10;
                            attain = true;
                        }
                        if (attain) break;
                    }
                    if (attain) break;
                }
                if (attain) attain = false;
            }
            
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
                //Console.WriteLine("Sent from server: " + msg);
                stream.Write(message, 0, message.Length);
                stream.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(msg);
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
                player.Aggro = true;
                player.Stun = true;
                Disperse("201", player);
            }         
        }

        public void SendAttack2Response(Player player, NetworkStream stream)
        {
            if (player.CanDo())
            {
                player.Aggro = true;
                player.Stun = true;
                Disperse("202", player);
            }
        }

        public void SendHurt(Player player)
        {
           Disperse("500", player);
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

        public void sendCancel(Player player)
        {
            Disperse("901", player);
        }

        public void ManageRequest(string req, TcpClient clientSocket)
        {
            NetworkStream stream = clientSocket.GetStream();
            string[] chain = req.Split(new char[]{'&'});
            int code = int.Parse(chain[0]);
            Player player = players[clientSocket];
            //if(player.id == 1 && code != 401) Console.WriteLine("request from client: " + req + " " + player.CurrentAnimation);
            switch (code)
            {
                case 900:
                    sendCancel(player);
                    player.Alive = false;
                    stream.Close();
                    players.Remove(clientSocket);
                    break;
                case 800:
                    player.Position.Y = float.Parse(chain[1]);
                    break;
                case 401:
                    player.CurrentFrame = int.Parse(chain[1]);
                    player.CurrentAnimation = setter[int.Parse(chain[2])];
                    DetectCollsions(player);
                    break;
                case 100:
                    SendMovementResponse(player, chain[2], stream);
                    break;
                case 101:
                    SendAttack1Response(player, stream);
                    break;
                case 102:
                    SendAttack2Response(player, stream);
                    break;      
                case 103:
                    SendJumpResponse(player, stream);
                    break;
                case 300:
                    player.Stun = false;
                    break;
                case 400:
                    player.Air = false;
                    player.Aggro = false;
                    break;
            }

        }
    
    }
}
