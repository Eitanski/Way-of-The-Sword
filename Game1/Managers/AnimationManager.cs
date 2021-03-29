using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public class AnimationManager
    {
        public Animation _animation;

        public float _timer;

        public bool _ended = false;

        public static bool HitboxLayout { get; set; } = false;

        public Vector2 Position { get; set; }

        public AnimationManager(Animation animation)
        {
            _animation = animation;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_animation.Texture,
                             new Rectangle(Position.ToPoint(), new Point(_animation.FrameWidth * 2, _animation.FrameHeight * 2)),
                             new Rectangle(_animation.CurrentFrame * _animation.FrameWidth,
                                           0,
                                           _animation.FrameWidth,
                                           _animation.FrameHeight),
                             Color.White);

            if (HitboxLayout)
                foreach (Hitbox hitbox in _animation.Hitboxes[_animation.CurrentFrame])
                    spriteBatch.Draw(hitbox.Texture, hitbox.rect, Color.White);
        }

        public void Play(Animation animation)
        {
            if (_animation == animation)
                return;

            _animation = animation;

            _animation.CurrentFrame = 0;

            _timer = 0;
        }

        public void Stop()
        {
            _timer = 0f;

            _animation.CurrentFrame = 0;
        }

        public void Update(GameTime gameTime, bool ooga = true)
        {
            _ended = false;
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer > _animation.FrameSpeed)
            {
                _timer = 0f;

                if(ooga) Communicator.UpdateFrame(_animation.CurrentFrame, _animation.AnimationId);

                _animation.CurrentFrame++;

                if (_animation.CurrentFrame >= _animation.FrameCount)
                {
                    _animation.CurrentFrame = 0;
                    _ended = true;
                }
            }

            if (HitboxLayout)
                foreach (Hitbox hitbox in _animation.Hitboxes[_animation.CurrentFrame])
                    hitbox.rect = new Rectangle((Position + hitbox.Offset).ToPoint(), new Point(hitbox.Width, hitbox.Height));                
                            
        }
    }
}