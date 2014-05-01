using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Yuuki2TheGame.Core;
using Yuuki2TheGame.Graphics;

namespace Yuuki2TheGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Engine gameEngine;
        Texture2D defaultTexture;
        public static List<SoundEffect> GameAudio = new List<SoundEffect>();

        public const int WORLD_WIDTH = 100;
        public const int WORLD_HEIGHT = 100;

        public const int GAME_WIDTH = 800;
        public const int GAME_HEIGHT = 600;

        public const int METER_LENGTH = 16;

        /// <summary>
        /// Contains the number of blocks that are on the screen.
        /// </summary>
        private Point blocksOnScreen;

        public bool DebugMode { get; set; }

        private SpriteFont font;

        public Game1()
        {
            DebugMode = false;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GAME_WIDTH;
            graphics.PreferredBackBufferHeight = GAME_HEIGHT;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";


            SoundEffect bgm = Content.Load<SoundEffect>("Backround");
            SoundEffect swing01 = Content.Load<SoundEffect>("swing01");
            GameAudio.Add(bgm);
            GameAudio.Add(swing01);

            blocksOnScreen = new Point (GAME_WIDTH / METER_LENGTH + 1, GAME_HEIGHT / METER_LENGTH + 1);
            gameEngine = new Engine(new Point(WORLD_WIDTH, WORLD_HEIGHT));




        }

        ControlManager controls = new ControlManager();

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            BindControls();
            int butts = 0;
            butts++;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            defaultTexture = Content.Load<Texture2D>(@"Tiles/default_tile");
            font = Content.Load<SpriteFont>("SegoeUI");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            controls.Update(gameTime);
            gameEngine.Update(gameTime);
            base.Update(gameTime);

        }

        public static void Debug(String str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Sprite bg = gameEngine.GetBackground(GAME_WIDTH, GAME_HEIGHT);
            IList<Sprite> tiles = gameEngine.GetView(blocksOnScreen.X, blocksOnScreen.Y, METER_LENGTH, METER_LENGTH);
            IList<Sprite> chars = gameEngine.GetCharacters(GAME_WIDTH, GAME_HEIGHT);
            bg.Texture = NameToTexture(bg.TextureID);
            ProcessSpriteGraphics(tiles);
            ProcessSpriteGraphics(chars);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            // draw bg first ALWAYS!
            spriteBatch.Draw(bg.Texture, bg.Destination, bg.Source, Color.Pink);
            foreach (Sprite sp in tiles)
            {
                spriteBatch.Draw(sp.Texture, sp.Destination, sp.Source, Color.White);
            }
            foreach (Sprite sp in chars)
            {
                spriteBatch.Draw(sp.Texture, sp.Destination, sp.Source, Color.White);
            }
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            MouseState currentMouse = Mouse.GetState();
            Vector2 pos = new Vector2(currentMouse.X, currentMouse.Y);

            spriteBatch.Draw(bg.Texture, pos, Color.White);
            if (DebugMode)
            {
                DrawDebugInfo();
            }
            if (gameEngine.ManualPhysStepMode)
            {
                DrawManualPhysInfo();
            }
            if (gameEngine.RecordPhysStep)
            {
                DrawRecordingPhysStep();
            }
            spriteBatch.End();
            base.Draw(gameTime);
  
            
        }

        private void BindControls()
        {
            controls.BindMouseButtonClick(MouseButton.LEFT, delegate(MouseButtonEventArgs e) {
                gameEngine.Click(e.X, e.Y);
            });
            KeyAction jump = delegate(KeyEventArgs e) {
                gameEngine.Player.Jump();
            };
            KeyAction startRight = delegate(KeyEventArgs e) {
                gameEngine.Player.StartMovingRight();
            };
            KeyAction startLeft = delegate(KeyEventArgs e) {
                gameEngine.Player.StartMovingLeft();
            };
            KeyAction stopRight = delegate(KeyEventArgs e) {
                gameEngine.Player.StopMovingRight();
            };
            KeyAction stopLeft = delegate(KeyEventArgs e) {
                gameEngine.Player.StopMovingLeft();
            };
            controls.BindKeyDown(Keys.Left, startLeft, false);
            controls.BindKeyDown(Keys.A, startLeft, false);
            controls.BindKeyUp(Keys.Left, stopLeft);
            controls.BindKeyUp(Keys.A, stopLeft);
            controls.BindKeyDown(Keys.Right, startRight, false);
            controls.BindKeyDown(Keys.D, startRight, false);
            controls.BindKeyUp(Keys.Right, stopRight);
            controls.BindKeyUp(Keys.D, stopRight);
            controls.BindKeyDown(Keys.W, jump, false);
            controls.BindKeyDown(Keys.Space, jump, false);
            controls.BindKeyDown(Keys.Up, jump, false);
            controls.BindKeyDown(Keys.F4, delegate(KeyEventArgs e) {
                DebugMode = !DebugMode;
            }, false);
            controls.BindKeyDown(Keys.F5, delegate(KeyEventArgs e) {
                gameEngine.ManualPhysStepMode = !gameEngine.ManualPhysStepMode;
            }, false);
            controls.BindKeyDown(Keys.F6, delegate(KeyEventArgs e) {
                gameEngine.RecordPhysStep = !gameEngine.RecordPhysStep;
            }, false);
            controls.BindKeyDown(Keys.F7, delegate(KeyEventArgs e) {
                gameEngine.Respawn();
            }, false);
            controls.BindKeyDown(Keys.F8, delegate(KeyEventArgs e) {
                if (gameEngine.ManualPhysStepMode) {
                    gameEngine.StepPhysics();
                }
            }, false);
        }

        private void DrawDebugInfo()
        {
            PlayerCharacter pc = gameEngine.Player;
            Vector2 s = pc.PhysPosition;
            Vector2 v = pc.Velocity;
            Vector2 a = pc.Acceleration;
            Vector2 f = pc.Force;
            string[] debug = new string[5];
            debug[0] = string.Format("P:({0}, {1})", s.X, s.Y);
            debug[1] = string.Format("V:({0}, {1})", v.X, v.Y);
            debug[2] = string.Format("A:({0}, {1})", a.X, a.Y);
            debug[3] = string.Format("F:({0}, {1})", f.X, f.Y);
            debug[4] = string.Format("M:{0}  C:{1}  G:{2}", pc.Mass, Convert.ToString(pc.ContactMask, 2).PadLeft(4, '0'), pc.IsOnGround());
            for (int i = 0; i < debug.Length; i++)
            {
                spriteBatch.DrawString(font, debug[i], new Vector2(5, i * 15), Color.Red);
            }
        }

        private void DrawManualPhysInfo()
        {
            string debug = "Hit F8 to step physics or F5 to turn on auto step";
            spriteBatch.DrawString(font, debug, new Vector2(200, 0), Color.Red);
        }

        private void DrawRecordingPhysStep()
        {
            string debug = "(recording)";
            spriteBatch.DrawString(font, debug, new Vector2(720, 0), Color.Red);
        }

        private void ProcessSpriteGraphics(IList<Sprite> sprites)
        {
            foreach (Sprite spr in sprites)
            {
                //TODO: use preloaded graphics
                spr.Texture = NameToTexture(spr.TextureID);
            }
        }

        /// <summary>
        /// Attempts to convert the resource name into a texture. If resource name is null, default texture is used.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Texture2D NameToTexture(string name)
        {
            return Content.Load<Texture2D>(name != null ? name : @"Tiles/default_tile");
        }
    }
}
