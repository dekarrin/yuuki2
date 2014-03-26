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
            Control pressKey = new Control();
            Inventory ic = new Inventory();
            KeyboardState KeyState = new KeyboardState();
            MouseState mouseState = Mouse.GetState();

            // Press A or left arrow to move sprite left.
            if (KeyState.IsKeyDown(Keys.A) || KeyState.IsKeyDown(Keys.Left))
            {
                pressKey.moveLeft(gameTime);
            }

            // Press D or Right arrow to move sprite right.
            if (KeyState.IsKeyDown(Keys.D) || KeyState.IsKeyDown(Keys.Right))
            {
                pressKey.moveRight(gameTime);
            }

            // Press S or Down arrow to move sprite down               
            if (KeyState.IsKeyDown(Keys.S) || KeyState.IsKeyDown(Keys.Down))
            {
                // do nothing.
            }

            // Press W or Space or Up arrow to move jump sprite.
            if (KeyState.IsKeyDown(Keys.W) || KeyState.IsKeyDown(Keys.Up) || KeyState.IsKeyDown(Keys.Space))
            {
                pressKey.jump();
            }

            // Press Left click to interact with currently selected item.
            if (mouseState.LeftButton == ButtonState.Pressed)
            {

            }

            // Press Right click to interact with currently selected item.
            if (mouseState.RightButton == ButtonState.Pressed)
            {

            }

            // Press Tab to open Inventory.
            if (KeyState.IsKeyDown(Keys.Tab))
            {
                
            }

            // Press Escape to open options menu.
            if (KeyState.IsKeyDown(Keys.Escape))
            {

            }

            // Press Q to Quick use slot 1, use the item in the quick use slot 1.
            if (KeyState.IsKeyDown(Keys.Q))
            {

            }

            // Press E to Quick use slot 2, use the item in the quick use slot 2.
            if (KeyState.IsKeyDown(Keys.E))
            {

            }


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
            spriteBatch.End();
            base.Draw(gameTime);
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
