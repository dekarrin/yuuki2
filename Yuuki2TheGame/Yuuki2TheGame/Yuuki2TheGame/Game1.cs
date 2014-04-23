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

        public const int WORLD_WIDTH = 100;
        public const int WORLD_HEIGHT = 100;

        public const int GAME_WIDTH = 800;
        public const int GAME_HEIGHT = 600;

        public const int BLOCK_WIDTH = 16;
        public const int BLOCK_HEIGHT = 16;

        /// <summary>
        /// Contains the number of blocks that are on the screen.
        /// </summary>
        private Point blocksOnScreen;

        public bool DebugMode { get; set; }

        private bool debugKeyLocked = false;

        private SpriteFont font;

        public Game1()
        {
            DebugMode = true;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GAME_WIDTH;
            graphics.PreferredBackBufferHeight = GAME_HEIGHT;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            blocksOnScreen = new Point (GAME_WIDTH / BLOCK_WIDTH + 1, GAME_HEIGHT / BLOCK_HEIGHT + 1);
            gameEngine = new Engine(new Point(WORLD_WIDTH, WORLD_HEIGHT));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
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
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left))
            {
                gameEngine.Player.StartMovingLeft();
            }
            else if (keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right))
            {
                gameEngine.Player.StartMovingRight();
            }
            if (keyState.IsKeyUp(Keys.A) || keyState.IsKeyUp(Keys.Left))
            {
                gameEngine.Player.StopMovingLeft();
            }
            if (keyState.IsKeyUp(Keys.D) || keyState.IsKeyUp(Keys.Right))
            {
                gameEngine.Player.StopMovingRight();
            }
            // TODO: Individual key resets
            if (!pressedJump && (keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Space) || keyState.IsKeyDown(Keys.Up)))
            {
                gameEngine.Player.Jump();
                pressedJump = true;
            }
            else if (pressedJump && (keyState.IsKeyUp(Keys.W) && keyState.IsKeyUp(Keys.Space) && keyState.IsKeyUp(Keys.Up)))
            {
                pressedJump = false;
            }
            if (!debugKeyLocked && keyState.IsKeyDown(Keys.F4))
            {
                debugKeyLocked = true;
                DebugMode = !DebugMode;
            }
            else if (debugKeyLocked && keyState.IsKeyUp(Keys.F4))
            {
                debugKeyLocked = false;
            }
            if (!physKeyLocked && keyState.IsKeyDown(Keys.F5))
            {
                physKeyLocked = true;
                gameEngine.ManualPhysStepMode = !gameEngine.ManualPhysStepMode;
            }
            else if (physKeyLocked && keyState.IsKeyUp(Keys.F5))
            {
                physKeyLocked = false;
            }
            if (!physStepKeyLocked && keyState.IsKeyDown(Keys.F8) && gameEngine.ManualPhysStepMode)
            {
                physStepKeyLocked = true;
                gameEngine.StepPhysics();
            }
            else if (physStepKeyLocked && keyState.IsKeyUp(Keys.F8))
            {
                physStepKeyLocked = false;
            }
            // TODO: Add your update logic here
            gameEngine.Update(gameTime);
            base.Update(gameTime);
        }

        private bool pressedJump = false;

        private bool physKeyLocked = false;

        private bool physStepKeyLocked = false;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Sprite bg = gameEngine.GetBackground(GAME_WIDTH, GAME_HEIGHT);
            IList<Sprite> tiles = gameEngine.GetView(blocksOnScreen.X, blocksOnScreen.Y, BLOCK_WIDTH, BLOCK_HEIGHT);
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
            if (DebugMode)
            {
                DrawDebugInfo();
            }
            if (gameEngine.ManualPhysStepMode)
            {
                DrawManualPhysInfo();
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawDebugInfo()
        {
            PlayerCharacter pc = gameEngine.Player;
            Vector2 s = pc.PhysPosition;
            Vector2 v = pc.Velocity;
            Vector2 a = pc.Acceleration;
            Vector2 f = pc.Force;
            string onGround = pc.IsOnGround ? "T" : "F";
            string debug = "P:({0}, {1})  V:({2}, {3})  A:({4}, {5})  F:({6}, {7})  M:{8}  G:{9}";
            string output = string.Format(debug, s.X, s.Y, v.X, v.Y, a.X, a.Y, f.X, f.Y, pc.Mass, onGround);
            spriteBatch.DrawString(font, output, new Vector2(10, 10), Color.Red);
        }

        private void DrawManualPhysInfo()
        {
            string debug = "Hit F8 to step physics or F5 to turn on auto step";
            spriteBatch.DrawString(font, debug, new Vector2(10, 25), Color.Red);
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
