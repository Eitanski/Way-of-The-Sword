using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    class HitBoxFileManager
    {
        private string filePath = "C:\\Users\\משתמש\\Desktop\\Game1\\Game1\\hitboxes.txt";

        private string data;
        public HitBoxFileManager()
        {
            data = System.IO.File.ReadAllText(filePath);
        }

        public List<List<Hitbox>> AquireSingleData(GraphicsDevice dev, int frameWidth, int frameHeight) // for each animation
        {
            List<List<Hitbox>> res = new List<List<Hitbox>>();

            string green = data.Substring(0, data.IndexOf("#RED#"));
            string red = data.Substring(data.IndexOf("#RED#"));

            string[] greenHitBoxes = green.Split("FRAME");
            string[] redHitBoxes = red.Split("FRAME");

            string[] tmpRed;
            string[] tmpGreen;
            List<Hitbox> tmpList;

            for (int i = 0;i<greenHitBoxes.Length;i++)
            {
                tmpGreen = greenHitBoxes[i].Split('\n');
                tmpRed = redHitBoxes[i].Split('\n');
                tmpList = new List<Hitbox>();

                for (int j = 2; j < tmpGreen.Length - 1; j++) 
                {
                    tmpList.Add(new Hitbox(frameWidth, frameHeight, tmpGreen[j], dev));
                }
                for (int k = 2; k < tmpRed.Length - 1; k++)
                {
                    tmpList.Add(new Hitbox(frameWidth, frameHeight, tmpRed[k], dev));
                }

                res.Add(tmpList);
            }

            return res;
        }

    }
}
