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

        public Game1()
        {
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

            // TODO: Add your update logic here
            gameEngine.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            IList<Tile> drawn = gameEngine.GetView(blocksOnScreen.X, blocksOnScreen.Y, BLOCK_WIDTH, BLOCK_HEIGHT);
            ProcessTileGraphics(drawn);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            foreach (Tile t in drawn)
            {
                spriteBatch.Draw(t.Texture, t.Location, Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void ProcessTileGraphics(IList<Tile> tiles)
        {
            foreach (Tile t in tiles)
            {
                //TODO: use preloaded graphics
                t.Texture = Content.Load<Texture2D>(t.TextureID != null ? t.TextureID : @"Tiles/default_tile");
            }
        }
    }
}
