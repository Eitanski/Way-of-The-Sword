using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MLEM.Ui;
using MLEM.Ui.Elements;

namespace Game1
{
    static class Communicator
    {
        private static TcpClient client;

        public static bool ExitSoftware = false;

        private static NetworkStream stream;

        private static Dictionary<int, Tuple<Mutex, Queue<string[]>>> coms; 

        public static int ClientId = 0;

        private static bool newPlayer = false;

        private static bool banish = false;

        public static bool alive = true;

        public static string chunk;

        public static Mutex newPlayerMutex;

        public static void clientShutDown()
        {
            client.Dispose(); 
            client.Close();
        }

        public static void init()
        {
            client = new TcpClient();
            coms = new Dictionary<int, Tuple<Mutex, Queue<string[]>>>();
            newPlayer = false;
            banish = false;
            alive = true;
            newPlayerMutex = new Mutex();
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
        public static void Setup(string champion, string nickName)
        {
            byte[] buffer = new byte[8];
            try
            {
                init();
                client.Connect("127.0.0.1", 11111);
                stream = client.GetStream();
                SendChampion(champion, nickName);
                string[] response = Parse(buffer).Split(new char[] { '&' }); // receiving id
                newPlayer = false; 
                ClientId = int.Parse(response[1]);
                coms.Add(ClientId, new Tuple<Mutex, Queue<string[]>>(new Mutex(), new Queue<string[]>()));
                chunk = ClientId.ToString().Length + ClientId.ToString();
                Console.WriteLine("client number " + response[1] + " connected to: " + client.Client.RemoteEndPoint.ToString());
                alive = true;
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
                //if(msg.Substring(0,3) != "401") Console.WriteLine("sent from client " + msg);
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

            if (part == "600") newPlayer = true;

            if (part == "901") banish = true;

            while (part != "e")
            {
                chain += part + "&";
                numByte = stream.Read(buffer, 0, 1); // read len
                len = int.Parse(Encoding.ASCII.GetString(buffer, 0, numByte));
                numByte = stream.Read(buffer, 0, len);
                part = Encoding.ASCII.GetString(buffer, 0, numByte);
            }
            return chain;
        }

        public static void Receive() 
        {
            byte[] buffer = new byte[1024];
            string[] chain;
            int tmpId;
            string chainer;
            Game1.champions tmpChamp;
            Vector2 ground;
            while (alive)
            {
                chainer = Parse(buffer);
                chain = chainer.Split(new char[] { '&' });
                tmpId = int.Parse(chain[1]);
                if (newPlayer)
                {
                    if (!coms.ContainsKey(tmpId)) // create a new player
                    {
                        newPlayerMutex.WaitOne();
                        tmpChamp = chain[4] == "1" ? Game1.champions.Feng : Game1.champions.Knight;
                        ground = tmpChamp == Game1.champions.Feng ? Feng.ground : Knight.ground;
                        Game1.sprites.Add(new Guest(CloneAnimations(Game1.animations[tmpChamp]), chain[3] == "1", tmpChamp, chain[5],float.Parse(chain[6]))
                        {
                            Position = new Vector2(float.Parse(chain[2]), ground.Y),
                            Id = tmpId
                        });
                        Game1.UiSystem.Add(tmpId.ToString() + "h",Game1.sprites[Game1.sprites.Count - 1].healthBar);
                        Game1.UiSystem.Add(tmpId.ToString() + "n", Game1.sprites[Game1.sprites.Count - 1].nickName);
                        newPlayerMutex.ReleaseMutex();  
                        coms.Add(tmpId, new Tuple<Mutex, Queue<string[]>>(new Mutex(), new Queue<string[]>()));          
                    }
                    newPlayer = false;
                }
                else if (banish)
                {
                    if (ClientId == tmpId)
                    {
                        clientShutDown();
                        alive = false;
                    }
                    else
                    {
                        newPlayerMutex.WaitOne();
                        foreach (Sprite sprite in Game1.sprites)
                        {
                            if (sprite.Id == tmpId)
                            {
                                coms.Remove(tmpId);
                                Game1.sprites.Remove(sprite);
                                sprite.healthBar.IsHidden = true;
                                sprite.nickName.IsHidden = true;
                                break;
                            }
                        }
                        banish = false;
                        newPlayerMutex.ReleaseMutex();
                    }
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

        public static void SendExit()
        {
            if (alive)
                Send("900" + chunk);
            alive = false;
        }

        public static void SendChampion(string champ,string name)
        {
            Send("700" + "1" + (champ == "feng" ? 1 : 0) + name.Length + name + chunk);
        }

        public static void SetGround(float y)
        {
            Send("800"  + y.ToString().Length + y.ToString());
        }
    }
}
