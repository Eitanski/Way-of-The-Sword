using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;

namespace Game1
{
    class HitBoxFileManager
    {
        private JObject jsonData;
        private Dictionary<Game1.champions, string> types;
        public HitBoxFileManager()
        {
            jsonData = JObject.Parse(File.ReadAllText(@"..\..\..\Hitboxes.json"));
            types = new Dictionary<Game1.champions, string>() { { Game1.champions.Feng, "Feng" }, { Game1.champions.Ronin, "Ronin" } };
        }

        public void AquireData(GraphicsDevice dev, int frameWidth, int frameHeight) // for each animation
        {
            dynamic tmpAnimation;
            dynamic tmpHitbox;
            foreach (var animationData in Game1.animations)
            {
                tmpAnimation = jsonData[types[animationData.Key]];
                foreach (var frameData in tmpAnimation)
                {
                    
                }
            }
            
        }

    }
}
