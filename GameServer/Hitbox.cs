using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace GameServer
{
    public class Hitbox
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public bool Mode { get; set; }
        public Vector2 Offset { get; set; }
        public Hitbox(bool mode, float x, float y, float w, float h, int frameHeight, int frameWidth)
        {
            Width = (int)(w * frameWidth);
            Height = (int)(h * frameHeight);
            Offset = new Vector2(x * frameWidth, y * frameHeight);
            Mode = mode;
        }
        public Hitbox(Hitbox other)
        {
            Height = other.Height;
            Width = other.Width;
            Offset = other.Offset;
            Mode = other.Mode;


        }
    }
}
