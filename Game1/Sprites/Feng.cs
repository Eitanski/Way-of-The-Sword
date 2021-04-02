﻿using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game1
{
    public class Feng : Character
    {
        public Feng() : base(5f)
        {
            _animations = new Dictionary<string, string>
            {   { "RunLeft", "test/Run_Left"},
                { "RunRight", "test/Run_Right"},
                { "JumpRight", "test/Jump_Right"},
                { "JumpLeft", "test/Jump_Left"},
                { "FallRight", "test/Fall_Right"},
                { "FallLeft", "test/Fall_Left"},
                { "IdleRight", "test/Idle_Right"},
                { "IdleLeft", "test/Idle_Left"},
                { "Attack1Right", "test/Attack1_Right"},
                { "AttackRight", "test/Attack_Right"},
                { "Attack1Left", "test/Attack1_Left"},
                { "AttackLeft", "test/Attack_Left" } };

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
