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

        public bool _direction = true; // right - true, left - false

        private bool _stun = false;

        private bool _attack1 = false;

        private bool _attack2 = false;

        public bool _idle = false;
        #endregion

        #region Properties
        public int Id;

        public static Vector2 ground = Vector2.Zero;

        public Character Champion;

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
        
        public void setComplexAnimations()
        {
            if (Velocity.Y > 0)
            {
                if (!_direction)
                    _animationManager.Play(_animations["Fall_Left"]);
                else
                    _animationManager.Play(_animations["Fall_Right"]);
            }
            else if (Velocity.Y < 0)
            {
                if (!_direction)
                    _animationManager.Play(_animations["Jump_Left"]);
                else
                    _animationManager.Play(_animations["Jump_Right"]);
            }
            else if (_attack1 && !_stun)
            {
                _stun = true;
                if (!_direction)
                {
                    _animationManager.Play(_animations["Attack1_Left"]);
                }
                else
                {
                    _animationManager.Play(_animations["Attack1_Right"]);
                }
            }
            else if (_attack2 && !_stun)
            {
                _stun = true;
                if (!_direction)
                {
                    _animationManager.Play(_animations["Attack_Left"]);
                }
                else
                {
                    _animationManager.Play(_animations["Attack_Right"]);
                }
            }
        }

        public void Retrieve()
        {
            if (_stun)
            {
                if (_animationManager._ended)
                {
                    _animationManager.Stop();
                    Communicator.SendEndofStun();
                    _idle = true;

                    _stun = false;
                    _attack1 = false;
                    _attack2 = false;
                }
            }

            if (!_air)
            {
                Velocity.Y = 0;
            }

        }

        public virtual void DoAction()
        {
            if (Keyboard.GetState().IsKeyDown(Champion.Input.Jump) && !_air)
            {
                Communicator.SendJumpRequest();
            }
            else if (Keyboard.GetState().IsKeyDown(Champion.Input.Attack1) && !_attack1)
            {
                Communicator.SendAttack1Request();
            }
            else if (Keyboard.GetState().IsKeyDown(Champion.Input.Attack2) && !_attack2)
            {   
                Communicator.SendAttack2Request();
            }
            else if (Keyboard.GetState().IsKeyDown(Champion.Input.Left))
            {
                Communicator.SendMovementRequest("l");
            }
            else if (Keyboard.GetState().IsKeyDown(Champion.Input.Right))
            {
                Communicator.SendMovementRequest("r");
            }
            else
            {
                if(!_air) _idle = true;
            }
        }

        protected virtual void SetAnimations()
        {
            string[] chain;
            int code;   
            
            if (_air)
            {
                Velocity += Acceleration;
                if (Position.Y >= _relativePos)
                {
                    _air = false;
                    Velocity = Vector2.Zero;
                    Communicator.SendEndofAir();
                }
            }
                               
            if (!_stun)
            {
                if (_idle)
                {
                    if (_direction)
                        _animationManager.Play(_animations["Idle_Right"]);
                    else
                        _animationManager.Play(_animations["Idle_Left"]);   
                    _idle = !_idle;
                }
                else
                {
                    if (Communicator.CanDo(Id))
                    {
                        chain = Communicator.GetAction(Id);
                        code = int.Parse(chain[0]);
                        switch (code)
                        {
                            case 200: // move
                                if (chain[2] == "r")
                                {
                                    _animationManager.Play(_animations["Run_Right"]);
                                    Velocity.X = Champion.Speed;
                                    _direction = true;
                                }
                                else
                                {
                                    _animationManager.Play(_animations["Run_Left"]);
                                    Velocity.X = -Champion.Speed;
                                    _direction = false;
                                }
                                break;
                            case 201: // attack1
                                _attack1 = true;
                                break;
                            case 202: // attack2
                                _attack2 = true;
                                break;
                            case 203: // jump
                                _idle = false;
                                _air = true;
                                _relativePos = Position.Y;
                                Velocity.Y = -Champion.Speed - 20f;
                                break;
                        }
                    }
                }
            }
        }

        public Sprite() { }

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

            setComplexAnimations();

            _animationManager.Update(gameTime);

            Retrieve();
        
            Position += Velocity;

            Velocity.X = 0;
        }

        #endregion
    }
}