using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public class Hitbox
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public bool Mode { get; set; }

        public Vector2 Offset { get; set; }

        public Texture2D Texture { get; set; }

        public Rectangle rect { get; set; }

        private void SetHitbox(GraphicsDevice graphicsDevice)
        {
            List<Color> pixelData = new List<Color>();
            for (int y = 0; y < Height; y++) 
            {
                for (int x = 0; x < Width; x++)
                {
                    if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                        pixelData.Add(new Color(0, 255, 0, 0));
                    else
                        pixelData.Add(new Color(0, 0, 0, 0));
                }
            }
            Texture = new Texture2D(graphicsDevice, Width, Height);
            Texture.SetData(pixelData.ToArray());
        }

        public Hitbox(Vector2 offset, int width, int height, GraphicsDevice graphicsDevice)
        {
            Height = height;
            Width = width;
            SetHitbox(graphicsDevice);
            Offset = offset;
        }

        public Hitbox(int w, int h,string format, GraphicsDevice dev)
        {
            SetHitbox(dev);
            
            string[] parts = format.Split("|");
            float[] items = new float[4];
            string tmp;
            int idx = 0;

            for (int i = 0; i < 4; i++) 
            {
                tmp = parts[i].Substring(idx, parts[i].IndexOf('0', idx));
                idx += tmp.Length;
                if (tmp[tmp.Length] == ' ') tmp = tmp.Substring(0, tmp.Length - 1);
                items[i] = float.Parse(tmp);
            }

            Offset = new Vector2(w * items[0], h * items[1]);
            Width = (int)(w * items[2]);
            Height = (int)(h * items[3]);
        }

        public Hitbox(Hitbox other)
        {
            Height = other.Height;
            Width = other.Width;
            Texture = other.Texture;
            Offset = other.Offset;
        }

    }
}
