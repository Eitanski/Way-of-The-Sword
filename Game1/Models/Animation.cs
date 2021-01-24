using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public class Animation
    {
        public int CurrentFrame { get; set; }

        public int FrameCount { get; private set; }

        public int FrameHeight { get { return Texture.Height; } }

        public float FrameSpeed { get; set; }

        public int FrameWidth { get { return Texture.Width / FrameCount; } }

        public int ActualWidth { get { return FrameWidth * Fix; }}
        public int ActualHeight { get { return FrameHeight * Fix; } }

        public bool IsLooping { get; set; }

        public Texture2D Texture { get; private set; }

        public List<List<Hitbox>> Hitboxes { get; set; }

        public static int Fix { get; set; } = 2;

        public Animation(Animation other)
        {
            Texture = other.Texture;

            FrameCount = other.FrameCount;

            IsLooping = other.IsLooping;

            FrameSpeed = other.FrameSpeed;

            Hitboxes = new List<List<Hitbox>>();

            List<Hitbox> tmpList;

            foreach (List<Hitbox> boxes in other.Hitboxes)
            {
                tmpList = new List<Hitbox>();
                foreach (Hitbox box in boxes)
                    tmpList.Add(new Hitbox(box));
                Hitboxes.Add(tmpList);    
            }
        }

        public Animation(Texture2D texture, int frameCount, float frameSpeed = 0.08f)
        {
            Texture = texture;

            FrameCount = frameCount;

            IsLooping = true;

            FrameSpeed = frameSpeed;
        }
    }
}