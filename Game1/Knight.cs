using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game1
{
    class Knight:Character
    {
        public Knight() : base(5f)
        {
            _animations = new Dictionary<string, string>
            {   { "RunLeft", "Knight/Run_Left"},
                { "RunRight", "Knight/Run_Right"},
                { "JumpRight", "Knight/Jump_Right"},
                { "JumpLeft", "Knight/Jump_Left"},
                { "FallRight", "Knight/Fall_Right"},
                { "FallLeft", "Knight/Fall_Left"},
                { "IdleRight", "Knight/Idle_Right"},
                { "IdleLeft", "Knight/Idle_Left"},
                { "Attack1Right", "Knight/Attack1_Right"},
                { "AttackRight", "Knight/Attack_Right"},
                { "Attack1Left", "Knight/Attack1_Left"},
                { "AttackLeft", "Knight/Attack_Left" } };

            Input = new Input()
            {
                Up = Keys.Up,
                Down = Keys.Down,
                Left = Keys.Left,
                Right = Keys.Right,
                Jump = Keys.Space,
                Attack1 = Keys.A,
                Attack2 = Keys.S,
                Attack3 = Keys.D
            };
        }
    }
}
