using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;

namespace Game1
{
    public class HitBoxFileManager
    {
        private JObject jsonData;
        private Dictionary<Game1.champions, string> types;
        public HitBoxFileManager()
        {
            jsonData = JObject.Parse(File.ReadAllText(@"..\..\..\Hitboxes.json"));
            types = new Dictionary<Game1.champions, string>() { { Game1.champions.Feng, "Feng" }, { Game1.champions.Ronin, "Ronin" } };
        }

        public void AquireData(GraphicsDevice dev) // for each animation
        {
            JObject tmpChamp;
            string champ;
            List<Hitbox> tmpBoxes;
            int w, h;
            foreach (KeyValuePair<Game1.champions, Dictionary<string, Animation>> animationData in Game1.animations) 
            {
                champ = types[animationData.Key];
                tmpChamp = (JObject)jsonData[champ];
                foreach (var tmpAnimation in tmpChamp)
                {
                    animationData.Value[tmpAnimation.Key].Hitboxes = new List<List<Hitbox>>();
                    foreach (var frameData in tmpAnimation.Value)
                    {
                        tmpBoxes = new List<Hitbox>();
                        h = animationData.Value[tmpAnimation.Key].ActualHeight;
                        w = animationData.Value[tmpAnimation.Key].ActualWidth;
                        foreach (var greenBox in frameData["Green"])
                        {
                            tmpBoxes.Add(new Hitbox(true, (float)greenBox["X"], (float)greenBox["Y"], (float)greenBox["W"], (float)greenBox["H"], h, w, dev));
                        }
                        foreach (var redBox in frameData["Red"])
                        {
                            tmpBoxes.Add(new Hitbox(false, (float)redBox["X"], (float)redBox["Y"], (float)redBox["W"], (float)redBox["H"], h, w, dev));
                        }
                        animationData.Value[tmpAnimation.Key].Hitboxes.Add(tmpBoxes);
                    }
                    
                }
            }

            
        }

    }
}
