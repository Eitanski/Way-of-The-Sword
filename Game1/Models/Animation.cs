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

        public bool IsLooping { get; set; }

        public Texture2D Texture { get; private set; }

        public List<Hitbox> Hitboxes { get; set; }

        public Animation(Animation other)
        {
            Texture = other.Texture;

            FrameCount = other.FrameCount;

            IsLooping = other.IsLooping;

            FrameSpeed = other.FrameSpeed;

            Hitboxes = new List<Hitbox>();

            foreach (Hitbox box in other.Hitboxes)
                Hitboxes.Add(new Hitbox(box));
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