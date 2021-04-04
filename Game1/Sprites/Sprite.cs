using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Ui.Style;
using MLEM.Font;
using MLEM.Textures;

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

        private bool _enable = true;
        
        private float _relativePos;

        public bool _direction = true; // right - true, left - false

        private bool _stun = false;

        private bool _attack1 = false;

        private bool _attack2 = false;

        public bool _idle = false;

        private bool _hurt = false;

        private bool[] bools = new bool[3];

        public bool ooga = true;

        private Vector2 healthBarOffset;

        private Vector2 nickNameOffset;

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

        public ProgressBar healthBar = new ProgressBar(Anchor.TopLeft, new Vector2(90, 10), MLEM.Misc.Direction2.Left, 100);

        public Paragraph nickName = new Paragraph(Anchor.TopLeft, 1, "player", true);

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
                    _animationManager.Play(_animations["Attack_Left"]);
                else
                    _animationManager.Play(_animations["Attack_Right"]);
            }
            else if(_hurt && !_stun)
            {
                if (healthBar.CurrentValue == healthBar.MaxValue)
                {
                    _stun = true;
                    if (!_direction)
                        _animationManager.Play(_animations["Death_Left"]);
                    else
                        _animationManager.Play(_animations["Death_Right"]);
                }
                else
                {
                    _stun = true;
                    if (!_direction)
                        _animationManager.Play(_animations["Hurt_Left"]);
                    else
                        _animationManager.Play(_animations["Hurt_Right"]);
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
                    if(ooga) Communicator.SendEndofStun();
                    _idle = true; 
                    _stun = false;
                    _attack1 = false;
                    _attack2 = false;
                    _hurt = false;

                    if(_animationManager._animation.AnimationId == 15 || _animationManager._animation.AnimationId == 14 && ooga)
                    {
                        _enable = false;
                        Game1.pnlDefeat.IsHidden = false;
                        healthBar.IsHidden = true;
                        nickName.IsHidden = true;
                        Communicator.SendExit();
                    }

                    bools[2] = false;
                    bools[1] = false;
                }
            }

            if (!_air)
            {
                Velocity.Y = 0;
            }   

        }

        public virtual void DoAction()
        {
            if (Keyboard.GetState().IsKeyDown(Champion.Input.Jump) && !bools[0] && !_stun)
            {
                Communicator.SendJumpRequest();
                bools[0] = true;
            }
            else if (Keyboard.GetState().IsKeyDown(Champion.Input.Attack1) && !bools[1] && !_air)
            {
                Communicator.SendAttack1Request();
                bools[1] = true;
            }
            else if (Keyboard.GetState().IsKeyDown(Champion.Input.Attack2) && !bools[2] && !_air)
            {   
                Communicator.SendAttack2Request();
                bools[2] = true;
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
                    if(ooga) Communicator.SendEndofAir();
                    bools[0] = false;
                }
            }

            if (!_stun)
            {
                if (_idle && !Communicator.CanDo(Id))
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
                            case 909:
                                Game1.vicCount++;
                                Game1.btnVictory.Text.Text = "Victory! You have slain the #" + Game1.vicCount + " Enemy.";
                                Game1.btnVictory.IsHidden = false;
                                break;
                            case 500:
                                _hurt = true;
                                healthBar.CurrentValue += 10;
                                break;
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

        public Sprite(Dictionary<string, Animation> animations, Game1.champions champ, string name)
        {
            _animations = animations;
            _animationManager = new AnimationManager(_animations.First().Value);

            nickName.Text = name;

            if (champ == Game1.champions.Feng)
            {
                healthBarOffset = Feng.healthBarOffset;
                nickNameOffset = Feng.nickNameOffset;
            }
            else
            {
                healthBarOffset = Knight.healthBarOffset;
                nickNameOffset = Knight.nickNameOffset;
            }
        }

        public Sprite(Texture2D texture)
        {
            _texture = texture;
        }

        public void UpdateMiscs()
        {
            healthBar.PositionOffset = Position + healthBarOffset;
            nickName.PositionOffset = Position + nickNameOffset;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (_enable)
            {
                DoAction();

                SetAnimations();

                setComplexAnimations();

                _animationManager.Update(gameTime);

                Retrieve();

                Position += Velocity;

                UpdateMiscs();

                Velocity.X = 0;
            }

        }

        #endregion
    }



}