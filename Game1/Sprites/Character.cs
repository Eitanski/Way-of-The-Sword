using System;
using System.Collections.Generic;
using System.Text;

namespace Game1
{
    public class Character
    {
        public Character(float speed)
        {
            Speed = speed;
        }

        protected Dictionary<string, string> _animations;

        public float Speed { get; set; }
        public Input Input { get; set; }
    }
}
