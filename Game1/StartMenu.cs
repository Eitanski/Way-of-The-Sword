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
    class StartMenu : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public UiSystem titleUi;

        public UiSystem UiSystem;

        public static string champion = "none";

        public StartMenu()
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
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var tex = Content.Load<Texture2D>("TestTextures/Test");
            var fengPortrait = new TextureRegion(Content.Load<Texture2D>("Portraits/framedFeng"));
            var knightPortrait = new TextureRegion(Content.Load<Texture2D>("Portraits/framedKnight"));
            var fengPortraitGolden = new TextureRegion(Content.Load<Texture2D>("Portraits/framedFengGolden"));
            var knightPortraitGolden = new TextureRegion(Content.Load<Texture2D>("Portraits/framedKnightGolden"));

            var titleStyle = new UntexturedStyle(_spriteBatch)
            {
                Font = new GenericSpriteFont(Content.Load<SpriteFont>("Fonts/alagard")),
                TextScale = 8.0f,
                PanelTexture = new NinePatch(new TextureRegion(tex, 0, 8, 24, 24), 8),
                ButtonTexture = new NinePatch(new TextureRegion(tex, 24, 8, 16, 16), 4),
            };

            var style = new UntexturedStyle(_spriteBatch)
            {
                Font = new GenericSpriteFont(Content.Load<SpriteFont>("Fonts/alagard")),
                TextScale = 2.25f,
                PanelTexture = new NinePatch(new TextureRegion(tex, 0, 8, 24, 24), 8),
                ButtonTexture = new NinePatch(new TextureRegion(tex, 24, 8, 16, 16), 4),
                ScrollBarBackground = new NinePatch(new TextureRegion(tex, 12, 0, 4, 8), 1, 1, 2, 2),
                ScrollBarScrollerTexture = new NinePatch(new TextureRegion(tex, 8, 0, 4, 8), 1, 1, 2, 2)
            };

            UiSystem = new UiSystem(Window, GraphicsDevice, style);
            titleUi = new UiSystem(Window, GraphicsDevice, titleStyle);

            var prgTitle = new Paragraph(Anchor.TopCenter, 1, "Way of The", true) { PositionOffset = new Vector2(0, 20) };
            var prgSubTitle = new Paragraph(Anchor.TopCenter, 1, "Sword", true) { PositionOffset = new Vector2(0, 140) };
            titleUi.Add("title", prgTitle);
            titleUi.Add("subTitle", prgSubTitle);

            var panel = new Panel(Anchor.Center, new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), positionOffset: Vector2.Zero);
            var imgFeng = new Image(Anchor.Center, new Vector2(200, 200), fengPortrait) { IsHidden = true, PositionOffset = new Vector2(-150,20)};
            var btnFeng = new Button(Anchor.Center, imgFeng.Size, "")
            {
                HoveredColor = Color.Transparent,
                NormalColor = Color.Transparent,
                PositionOffset = new Vector2(-150, 20),
                IsHidden = true
            };
            var imgKnight = new Image(Anchor.Center, new Vector2(235, 235), knightPortrait) { IsHidden = true, PositionOffset = new Vector2(150, 32) }; ;
            var btnKnight = new Button(Anchor.Center, imgKnight.Size, "")
            {
                HoveredColor = Color.Transparent,
                NormalColor = Color.Transparent,
                PositionOffset = new Vector2(150, 32),
                IsHidden = true
            };
            var prgAuthor = new Paragraph(Anchor.Center, 1, "By Ethan Eshed", true) { PositionOffset = new Vector2(0, 80) };
            var btnPlay = new Button(Anchor.BottomCenter, new Vector2(200, 50), "Play")
            {
                PositionOffset = new Vector2(0, 35)
            };
            var btnFight = new Button(Anchor.BottomCenter, new Vector2(200, 50), "Fight!")
            {
                IsHidden = true,
                PositionOffset = new Vector2(0, 35)
            };
            var prgPlayers = new Paragraph(Anchor.Center, 1, "Select your Champion", true) { PositionOffset = new Vector2(0, -120), IsHidden = true };
            var btnBack = new Button(Anchor.TopRight, new Vector2(200, 50), "Back")
            {
                IsHidden = true,
                PositionOffset = new Vector2(15, 15),
                OnPressed = e =>
                {
                    prgTitle.IsHidden = false;
                    prgSubTitle.IsHidden = false;
                    prgAuthor.IsHidden = false;
                    e.IsHidden = true;
                    btnPlay.IsHidden = false;
                    prgPlayers.IsHidden = true;
                    imgFeng.IsHidden = true;
                    imgKnight.IsHidden = true;
                    btnFight.IsHidden = true;
                    btnFeng.IsHidden = true;
                    btnKnight.IsHidden = true;
                }
            };

            btnPlay.OnPressed = e =>
            {
                prgTitle.IsHidden = true;
                prgSubTitle.IsHidden = true;
                prgAuthor.IsHidden = true;
                e.IsHidden = true;
                btnBack.IsHidden = false;
                prgPlayers.IsHidden = false;
                imgFeng.IsHidden = false;
                imgKnight.IsHidden = false;
                btnFight.IsHidden = false;
                btnFeng.IsHidden = false;
                btnKnight.IsHidden = false;
            };

            btnFeng.OnPressed = e =>
            {
                imgKnight.Texture = knightPortrait;
                imgFeng.Texture = fengPortraitGolden;
                champion = "feng";
            };
            btnFeng.OnMouseEnter = e =>
            {
                imgFeng.Texture = fengPortraitGolden;
            };
            btnFeng.OnMouseExit = e =>
            {
                if(champion != "feng")
                    imgFeng.Texture = fengPortrait;
            };

            btnKnight.OnPressed = e =>
            {
                imgFeng.Texture = fengPortrait;
                imgKnight.Texture = knightPortraitGolden;
                champion = "knight";
            };
            btnKnight.OnMouseEnter = e =>
            {
                imgKnight.Texture = knightPortraitGolden;
            };
            btnKnight.OnMouseExit = e =>
            {
                if(champion != "knight")
                    imgKnight.Texture = knightPortrait;
            };

            btnFight.OnPressed = e =>
            {
                if(champion != "none")
                {
                    UiSystem.Dispose();
                    titleUi.Dispose();
                    Exit();
                    Communicator.Setup(champion);
                }
            };

            panel.AddChild(prgPlayers);
            panel.AddChild(prgAuthor);
            panel.AddChild(btnPlay);
            panel.AddChild(btnBack);
            panel.AddChild(imgFeng);
            panel.AddChild(imgKnight);
            panel.AddChild(btnFight);
            panel.AddChild(btnFeng);
            panel.AddChild(btnKnight);
            panel.AddChild(btnPlay);

            UiSystem.Add("StartMenu", panel);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            titleUi.Update(gameTime);

            UiSystem.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            UiSystem.DrawEarly(gameTime, _spriteBatch);

            titleUi.DrawEarly(gameTime, _spriteBatch);

            GraphicsDevice.Clear(Color.Transparent);

            UiSystem.Draw(gameTime, _spriteBatch);

            titleUi.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }


    }
}
