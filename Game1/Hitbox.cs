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
                        pixelData.Add(Mode? Color.LimeGreen: Color.Red);
                    else
                        pixelData.Add(new Color(0, 0, 0, 0));
                }
            }
            Texture = new Texture2D(graphicsDevice, Width, Height);
            Texture.SetData(pixelData.ToArray());
        }

        public Hitbox(bool mode,float x, float y, float w, float h, int frameHeight, int frameWidth, GraphicsDevice graphicsDevice)
        {
            Width = (int)(w * frameWidth);
            Height = (int)(h * frameHeight);

            if (Width == 0) Width = 1;
            if (Height == 0) Height = 1;
            Mode = mode;

            SetHitbox(graphicsDevice);
            Offset = new Vector2(x * frameWidth, y * frameHeight);
        }

        public Hitbox(Hitbox other)
        {
            Height = other.Height;
            Width = other.Width;
            Texture = other.Texture;
            Offset = other.Offset;
            Mode = other.Mode;
        }

    }
}
