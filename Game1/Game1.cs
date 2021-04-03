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
        public enum champions {Feng,Knight, Ronin };
        public HitBoxFileManager hitBoxManager = new HitBoxFileManager();

        private Sprite background;

        public static UiSystem UiSystem;

        public static Dictionary<champions, Dictionary<string, Animation>> animations;

        public static List<Sprite> sprites;

        public Game1()
        {
            Window.Title = "Way of The Sword";
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

            int idCount1 = 0;
            int idCount2 = 0;

            animations = new Dictionary<champions, Dictionary<string, Animation>>()
            {
                { champions.Feng, new Dictionary<string, Animation>()

                {{"Run_Left", new Animation(Content.Load<Texture2D>("Feng/Run_Left"), 8,idCount1++) },
                 { "Run_Right", new Animation(Content.Load<Texture2D>("Feng/Run_Right"), 8,idCount1++) },
                 { "Jump_Right", new Animation(Content.Load<Texture2D>("Feng/Jump_Right"),2, idCount1++) },
                 { "Jump_Left", new Animation(Content.Load<Texture2D>("Feng/Jump_Left"), 2, idCount1++) },
                 { "Fall_Right", new Animation(Content.Load<Texture2D>("Feng/Fall_Right"), 2, idCount1++) },
                 { "Fall_Left", new Animation(Content.Load<Texture2D>("Feng/Fall_Left"), 2, idCount1++) },
                 { "Idle_Right", new Animation(Content.Load<Texture2D>("Feng/Idle_Right"), 8, idCount1++) },
                 { "Idle_Left", new Animation(Content.Load<Texture2D>("Feng/Idle_Left"), 8, idCount1++) },
                 { "Attack1_Right", new Animation(Content.Load<Texture2D>("Feng/Attack1_Right"), 6, idCount1++) },
                 { "Attack_Right", new Animation(Content.Load<Texture2D>("Feng/Attack_Right"), 12, idCount1++) },
                 { "Attack1_Left", new Animation(Content.Load<Texture2D>("Feng/Attack1_Left"), 6, idCount1++) },
                 { "Attack_Left", new Animation(Content.Load<Texture2D>("Feng/Attack_Left"), 12, idCount1++) },
                 { "Take_Hit", new Animation(Content.Load<Texture2D>("Feng/Take_Hit"), 4, idCount1++) }} },

                {champions.Knight, new Dictionary<string, Animation>()

                {{"Run_Left", new Animation(Content.Load<Texture2D>("Knight/Run_Left"), 10,idCount2++) },
                 { "Run_Right", new Animation(Content.Load<Texture2D>("Knight/Run_Right"), 10,idCount2++) },
                 { "Jump_Right", new Animation(Content.Load<Texture2D>("Knight/Jump_Right"),3, idCount2++) },
                 { "Jump_Left", new Animation(Content.Load<Texture2D>("Knight/Jump_Left"), 3, idCount2++) },
                 { "Fall_Right", new Animation(Content.Load<Texture2D>("Knight/Fall_Right"), 4, idCount2++) },
                 { "Fall_Left", new Animation(Content.Load<Texture2D>("Knight/Fall_Left"), 4, idCount2++) },
                 { "Idle_Right", new Animation(Content.Load<Texture2D>("Knight/Idle_Right"), 8, idCount2++) },
                 { "Idle_Left", new Animation(Content.Load<Texture2D>("Knight/Idle_Left"), 8, idCount2++) },
                 { "Attack1_Right", new Animation(Content.Load<Texture2D>("Knight/Attack1_Right"), 6, idCount2++) },
                 { "Attack_Right", new Animation(Content.Load<Texture2D>("Knight/Attack_Right"), 6, idCount2++) },
                 { "Attack1_Left", new Animation(Content.Load<Texture2D>("Knight/Attack1_Left"), 6, idCount2++) },
                 { "Attack_Left", new Animation(Content.Load<Texture2D>("Knight/Attack_Left"), 6, idCount2++) },
                 { "Take_Hit", new Animation(Content.Load<Texture2D>("Knight/Take_Hit"), 3, idCount2++) }}}
            };

            hitBoxManager.AquireData(GraphicsDevice);

            background = new Sprite(Content.Load<Texture2D>("maps/pixel hills"));

            Feng.ground = new Vector2(100, GraphicsDevice.Viewport.Height - 270);
            Knight.ground = new Vector2(100, GraphicsDevice.Viewport.Height - 130);

            champions champ = StartMenu.champion == "feng" ? champions.Feng : champions.Knight;

            Character character;
            Vector2 g;

            if (champ == champions.Feng)
            {
                character = new Feng();
                g = Feng.ground;
            }
            else
            {
                character = new Knight();
                g = Knight.ground;
            }

            sprites = new List<Sprite>()
            {
                new Sprite(animations[champ],champ)
                {
                Position = new Vector2(g.X, g.Y),
                Id = Communicator.ClientId,
                Champion = character
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

    
