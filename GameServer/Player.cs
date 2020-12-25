using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;


namespace GameServer
{
    class Player
    {   

        private float _relativePos;

        private bool _direction = true; // right - true, left - false

        private bool _attack1 = false;

        private bool _attack2 = false;

        public float Speed = 5f;

        public Vector2 Velocity = Vector2.Zero;

        public Vector2 Acceleration = new Vector2(0f, 2f);

        public bool Stun = false;

        public bool Air = false;

        private static int count = 0;

        public int id = ++count;

        public string chunk;

        public Player()
        {
            chunk = id.ToString().Length.ToString() + id.ToString();
        }
        public void Move(string dir)
        {
            if (dir == "r")
            {
                Velocity.X = Speed;
                _direction = true;
            }
            else
            {
                Velocity.X = -Speed;
                _direction = false;
            }
        }

        public bool CanDo()
        {
            return !Air && !Stun;
        }
       


    }
}
