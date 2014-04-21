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

        SpriteManager spriteManager;
        Engine gameEngine;
        Texture2D defaultTexture;

        public const int WORLD_WIDTH = 100;
        public const int WORLD_HEIGHT = 100;

        public const int GAME_WIDTH = 800;
        public const int GAME_HEIGHT = 600;

        public const int BLOCK_WIDTH = 16;
        public const int BLOCK_HEIGHT = 16;

        MouseState mbd;
        MouseState Prevmbd;
        public enum GameState
        {
            Menu,
            NewGame,
            InGame,
            LoadGame,
            Options,
            Exit
           
        }
        
        /// <summary>
        /// Contains the number of blocks that are on the screen.
        /// </summary>
        private Point blocksOnScreen;
        public GameState gamestate = GameState.Menu;
        List<MainMenu> menuButtons = new List<MainMenu>();
        Texture2D mainmenuTexture;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);


            menuButtons.Add(new MainMenu(new Rectangle(400, 45, 100, 14), "NewGame"));
            menuButtons.Add(new MainMenu(new Rectangle(400, 75, 100, 14), "LoadGame"));
            menuButtons.Add(new MainMenu(new Rectangle(400, 105, 100, 14), "Options"));
            menuButtons.Add(new MainMenu(new Rectangle(400, 135, 100, 14), "Exit"));


            graphics.PreferredBackBufferWidth = GAME_WIDTH;
            graphics.PreferredBackBufferHeight = GAME_HEIGHT;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            blocksOnScreen = new Point (GAME_WIDTH / BLOCK_WIDTH + 1, GAME_HEIGHT / BLOCK_HEIGHT + 1);
            gameEngine = new Engine(new Point(WORLD_WIDTH, WORLD_HEIGHT));

            IsMouseVisible = true;
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
            spriteManager = new SpriteManager(this);
            Components.Add(spriteManager);
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
            
            mainmenuTexture = Content.Load<Texture2D>(@"Menu/Minecraft");
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

           // if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
              //  this.Exit();
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
           switch (gamestate)
            {
               case GameState.NewGame:
                   {
                        Initialize();  //resets the sprites
                        gamestate = GameState.InGame;
                        IsMouseVisible = false;
                        //spriteManager.Enabled = true;
                        //spriteManager.Visible = true;
                        menuButtons.RemoveRange(0, menuButtons.Count);
                   }
                   break;
               case GameState.InGame:
                       {

                       }
                       break;
               case GameState.LoadGame:
                       {

                       }
                   break;
               case GameState.Options:
                   {

                   }
                   break;
               case GameState.Exit:
                   {

                   }
                   break;

             }
           mbd = Mouse.GetState();

           foreach (MainMenu m in menuButtons)
           {
               m.mouseOver(mbd);
               if (m.text == "NewGame" && m.selected == true && mbd.LeftButton == ButtonState.Pressed && Prevmbd.LeftButton == ButtonState.Released)
               {
                   gamestate = GameState.NewGame;
               }

               else if (m.text == "LoadGame" && m.selected == true && mbd.LeftButton == ButtonState.Pressed && Prevmbd.LeftButton == ButtonState.Released)
               {
                   gamestate = GameState.LoadGame;
               }

               else if (m.text == "Options" && m.selected == true && mbd.LeftButton == ButtonState.Pressed && Prevmbd.LeftButton == ButtonState.Released)
               {
                   gamestate = GameState.Options;
               }

               else if (m.text == "Exit" && m.selected == true && mbd.LeftButton == ButtonState.Pressed && Prevmbd.LeftButton == ButtonState.Released)
               {
                   gamestate = GameState.Exit;
               }

           }

           Prevmbd = mbd;




                    if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left))
                    {
                        gameEngine.Player.MoveLeft();
                    }
                    else if (keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right))
                    {
                        gameEngine.Player.MoveRight();
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

                    if (mouseState.LeftButton == ButtonState.Pressed && !mouseLeftLocked)
                    {
                        mouseLeftLocked = true;
                        int globalx = (mouseState.X + gameEngine.Camera.Location.X) / BLOCK_WIDTH;
                        int globaly = (mouseState.Y + gameEngine.Camera.Location.Y) / BLOCK_HEIGHT;
                        Point p = new Point(globalx, globaly);

                        if (globalx <= WORLD_WIDTH && globalx >= 0 && globaly <= WORLD_HEIGHT && globaly >= 0)
                        {
                            Block block = gameEngine._map.BlockAt(p);
                            //TODO: Make gameEngine responsible with a single method!
                            if (block != null)
                            {
                                gameEngine._map.DestroyBlock(p);
                                gameEngine.RemovePhysical(block);
                            }
                            else
                            {
                                gameEngine._map.AddBlock(p);
                                block = gameEngine._map.BlockAt(p);
                                gameEngine.AddPhysical(block);
                            }
                        }
                    }
                    else if (mouseState.LeftButton == ButtonState.Released)
                    {
                        mouseLeftLocked = false;
                    }


                    // TODO: Add your update logic here
                    gameEngine.Update(gameTime);
                    base.Update(gameTime);

            }
        

        private bool mouseLeftLocked = false;
      
        private bool pressedJump = false;
       

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
         // GraphicsDevice.Clear(Color.Black);

            
            
           
           
            switch (gamestate)
            {
                case GameState.Menu:
                    {

            spriteBatch.Begin();
            IsMouseVisible = true;
           // spriteBatch.Draw(mainmenuTexture, pos1, Color.White);
            


            spriteBatch.Draw(mainmenuTexture,
                new Rectangle(0, 0, Window.ClientBounds.Width,
                    Window.ClientBounds.Height), null,
                    Color.White, 0, Vector2.Zero,
                    SpriteEffects.None, 1);


            foreach (MainMenu m in menuButtons)
            {
                m.Draw(spriteBatch, spriteManager.font);
            }
                       spriteBatch.End();
                    }
                    break;
                case GameState.InGame:
                    {
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

                        spriteBatch.End();
                    }
                    break;
                case GameState.LoadGame:
                    {

                    }
                    break;
                case GameState.Options:
                    {

                    }
                    break;
                case GameState.Exit:
                    Exit();
                    break;

            

           
        }
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
