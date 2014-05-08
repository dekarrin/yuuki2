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
using Yuuki2TheGame.Data;
using FileHelpers;

namespace Yuuki2TheGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static List<SoundEffect> GameAudio = new List<SoundEffect>();
        public static List<GameDataObject> ObjectData = new List<GameDataObject>();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteManager spriteManager;
        Engine gameEngine;
        Texture2D defaultTexture;

        public const int WORLD_WIDTH = 100;
        public const int WORLD_HEIGHT = 100;

        public const int GAME_WIDTH = 800;
        public const int GAME_HEIGHT = 600;

        public const int METER_LENGTH = 16;

        public const int INVENTORY_ITEMS = 32;

        public const int QUICK_SLOTS = 8;

        private GraphicsDeviceManager graphics;

        private SpriteBatch spriteBatch;

        private Engine gameEngine;

        private Texture2D defaultTexture;

        private ControlManager controls = new ControlManager();

        private SpriteFont font;

        private IList<Painter> painters;

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
        Texture2D mainMenuTexture;
        Texture2D logo;

        Button newgame;
        Button loadgame;
        Button options;
        Button exit;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GAME_WIDTH;
            graphics.PreferredBackBufferHeight = GAME_HEIGHT;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";


            SoundEffect bgm = Content.Load<SoundEffect>("Backround");
            SoundEffect swing01 = Content.Load<SoundEffect>("swing01");
            GameAudio.Add(bgm);
            GameAudio.Add(swing01);



            FileHelperEngine engine = new FileHelperEngine(typeof(GameDataObject));

            GameDataObject[] res = (GameDataObject[])engine.ReadFile("Data\\BlockData.csv");

            foreach (GameDataObject record in res)
            {
                ObjectData.Add(record);
            }

            blocksOnScreen = new Point (GAME_WIDTH / METER_LENGTH + 1, GAME_HEIGHT / METER_LENGTH + 1);
            painters = new List<Painter>();
            gameEngine = new Engine(new Point(WORLD_WIDTH, WORLD_HEIGHT));

            IsMouseVisible = true;
            CreatePainters();
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
            InitializePainters();
            BindControls();
            SetWindowSize();
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
            logo = Content.Load<Texture2D>(@"Menupics/logo");
            mainMenuTexture = Content.Load<Texture2D>(@"Menupics/background");
            defaultTexture = Content.Load<Texture2D>(@"Tiles/default_tile");
            IsMouseVisible = true;
            newgame = new Button(Content.Load<Texture2D>(@"Menupics/newGame"), graphics.GraphicsDevice);
            newgame.setPosition(new Vector2(260, 215));
            loadgame = new Button(Content.Load<Texture2D>(@"Menupics/loadGame"), graphics.GraphicsDevice);
            loadgame.setPosition(new Vector2(260, 280));
            options = new Button(Content.Load<Texture2D>(@"Menupics/options"), graphics.GraphicsDevice);
            options.setPosition(new Vector2(259, 345));
            exit = new Button(Content.Load<Texture2D>(@"Menupics/exitGame"), graphics.GraphicsDevice);
            exit.setPosition(new Vector2(260, 411));
            defaultTexture = Content.Load<Texture2D>(@"Tiles\default_tile");
            font = Content.Load<SpriteFont>(@"Fonts\Default");
            SetPainterDefaults(defaultTexture, font);
            LoadPainterContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            UnloadPainterContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            switch (gamestate)
            {
                case GameState.Menu:
                    {
                        if (newgame.isclicked == true)
                            gamestate = GameState.InGame;
                        newgame.Update(mouseState);
                        if (loadgame.isclicked == true)

                            gamestate = GameState.LoadGame;
                        loadgame.Update(mouseState);

                        if (options.isclicked == true)
                            gamestate = GameState.Options;
                        if (keyState.IsKeyDown(Keys.Back))
                        {
                            gamestate = GameState.Menu;
                        }
                        options.Update(mouseState);
                        if (exit.isclicked == true)
                            gamestate = GameState.Exit;
                        exit.Update(mouseState);
                    }
                    break;

                case GameState.LoadGame:
                    if (keyState.IsKeyDown(Keys.Back))
                    {
                        gamestate = GameState.Menu;
                    }
                    break;

                case GameState.Options:
                    if (keyState.IsKeyDown(Keys.Back))
                    {
                        gamestate = GameState.Menu;
                    }
                    break;

                case GameState.Exit:
                    {

                    }
                    break;
            }

            if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left))
            {
                controls.Update(gameTime);
            }
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
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            foreach (Painter p in painters)
            {
                p.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void BindControls()
        {
            controls.BindMouseButtonClick(MouseButton.LEFT, gameEngine.Click);
            controls.BindMouseButtonDown(MouseButton.LEFT, gameEngine.Press, false);
            controls.BindMouseButtonUp(MouseButton.LEFT, gameEngine.Release);
            KeyAction jump = delegate(KeyEventArgs e)
            {
                gameEngine.Player.Jump();
            };
            KeyAction startRight = delegate(KeyEventArgs e)
            {
                gameEngine.Player.StartMovingRight();
            };
            KeyAction startLeft = delegate(KeyEventArgs e)
            {
                gameEngine.Player.StartMovingLeft();
            };
            KeyAction stopRight = delegate(KeyEventArgs e)
            {
                gameEngine.Player.StopMovingRight();
            };
            KeyAction stopLeft = delegate(KeyEventArgs e)
            {
                gameEngine.Player.StopMovingLeft();
            };
            controls.BindKeyDown(Keys.Escape, delegate(KeyEventArgs e)
            {
                gameEngine.InInventoryScreen = !gameEngine.InInventoryScreen;
            }, false);
            controls.BindKeyDown(Keys.Left, startLeft, false);
            controls.BindKeyDown(Keys.A, startLeft, false);
            controls.BindKeyUp(Keys.Left, stopLeft);
            controls.BindKeyUp(Keys.A, stopLeft);
            controls.BindKeyDown(Keys.Right, startRight, false);
            controls.BindKeyDown(Keys.D, startRight, false);
            controls.BindKeyUp(Keys.Right, stopRight);
            controls.BindKeyUp(Keys.D, stopRight);
            controls.BindKeyDown(Keys.W, jump, true);
            controls.BindKeyDown(Keys.Space, jump, true);
            controls.BindKeyDown(Keys.Up, jump, true);
            controls.BindKeyDown(Keys.F3, delegate(KeyEventArgs e)
            {
                gameEngine.GiveItem(ItemID.BlockDirt, 1);
            }, true);
            controls.BindKeyDown(Keys.F4, delegate(KeyEventArgs e)
            {
                gameEngine.InDebugMode = !gameEngine.InDebugMode;
            }, false);
            controls.BindKeyDown(Keys.F5, delegate(KeyEventArgs e)
            {
                gameEngine.ManualPhysStepMode = !gameEngine.ManualPhysStepMode;
            }, false);
            controls.BindKeyDown(Keys.F6, delegate(KeyEventArgs e)
            {
                gameEngine.RecordPhysStep = !gameEngine.RecordPhysStep;
            }, false);
            controls.BindKeyDown(Keys.F7, delegate(KeyEventArgs e)
                {
                gameEngine.Respawn();
            }, false);
            controls.BindKeyDown(Keys.F8, delegate(KeyEventArgs e)
                    {
                if (gameEngine.ManualPhysStepMode) {
                    gameEngine.StepPhysics();
                    }
            }, false);
            controls.BindMouseScrollUp(delegate(MouseScrollEventArgs e)
            {
                gameEngine.Player.Inventory.ActiveSlotNumber--;
            });
            controls.BindMouseScrollDown(delegate(MouseScrollEventArgs e)
            {
                gameEngine.Player.Inventory.ActiveSlotNumber++;
            });
            controls.BindKeyDown(Keys.D1, delegate(KeyEventArgs e)
            {
                gameEngine.Player.Inventory.ActiveSlotNumber = 0;
            }, false);
            controls.BindKeyDown(Keys.D2, delegate(KeyEventArgs e)
            {
                gameEngine.Player.Inventory.ActiveSlotNumber = 1;
            }, false);
            controls.BindKeyDown(Keys.D3, delegate(KeyEventArgs e)
            {
                gameEngine.Player.Inventory.ActiveSlotNumber = 2;
            }, false);
            controls.BindKeyDown(Keys.D4, delegate(KeyEventArgs e)
            {
                gameEngine.Player.Inventory.ActiveSlotNumber = 3;
            }, false);
            controls.BindKeyDown(Keys.D5, delegate(KeyEventArgs e)
            {
                gameEngine.Player.Inventory.ActiveSlotNumber = 4;
            }, false);
            controls.BindKeyDown(Keys.D6, delegate(KeyEventArgs e)
            {
                gameEngine.Player.Inventory.ActiveSlotNumber = 5;
            }, false);
            controls.BindKeyDown(Keys.D7, delegate(KeyEventArgs e)
                    {
                gameEngine.Player.Inventory.ActiveSlotNumber = 6;
            }, false);
            controls.BindKeyDown(Keys.D8, delegate(KeyEventArgs e)
            {
                gameEngine.Player.Inventory.ActiveSlotNumber = 7;
            }, false);
            }

        private void CreatePainters()
        {
            // First in this list means first to paint.
            painters.Add(BackgroundPainter.GetInstance(gameEngine, GAME_WIDTH, GAME_HEIGHT));
            painters.Add(WorldPainter.GetInstance(gameEngine));
            painters.Add(HudPainter.GetInstance(gameEngine, GAME_WIDTH, GAME_HEIGHT));
            painters.Add(DebugPainter.GetInstance(gameEngine));
            painters.Add(CursorPainter.GetInstance(gameEngine));
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
        private void LoadPainterContent()
        {
            foreach (Painter p in painters)
        {
            Sprite bg = gameEngine.GetBackground(GAME_WIDTH, GAME_HEIGHT);
            IList<Sprite> tiles = gameEngine.GetView(blocksOnScreen.X, blocksOnScreen.Y, BLOCK_WIDTH, BLOCK_HEIGHT);
            IList<Sprite> chars = gameEngine.GetCharacters(GAME_WIDTH, GAME_HEIGHT);
            bg.Texture = NameToTexture(bg.TextureID);
            ProcessSpriteGraphics(tiles);
            ProcessSpriteGraphics(chars);
            GraphicsDevice.Clear(Color.WhiteSmoke);

            switch (gamestate)
            {
                case GameState.Menu:
                    {
                        spriteBatch.Begin();
                        IsMouseVisible = true;
                        spriteBatch.Draw(mainMenuTexture, new Rectangle(0, 0, GAME_WIDTH, GAME_HEIGHT), Color.White);
                        newgame.Draw(spriteBatch);
                        loadgame.Draw(spriteBatch);
                        options.Draw(spriteBatch);
                        exit.Draw(spriteBatch);
                p.LoadContent(Content);
            }
        }

                        spriteBatch.Draw(logo, new Rectangle(290, 80, 200, 100), null,
                    Color.White, 0, Vector2.Zero,
                    SpriteEffects.None, 1);
                        spriteBatch.End();
                    }
                    break;
                    case GameState.InGame:
                    {
                        spriteBatch.Begin();
                        // draw bg first ALWAYS!
                        spriteBatch.Draw(bg.Texture, bg.Destination, bg.Source, Color.Pink);
                        foreach (Sprite sp in tiles)
        private void UnloadPainterContent()
            {
            foreach (Painter p in painters)
            {
                p.UnloadContent(Content);
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
                        IsMouseVisible = true;
                        string loadGame = "Select your saved game";
                        spriteBatch.Begin();
                        spriteBatch.Draw(mainMenuTexture, new Rectangle(0, 0, GAME_WIDTH, GAME_HEIGHT), Color.White);
                        newgame.Draw(spriteBatch);
                        spriteBatch.DrawString(spriteManager.font, loadGame, new Vector2(300, 100), Color.Red);
                        spriteBatch.End();
                    }
                    break;

                case GameState.Options:
                    {
                        IsMouseVisible = true;
                        string options = "Under construction. :(";
                        spriteBatch.Begin();
                        spriteBatch.DrawString(spriteManager.font, options, new Vector2(200, 100), Color.Black);
                        spriteBatch.End();
                    }
                    break;

                case GameState.Exit:
                    Exit();
                    break;

        }
            base.Draw(gameTime);
        }

        private void InitializePainters()
        {
            foreach (Painter p in painters)
            {
                p.Initialize();
            }
            }

        private void SetPainterDefaults(Texture2D defTexture, SpriteFont defFont)
        {
            Painter.DefaultFont = defFont;
            Painter.DefaultTexture = defTexture;
            Painter.GraphicsDevice = GraphicsDevice;
        }

        private void SetWindowSize()
        {
            graphics.PreferredBackBufferWidth = GAME_WIDTH;
            graphics.PreferredBackBufferHeight = GAME_HEIGHT;
            graphics.ApplyChanges();
        }
    }
}
