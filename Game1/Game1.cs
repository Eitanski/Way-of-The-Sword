using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.Collections.Generic;

namespace Game1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public enum champions {Feng, Njal };

        private Sprite background;

        public static Dictionary<champions,Dictionary<string, Animation>> animations;

        public static List<Sprite> sprites; 
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            animations = new Dictionary<champions, Dictionary<string, Animation>>()
            {
                { champions.Feng, new Dictionary<string, Animation>()

                {{"RunLeft", new Animation(Content.Load<Texture2D>("test/Run_Left"), 8){ Hitboxes = new List<Hitbox>()
                        { new Hitbox(new Vector2(150,150), 100, 20, GraphicsDevice) } } } ,
                 { "RunRight", new Animation(Content.Load<Texture2D>("test/Run_Right"), 8){ Hitboxes = new List<Hitbox>()
                        { new Hitbox(new Vector2(0,0), 100, 100, GraphicsDevice) } } },
                 { "JumpRight", new Animation(Content.Load<Texture2D>("test/Jump_Right"), 2){ Hitboxes = new List<Hitbox>()
                        { new Hitbox(new Vector2(0,0), 100, 100, GraphicsDevice) } } },
                 { "JumpLeft", new Animation(Content.Load<Texture2D>("test/Jump_Left"), 2){ Hitboxes = new List<Hitbox>()
                        { new Hitbox(new Vector2(0,0), 100, 100, GraphicsDevice) } } },
                 { "FallRight", new Animation(Content.Load<Texture2D>("test/Fall_Right"), 2){ Hitboxes = new List<Hitbox>()
                        { new Hitbox(new Vector2(0,0), 100, 100, GraphicsDevice) } } },
                 { "FallLeft", new Animation(Content.Load<Texture2D>("test/Fall_Left"), 2){ Hitboxes = new List<Hitbox>()
                        { new Hitbox(new Vector2(150,150), 100, 100, GraphicsDevice) } } },
                 { "IdleRight", new Animation(Content.Load<Texture2D>("test/Idle_Right"), 8){ Hitboxes = new List<Hitbox>()
                        { new Hitbox(new Vector2(192,154), 22, 22, GraphicsDevice), 
                          new Hitbox(new Vector2(182, 175), 35, 35, GraphicsDevice) ,
                          new Hitbox(new Vector2(180,210), 40, 40, GraphicsDevice) } } },
                 { "IdleLeft", new Animation(Content.Load<Texture2D>("test/Idle_Left"), 8){ Hitboxes = new List<Hitbox>()
                        { new Hitbox(new Vector2(185,154), 22, 22, GraphicsDevice),
                          new Hitbox(new Vector2(182, 175), 35, 35, GraphicsDevice) ,
                          new Hitbox(new Vector2(178,210), 40, 40, GraphicsDevice) } } },
                 { "Attack1Right", new Animation(Content.Load<Texture2D>("test/Attack1_Right"), 6){ Hitboxes = new List<Hitbox>()
                        { new Hitbox(new Vector2(0,0), 100, 100, GraphicsDevice) } } },
                 { "AttackRight", new Animation(Content.Load<Texture2D>("test/Attack_Right"), 12){ Hitboxes = new List<Hitbox>()
                        { new Hitbox(new Vector2(0,0), 100, 100, GraphicsDevice) } } },
                 { "Attack1Left", new Animation(Content.Load<Texture2D>("test/Attack1_Left"), 6){ Hitboxes = new List<Hitbox>()
                        { new Hitbox(new Vector2(0,0), 100, 100, GraphicsDevice) } } },
                 { "AttackLeft", new Animation(Content.Load<Texture2D>("test/Attack_Left"), 12){ Hitboxes = new List<Hitbox>()
                        { new Hitbox(new Vector2(0,0), 100, 100, GraphicsDevice) } } }} }
            };

            AnimationManager.HitboxLayout = true;

            background = new Sprite(Content.Load<Texture2D>("maps/pixel hills"));

            Sprite.ground = new Vector2(100, GraphicsDevice.Viewport.Height - 270);

            sprites = new List<Sprite>()
            {
                new Sprite(animations[champions.Feng]) 
                {
                Position = new Vector2(Sprite.ground.X, Sprite.ground.Y),
                Id = Communicator.ClientId,
                Champion = new Feng(),
                }
            };

            Thread thr = new Thread(Communicator.Receive);
            thr.Start();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Communicator.clientShutDown();
                Exit();
            }
            
            foreach (Sprite sprite in sprites)
            {
                sprite.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin();

            background.Draw(_spriteBatch, GraphicsDevice);

            foreach (Sprite sprite in sprites)
                sprite.Draw(_spriteBatch, GraphicsDevice);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
       
    }
}

    
