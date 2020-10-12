using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Game1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Sprite background;

        private List<Sprite> sprites; 
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Dictionary<string, Animation> animations = new Dictionary<string, Animation> 
            { { "RunLeft", new Animation(Content.Load<Texture2D>("Player/Run_Left"), 8) },
                { "RunRight", new Animation(Content.Load<Texture2D>("Player/Run_Right"), 8) },
                { "JumpRight", new Animation(Content.Load<Texture2D>("Player/Jump_Right"), 2) },
                { "JumpLeft", new Animation(Content.Load<Texture2D>("Player/Jump_Left"), 2) },
                { "FallRight", new Animation(Content.Load<Texture2D>("Player/Fall_Right"), 2) },
                { "FallLeft", new Animation(Content.Load<Texture2D>("Player/Fall_Left"), 2) },
                { "IdleRight", new Animation(Content.Load<Texture2D>("Player/Idle_Right"), 8) },
                { "IdleLeft", new Animation(Content.Load<Texture2D>("Player/Idle_Left"), 8) },
                { "Attack1Right", new Animation(Content.Load<Texture2D>("Player/Attack1_Right"), 6) },
                { "AttackRight", new Animation(Content.Load<Texture2D>("Player/Attack_Right"), 12) },
                { "Attack1Left", new Animation(Content.Load<Texture2D>("Player/Attack1_Left"), 6) },
                { "AttackLeft", new Animation(Content.Load<Texture2D>("Player/Attack_Left"), 12) }};

        background = new Sprite(Content.Load<Texture2D>("pixel hills"));
         
            sprites = new List<Sprite>()
            {
                new Sprite(animations) { 
                Position = new Vector2(100, GraphicsDevice.Viewport.Height - 270), 
                Input = new Input() { 
                Up = Keys.Up, 
                Down = Keys.Down, 
                Left = Keys.Left, 
                Right = Keys.Right, 
                Jump = Keys.Space, 
                Attack1 = Keys.A,
                Attack2 = Keys.S} } };
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Communicator.clientShutDown();
                Exit();
            }

            foreach (Sprite sprite in sprites)
                sprite.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            background.Draw(_spriteBatch,GraphicsDevice);

            foreach (Sprite sprite in sprites)
                sprite.Draw(_spriteBatch,GraphicsDevice);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
