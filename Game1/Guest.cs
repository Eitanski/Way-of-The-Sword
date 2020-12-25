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
        public Guest(Dictionary<string, Animation> animations, int guestId) : base(animations)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (!Communicator.CanDo(Id)) idle = true;

            SetAnimations();

            BandAid();

            _animationManager.Update(gameTime);

            BandAid2();

            Position += Velocity;

            Velocity.X = 0;

        }

    }
}
