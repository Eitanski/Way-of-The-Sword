using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace Game1
{
    public class Sprite
    {
        #region Fields

        protected AnimationManager _animationManager;

        protected Dictionary<string, Animation> _animations;

        protected Vector2 _position;

        protected Texture2D _texture;

        private bool _air = false;

        private float _relativePos;

        private bool _direction = true; // right - true, left - false

        private bool _stun = false;

        private bool idle = false;

        #endregion

        #region Properties

        public Input Input;

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                if (_animationManager != null)
                    _animationManager.Position = _position;
            }
        }

        public float Speed = 5f;

        public Vector2 Velocity;

        public Vector2 Acceleration = new Vector2(0f, 2f);

        #endregion

        #region Methods

        public virtual void Draw(SpriteBatch spriteBatch, GraphicsDevice device)
        {
            if (_texture != null)
            {
                spriteBatch.Draw(_texture, new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height), Color.White);
            }
            else if (_animationManager != null)
                _animationManager.Draw(spriteBatch);
            else throw new Exception("This ain't right..!");
        }

        public virtual void DoAction()
        {
            if (Keyboard.GetState().IsKeyDown(Input.Left))
            {
                Communicator.SendMovementRequest("l");
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Right))
            {
                Communicator.SendMovementRequest("r");
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Attack1))
            {
                Communicator.SendAttack1Request();
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Attack2))
            {
                Communicator.SendAttack2Request();
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Jump))
            {
                Communicator.SendJumpRequest("r");
            }
            else
            {
                idle = true;
            }
            /*
            if(_air)
            {
                Velocity += Acceleration;
                if(Position.Y >= _relativePos)
                {
                    _air = false;
                    Velocity = Vector2.Zero;
                }
            }

            if (!_stun)
            {
                if ((Keyboard.GetState().IsKeyDown(Input.Up) || Keyboard.GetState().IsKeyDown(Input.Jump)) && !_air)
                {
                    _air = true;
                    _relativePos = Position.Y;
                    Velocity.Y = -Speed - 20f;
                }

                else if (Keyboard.GetState().IsKeyDown(Input.Attack1))
                {
                    _stun = true;
                    _attack1 = true;
                }

                else if (Keyboard.GetState().IsKeyDown(Input.Attack2))
                {
                    _stun = true;
                    _attack2 = true;
                }

                else if (Keyboard.GetState().IsKeyDown(Input.Left))
                {
                    Velocity.X = -Speed;
                    _direction = false;
                }

                else if (Keyboard.GetState().IsKeyDown(Input.Right))
                {
                    Velocity.X = Speed;
                    _direction = true;
                }
            }
            */
        }

        protected virtual void SetAnimations()
        {
            string[] chain;
            int code;
            if (idle)
            {
                if(_direction)
                    _animationManager.Play(_animations["IdleRight"]);
                else
                    _animationManager.Play(_animations["IdleLeft"]);
                idle = !idle;
            }
            else
            {
                chain = Communicator.Receive();
                if (chain[0] == "1")
                {
                    code = int.Parse(chain[1]);
                    switch (code)
                    {
                        case 200:
                            if (chain[3] == "r")
                            {
                                _animationManager.Play(_animations["RunRight"]);
                                Velocity.X = Speed;
                                _direction = true;
                            }
                            else
                            {
                                _animationManager.Play(_animations["RunLeft"]);
                                Velocity.X = -Speed;
                                _direction = false;
                            }
                            break;
                        case 201:
                            break;
                        case 202:
                            break;
                        case 203:
                            break;
                        default:
                            break;
                    }
                }
            }

            /*
            if (Velocity.Y > 0)
            {
                if (!_direction)
                    _animationManager.Play(_animations["FallLeft"]);
                else
                    _animationManager.Play(_animations["FallRight"]);
                Communicator.Send("Fall");
            }

            else if (Velocity.Y < 0)
            {
                if (!_direction)
                    _animationManager.Play(_animations["JumpLeft"]);
                else
                    _animationManager.Play(_animations["JumpRight"]);
                Communicator.Send("Jump");
            }

            else if (Velocity.X > 0)
            {
                _animationManager.Play(_animations["RunRight"]);
                Communicator.SendMovementRequest("r");
            }

            else if (Velocity.X < 0)
            {
                _animationManager.Play(_animations["RunLeft"]);
                Communicator.SendMovementRequest("l");
            }

            else if (_attack1)
            {
                if (!_direction)
                {
                    _animationManager.Play(_animations["Attack1Left"]);
                    Communicator.Send("Attack1Left");
                }
                else
                {
                    _animationManager.Play(_animations["Attack1Right"]);
                    Communicator.Send("Attack1Right");
                }
            }

            else if (_attack2)
            {
                if (!_direction)
                {
                    _animationManager.Play(_animations["AttackLeft"]);
                    Communicator.Send("AttackLeft");
                }
                else
                {
                    _animationManager.Play(_animations["AttackRight"]);
                    Communicator.Send("AttackRight");
                }
            }

            else
            {
                if (_direction)
                    _animationManager.Play(_animations["IdleRight"]);
                else
                    _animationManager.Play(_animations["IdleLeft"]);
                Communicator.Send("Idle");
            }
            */

        }

        public Sprite(Dictionary<string, Animation> animations)
        {
            _animations = animations;
            _animationManager = new AnimationManager(_animations.First().Value);
        }

        public Sprite(Texture2D texture)
        {
            _texture = texture;
        }

        public virtual void Update(GameTime gameTime)
        {

            DoAction();

            SetAnimations();

            _animationManager.Update(gameTime);

            //if(_stun)
            //{
            //    if (_animationManager._ended)
            //    {
            //        _animationManager.Stop();
            //        if(_direction)
            //            _animationManager.Play(_animations["IdleRight"]);
            //        else
            //            _animationManager.Play(_animations["IdleLeft"]);
            //
            //        _stun = false;
            //        _attack1 = false;
            //        _attack2 = false;
            //    }
            //}

            Position += Velocity;

            Velocity.X = 0;

            if(!_air)
            {
                Velocity = Vector2.Zero;
            }
        }

        #endregion
    }
}