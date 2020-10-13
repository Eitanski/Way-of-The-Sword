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
        private bool _air = false;

        private float _relativePos;

        private bool _direction = true; // right - true, left - false

        private bool _stun = false;

        private bool _attack1 = false;

        private bool _attack2 = false;

        public float Speed = 5f;

        public Vector2 Velocity;

        public Vector2 Acceleration = new Vector2(0f, 2f);

        public void Update()
        {

        }


    }
}
