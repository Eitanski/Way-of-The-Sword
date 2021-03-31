﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Ui.Style;
using MLEM.Font;
using MLEM.Textures;


namespace Game1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public enum champions {Feng, Ronin };
        public HitBoxFileManager hitBoxManager = new HitBoxFileManager();

        private Sprite background;

        public UiSystem UiSystem;

        public static Dictionary<champions, Dictionary<string, Animation>> animations;

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

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            Communicator.SendExit();
            System.Environment.Exit(0);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var tex = Content.Load<Texture2D>("TestTextures/Test");
            var style = new UntexturedStyle(_spriteBatch)
            {
                Font = new GenericSpriteFont(Content.Load<SpriteFont>("Fonts/alagard")),
                TextScale = 1.5f,
                PanelTexture = new NinePatch(new TextureRegion(tex, 0, 8, 24, 24), 8),
                ButtonTexture = new NinePatch(new TextureRegion(tex, 24, 8, 16, 16), 4),
                ScrollBarBackground = new NinePatch(new TextureRegion(tex, 12, 0, 4, 8), 1, 1, 2, 2),
                ScrollBarScrollerTexture = new NinePatch(new TextureRegion(tex, 8, 0, 4, 8), 1, 1, 2, 2)
            };

            UiSystem = new UiSystem(Window, GraphicsDevice, style);

            var panel = new Panel(Anchor.Center, new Vector2(300, 300), positionOffset: Vector2.Zero);

            panel.AddChild(new Button(Anchor.AutoCenter, new Vector2(200, 50), "press me:3"));

            UiSystem.Add("ExampleUi", panel);

            int idCount = 0;

            animations = new Dictionary<champions, Dictionary<string, Animation>>()
            {
                { champions.Feng, new Dictionary<string, Animation>()

                {{"Run_Left", new Animation(Content.Load<Texture2D>("Feng/Run_Left"), 8,idCount++) },
                 { "Run_Right", new Animation(Content.Load<Texture2D>("Feng/Run_Right"), 8,idCount++) },
                 { "Jump_Right", new Animation(Content.Load<Texture2D>("Feng/Jump_Right"),2, idCount++) },
                 { "Jump_Left", new Animation(Content.Load<Texture2D>("Feng/Jump_Left"), 2, idCount++) },
                 { "Fall_Right", new Animation(Content.Load<Texture2D>("Feng/Fall_Right"), 2, idCount++) },
                 { "Fall_Left", new Animation(Content.Load<Texture2D>("Feng/Fall_Left"), 2, idCount++) },
                 { "Idle_Right", new Animation(Content.Load<Texture2D>("Feng/Idle_Right"), 8, idCount++) },
                 { "Idle_Left", new Animation(Content.Load<Texture2D>("Feng/Idle_Left"), 8, idCount++) },
                 { "Attack1_Right", new Animation(Content.Load<Texture2D>("Feng/Attack1_Right"), 6, idCount++) },
                 { "Attack_Right", new Animation(Content.Load<Texture2D>("Feng/Attack_Right"), 12, idCount++) },
                 { "Attack1_Left", new Animation(Content.Load<Texture2D>("Feng/Attack1_Left"), 6, idCount++) },
                 { "Attack_Left", new Animation(Content.Load<Texture2D>("Feng/Attack_Left"), 12, idCount++) },
                 { "Take_Hit", new Animation(Content.Load<Texture2D>("Feng/Take_Hit"), 4, idCount++) }} }
            };

            hitBoxManager.AquireData(GraphicsDevice);

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

            UiSystem.Update(gameTime);

            foreach (Sprite sprite in sprites)
            {
                sprite.Update(gameTime);
            }
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            UiSystem.DrawEarly(gameTime, _spriteBatch);

            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin();

            background.Draw(_spriteBatch, GraphicsDevice);
            
            foreach (Sprite sprite in sprites)
                sprite.Draw(_spriteBatch, GraphicsDevice);

            _spriteBatch.End();

            UiSystem.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }
       
    }
}

    
