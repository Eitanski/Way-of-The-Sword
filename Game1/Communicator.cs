using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    static class Communicator
    {
        private static TcpClient client = new TcpClient();

        private static NetworkStream stream;

        private static Dictionary<int, Tuple<Mutex, Queue<string[]>>> coms = new Dictionary<int, Tuple<Mutex, Queue<string[]>>>();

        public static int ClientId = 0;

        private static bool flagos = false;

        public static string chunk;

        public static void clientShutDown()
        {
            client.Dispose(); 
            client.Close();
        }

        static public Dictionary<string, Animation> CloneAnimations(Dictionary<string, Animation> input)
        {
            Dictionary<string, Animation> result = new Dictionary<string, Animation>();
            foreach (KeyValuePair<string, Animation> node in input)
            {
                result.Add(node.Key, new Animation(node.Value));
            }
            return result;
        }
        public static void Setup()
        {
            byte[] buffer = new byte[8];
            try
            {
                client.Connect("127.0.0.1", 11111);
                stream = client.GetStream();
                string[] response = Parse(buffer).Split(new char[] { '&' }); // receiving id
                flagos = false; 
                ClientId = int.Parse(response[1]);
                coms.Add(ClientId, new Tuple<Mutex, Queue<string[]>>(new Mutex(), new Queue<string[]>()));
                chunk = ClientId.ToString().Length + ClientId.ToString();
                Console.WriteLine("client number " + response[1] + " connected to: " + client.Client.RemoteEndPoint.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static bool CanDo(int Id)
        {
            return coms[Id].Item2.Count != 0;
        }

        public static string[] GetAction(int Id)
        {
            coms[Id].Item1.WaitOne();
            string[] message = coms[Id].Item2.Dequeue();
            coms[Id].Item1.ReleaseMutex();
            return message;
        }
        public static void Send(string msg)
        {
            try
            {
                if(ClientId == 1 && (msg[0] != '4' || msg[2] != '1')) Console.WriteLine("sent from client " + msg);
                byte[] message = Encoding.ASCII.GetBytes(msg + "1" + "e");
                stream.Write(message, 0, message.Length);
                stream.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static string Parse(byte[] buffer)
        {
            string part, chain = "";
            int numByte, len;

            numByte = stream.Read(buffer, 0, 3); // action type
            part = Encoding.ASCII.GetString(buffer, 0, numByte);

            if (part == "600") flagos = true;

            while (part != "e")
            {
                chain += part + "&";
                numByte = stream.Read(buffer, 0, 1); // read len
                len = int.Parse(Encoding.ASCII.GetString(buffer, 0, numByte));
                numByte = stream.Read(buffer, 0, len);
                part = Encoding.ASCII.GetString(buffer, 0, numByte);
            }
            if (ClientId == 1) Console.WriteLine("received client: " + chain);
            return chain;
        }

        public static void Receive() // returns the response in a string array
        {
            byte[] buffer = new byte[1024];
            string[] chain;
            int tmpId;

            while (true)
            {
                chain = Parse(buffer).Split(new char[] { '&' });
                tmpId = int.Parse(chain[1]);
                if (flagos)
                {
                    if (!coms.ContainsKey(tmpId)) // create a new player
                    {
                        Game1.sprites.Add(new Guest(CloneAnimations(Game1.animations[Game1.champions.Feng]), tmpId, chain[3] == "1",new Feng())
                        {
                            Position = new Vector2(float.Parse(chain[2]), Sprite.ground.Y),
                            Id = tmpId
                        }) ; 
                        coms.Add(tmpId, new Tuple<Mutex, Queue<string[]>>(new Mutex(), new Queue<string[]>()));          
                    }
                    flagos = false;
                }
                else
                {
                    coms[tmpId].Item1.WaitOne();
                    coms[tmpId].Item2.Enqueue(chain);
                    coms[tmpId].Item1.ReleaseMutex();
                }
            }
        }

        public static void SendEndofStun()
        {
            Send("300" + chunk);
        }

        public static void SendEndofAir()
        {
            Send("400" + chunk);
        }
        
        public static void SendMovementRequest(string where)
        {
            Send("100" + chunk + "1" + where);
        }

        public static void SendAttack1Request()
        {
            Send("101" + chunk);
        }

        public static void SendAttack2Request()
        {
            Send("102" + chunk);
        }

        public static void SendJumpRequest()
        {
            Send("103" + chunk);
        }

        public static void UpdateFrame(int num,int id)
        {
            Send("401" + (num > 9 ? "2" : "1") + num.ToString() + (id > 9 ? "2" : "1") + id.ToString() + chunk);
        }

    }
}
