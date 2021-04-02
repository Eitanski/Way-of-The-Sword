using Microsoft.Xna.Framework;
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

        public static UiSystem UiSystem;

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
            Communicator.clientShutDown();
            System.Environment.Exit(0);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var tex = Content.Load<Texture2D>("TestTextures/Test");

            var style = new UntexturedStyle(_spriteBatch)
            {
                Font = new GenericSpriteFont(Content.Load<SpriteFont>("Fonts/alagard")),
                TextScale = 2.0f,
                PanelTexture = new NinePatch(new TextureRegion(tex, 0, 8, 24, 24), 8),
                ButtonTexture = new NinePatch(new TextureRegion(tex, 24, 8, 16, 16), 4),
                ScrollBarBackground = new NinePatch(new TextureRegion(tex, 12, 0, 4, 8), 1, 1, 2, 2),
                ScrollBarScrollerTexture = new NinePatch(new TextureRegion(tex, 8, 0, 4, 8), 1, 1, 2, 2)
            };

            UiSystem = new UiSystem(Window, GraphicsDevice, style);

            var pnlOptions = new Panel(Anchor.Center, new Vector2(350, 400), positionOffset: Vector2.Zero) { IsHidden = true};
            var btnOptions = new Button(Anchor.TopLeft, new Vector2(200, 50), "Options") { PositionOffset = new Vector2(15, 15) };
            var btnHitbox = new Button(Anchor.TopLeft, new Vector2(220, 50), "HitboxLayout:") { PositionOffset = new Vector2(25, 25)};
            var prgHitbox = new Paragraph(Anchor.TopRight, 1, "Off", true) { PositionOffset = new Vector2(25, 33) };
            var btnExit = new Button(Anchor.Center, new Vector2(220, 50), "Exit Game") { PositionOffset = new Vector2(0, -70)};
            var btnOk = new Button(Anchor.BottomCenter, new Vector2(220, 50), "Ok") { PositionOffset = new Vector2(0, 30)};

            btnOptions.OnPressed = e =>
            {
                pnlOptions.IsHidden = false;
                foreach (var child in pnlOptions.GetChildren()) child.IsHidden = false;
                btnOptions.IsDisabled = true;
            };

            btnOk.OnPressed = e =>
            {
                pnlOptions.IsHidden = true;
                foreach (var child in pnlOptions.GetChildren()) child.IsHidden = true;
                btnOptions.IsDisabled = false;
            };

            btnHitbox.OnPressed = e => 
            {
                if(prgHitbox.Text == "Off")
                {
                    prgHitbox.Text = "On";
                    AnimationManager.HitboxLayout = true;
                }
                else
                {
                    prgHitbox.Text = "Off";
                    AnimationManager.HitboxLayout = false;
                }
            };

            btnExit.OnPressed = e =>
            {
                Exit();
            };

            pnlOptions.AddChild(btnHitbox);
            pnlOptions.AddChild(prgHitbox);
            pnlOptions.AddChild(btnExit);
            pnlOptions.AddChild(btnOk);
            UiSystem.Add("OptionsPanel", pnlOptions);
            UiSystem.Add("OptionsButton",btnOptions);

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

            UiSystem.Add(sprites[0].Id.ToString() + "h",sprites[0].healthBar);
            UiSystem.Add(sprites[0].Id.ToString() + "n", sprites[0].nickName);

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

            Communicator.newPlayerMutex.WaitOne();
            foreach (Sprite sprite in sprites)
            {
                sprite.Update(gameTime);
            }
            Communicator.newPlayerMutex.ReleaseMutex();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            UiSystem.DrawEarly(gameTime, _spriteBatch);

            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin();

            background.Draw(_spriteBatch, GraphicsDevice);

            Communicator.newPlayerMutex.WaitOne();
            foreach (Sprite sprite in sprites)
                sprite.Draw(_spriteBatch, GraphicsDevice);
            Communicator.newPlayerMutex.ReleaseMutex();

            _spriteBatch.End();

            UiSystem.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }
       
    }
}

    
