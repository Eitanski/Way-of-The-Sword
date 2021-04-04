using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.IO;

namespace GameServer
{
    class HitboxManager
    {

        private JObject jsonData;
        private Dictionary<Player.Champions,string> types = new Dictionary<Player.Champions, string>() { { Player.Champions.Feng, "Feng" }, { Player.Champions.Knight, "Knight" } };
        static public Dictionary<Player.Champions, Dictionary<string, List<List<List<Hitbox>>>>> HitboxData { get; set; }
        public HitboxManager()
        {
            jsonData = JObject.Parse(File.ReadAllText(@"..\..\Hitboxes.json"));
            HitboxData = new Dictionary<Player.Champions, Dictionary<string, List<List<List<Hitbox>>>>>()
            {
                { Player.Champions.Feng, new Dictionary<string, List<List<List<Hitbox>>>>()

                {{"Run_Left", new List<List<List<Hitbox>>>()},
                 { "Run_Right", new List<List<List<Hitbox>>>()},
                 { "Jump_Right", new List<List<List<Hitbox>>>()},
                 { "Jump_Left", new List<List<List<Hitbox>>>()},
                 { "Fall_Right", new List<List<List<Hitbox>>>()},
                 { "Fall_Left", new List<List<List<Hitbox>>>()},
                 { "Idle_Right", new List<List<List<Hitbox>>>()},
                 { "Idle_Left", new List<List<List<Hitbox>>>()},
                 { "Attack1_Right", new List<List<List<Hitbox>>>()},
                 { "Attack_Right", new List<List<List<Hitbox>>>()},
                 { "Attack1_Left", new List<List<List<Hitbox>>>()},
                 { "Attack_Left", new List<List<List<Hitbox>>>() },
                 { "Hurt_Right", new List<List<List<Hitbox>>>()},
                 { "Hurt_Left", new List<List<List<Hitbox>>>()},
                 { "Death_Left", new List<List<List<Hitbox>>>()},
                 { "Death_Right", new List<List<List<Hitbox>>>()}
                 }},

                { Player.Champions.Knight, new Dictionary<string, List<List<List<Hitbox>>>>()

                {{"Run_Left", new List<List<List<Hitbox>>>()},
                 { "Run_Right", new List<List<List<Hitbox>>>()},
                 { "Jump_Right", new List<List<List<Hitbox>>>()},
                 { "Jump_Left", new List<List<List<Hitbox>>>()},
                 { "Fall_Right", new List<List<List<Hitbox>>>()},
                 { "Fall_Left", new List<List<List<Hitbox>>>()},
                 { "Idle_Right", new List<List<List<Hitbox>>>()},
                 { "Idle_Left", new List<List<List<Hitbox>>>()},
                 { "Attack1_Right", new List<List<List<Hitbox>>>()},
                 { "Attack_Right", new List<List<List<Hitbox>>>()},
                 { "Attack1_Left", new List<List<List<Hitbox>>>()},
                 { "Attack_Left", new List<List<List<Hitbox>>>() },
                 { "Hurt_Left", new List<List<List<Hitbox>>>()},
                 { "Hurt_Right", new List<List<List<Hitbox>>>()},
                 { "Death_Left", new List<List<List<Hitbox>>>()},
                 { "Death_Right", new List<List<List<Hitbox>>>()}
                 }}
            };

            AquireData();
        }

        public void AquireData() 
        {
            JObject tmpChamp;
            string champ;
            List<Hitbox> tmpBoxes;
            List<List<Hitbox>> tmpLists;
            int w, h;
            foreach (KeyValuePair<Player.Champions, Dictionary<string,List<List<List<Hitbox>>>>> animationData in HitboxData)
            {
                champ = types[animationData.Key];
                tmpChamp = (JObject)jsonData[champ];
                if(champ == "Feng")
                {
                    h = 400;
                    w = 400;
                }
                else
                {
                    w = 200;
                    h = 110;
                }
                foreach (var tmpAnimation in tmpChamp)
                {
                    foreach (var frameData in tmpAnimation.Value)
                    {
                        tmpLists = new List<List<Hitbox>>();
                        tmpBoxes = new List<Hitbox>();
                        foreach (var greenBox in frameData["Green"])
                        {
                            tmpBoxes.Add(new Hitbox(true, (float)greenBox["X"], (float)greenBox["Y"], (float)greenBox["W"], (float)greenBox["H"], h, w));
                        }
                        tmpLists.Add(tmpBoxes);

                        tmpBoxes = new List<Hitbox>();
                        foreach (var redBox in frameData["Red"])
                        {
                            tmpBoxes.Add(new Hitbox(true, (float)redBox["X"], (float)redBox["Y"], (float)redBox["W"], (float)redBox["H"], h, w));
                        }
                        tmpLists.Add(tmpBoxes);
                        animationData.Value[tmpAnimation.Key].Add(tmpLists);
                    }
                }
            }

        }

    }
}
