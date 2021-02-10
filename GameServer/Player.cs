using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;


namespace GameServer
{
    public class Player
    {
        public Dictionary<string, List<List<List<Hitbox>>>> Hitboxes { get; set; } 
        public enum Champions { Feng,Ronin}
        public int CurrentFrame { get; set; }
        public string CurrentAnimation { get; set; } = "Idle_Right";

        private float _relativePos;

        private bool _attack1 = false;

        private bool _attack2 = false;

        public float Speed = 5f;

        public Vector2 Velocity = Vector2.Zero;

        public Vector2 Position = new Vector2(100, 0);

        public Vector2 Acceleration = new Vector2(0f, 2f);

        public bool Stun = false;

        public bool Air = false;

        private static int count = 0;

        public int id = ++count;

        public string chunk;
        public bool Direction { get; set; } = true; // right - true, left - false

        public bool Aggro { get; set; } = false;

        public virtual void SetHitboxData()
        {

        }

        public Player()
        {
            chunk = id.ToString().Length.ToString() + id.ToString();
        }
        public void Move(string dir)
        {
            if (dir == "r")
            {
                Velocity.X = Speed;
                Direction = true;
            }
            else
            {
                Velocity.X = -Speed;
                Direction = false;
            }

            Position += Velocity;
        }

        public bool CanDo()
        {
            return !Air && !Stun;
        }
       
    }
}
