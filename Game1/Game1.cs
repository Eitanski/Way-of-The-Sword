﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;

namespace Game1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public enum champions {Feng, Ronin };
        public HitBoxFileManager hitBoxManager = new HitBoxFileManager();

        private Sprite background;

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

    
