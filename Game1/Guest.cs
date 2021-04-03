using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Game1
{
    class Guest : Sprite
    {      
        public Guest(Dictionary<string, Animation> animations, bool dir, Game1.champions champ) : base(animations, champ)
        {
            _direction = dir;

            if (champ == Game1.champions.Feng)
                Champion = new Feng();
            else
                Champion = new Knight();

            ooga = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Communicator.CanDo(Id)) _idle = true;

            SetAnimations();

            setComplexAnimations();

            _animationManager.Update(gameTime, false);

            Retrieve();

            Position += Velocity;

            UpdateMiscs();

            Velocity.X = 0;

        }

    }
}
