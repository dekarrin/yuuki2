using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Yuuki2TheGame.Graphics;
using Yuuki2TheGame.Physics;

namespace Yuuki2TheGame.Core
{
    class Engine
    {
        public const float PHYS_WIND = 0f;

        public const float PHYS_GRAVITY = 9.806f;

        public const float PHYS_TIMESCALE = 1.0f;

        public const float PHYS_MEDIUM_DENSITY = 1.225f;

        public const float PHYS_SURFACE_FRICTION = 0.3f;

        private bool _recording = false;

        public bool RecordPhysStep
        {
            get
            {
                return _recording;
            }
            set
            {
                if (value)
                {
                    physics.StartRecording();
                }
                else
                {
                    physics.StopRecording();
                }
                _recording = value;
            }
        }

        public bool ManualPhysStepMode { get; set; }

        private static Map _map;

        private List<GameCharacter> _characters = new List<GameCharacter>();

        private List<Item> _items = new List<Item>();

        private bool mouseLeftLocked;

        private Point spawn;

        private PhysicsController physics;

        public Camera Camera { get; set; }

        public Engine(Point size)
        {
            ManualPhysStepMode = false;
            _map = new Map(size.X, size.Y);
            spawn = new Point(0, (size.Y / 2) * Game1.BLOCK_HEIGHT - 30);
            // temp vars until we can meet with the team
            Player = new PlayerCharacter("Becky", spawn, 100, 10, 10);
            _characters.Add(Player);
            Camera = new Camera(new Point(Game1.GAME_WIDTH, Game1.GAME_HEIGHT), Player, new Point(-100, -300));
            physics = new PhysicsController(PHYS_WIND, PHYS_GRAVITY, PHYS_MEDIUM_DENSITY, PHYS_SURFACE_FRICTION, PHYS_TIMESCALE);
            physics.AddMap(_map);
            physics.AddPhob(Player);
            //Player.ApplyImpulse(new Vector2(10000, 0));
        }

        private int TESTcycle = 0;

        private void TESTCamMove()
        {
            TESTcycle = (TESTcycle + 1) % 60;
            if (TESTcycle == 0)
            {
                Camera.TargetOffsetX += 1;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (RecordPhysStep)
            {
                _recording = physics.RecordingQueryTime;
            }
            foreach (GameCharacter c in _characters)
            {
                c.Update(gameTime);
            }
            foreach (Item i in _items)
            {
                i.Update(gameTime);
            }
            _map.Update(gameTime);
            if (!ManualPhysStepMode)
            {
                physics.Update(gameTime);
            }
        }

        public void Respawn()
        {
            Player.Teleport(spawn);
        }

        public void StepPhysics()
        {
            physics.Step(0.016f);
        }

        private void UpdateBlockInput(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && !mouseLeftLocked)
            {
                mouseLeftLocked = true;
                int globalx = (mouseState.X + this.Camera.Location.X) / Game1.BLOCK_WIDTH;
                int globaly = (mouseState.Y + this.Camera.Location.Y) / Game1.BLOCK_HEIGHT;
                Point p = new Point(globalx, globaly);

                if (globalx <= Game1.WORLD_WIDTH && globalx >= 0 && globaly <= Game1.WORLD_HEIGHT && globaly >= 0)
                {
                    Block block = _map.BlockAt(p);
                    //TODO: Make this.responsible with a single method!
                    if (block != null)
                    {
                        _map.DestroyBlock(p);
                        this.RemovePhysical(block);
                    }
                    else
                    {
                        _map.AddBlock(p);
                        block = _map.BlockAt(p);
                        this.AddPhysical(block);
                    }
                }
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                mouseLeftLocked = false;
            }
        }

        public void RemovePhysical(IPhysical phob)
        {
            physics.RemovePhob(phob);
        }

        public void AddPhysical(IPhysical phob)
        {
            physics.AddPhob(phob);
        }

        /// <summary>
        /// Gets all tiles that need to be displayed.
        /// </summary>
        /// <param name="numX">Number of tiles wide.</param>
        /// <param name="numY">Number of tiles high.</param>
        /// <param name="tileWidth">Width of a tile.</param>
        /// <param name="tileHeight">Height of a tile.</param>
        /// <returns></returns>
        public IList<Sprite> GetView(int numX, int numY, int tileWidth, int tileHeight)
        {
            Vector2 pos = Camera.BlockPosition;
            Point coords = new Point((int) pos.X, (int) pos.Y);
            Point offsets = Camera.BlockOffsets;
            IList<Sprite> view = new List<Sprite>();
            for (int i = 0; i < numX && coords.X + i < _map.Width; i++)
            {
                for (int j = 0; j < numY && coords.Y + j < _map.Height; j++)
                {
                    Point tilecoords = new Point(coords.X + i, coords.Y + j);
                    if (tilecoords.X >= 0 && tilecoords.Y >= 0 && _map.BlockAt(tilecoords) != null)
                    {
                        Point position = new Point(i * tileWidth - offsets.X, j * tileHeight - offsets.Y);
                        Sprite spr = new Sprite(position, new Point(tileWidth, tileHeight), _map.BlockAt(tilecoords).Texture);
                        view.Add(spr);
                    }
                }
            }
            return view;
        }

        public PlayerCharacter Player { get; private set; }

        public IList<GameCharacter> Characters
        {
            get
            {
                return _characters.AsReadOnly();
            }
        }

        public IList<Item> Items
        {
            get
            {
                return _items.AsReadOnly();
            }
        }

        public Sprite GetBackground(int screenWidth, int screenHeight)
        {
            int x = Math.Abs(Math.Min(Camera.Position.X, 0));
            int y = Math.Abs(Math.Min(Camera.Position.Y, 0));
            int width = screenWidth - x;
            int height = screenHeight - y;
            // Above will need to be changed if we want to make worlds that are smaller than the screen
            Sprite spr = new Sprite(new Point(x, y), new Point(width, height), null);
            // TODO: Correct position based on camera
            return spr;
        }

        public IList<Sprite> GetCharacters(int screenWidth, int screenHeight)
        {
            // TODO: OPTIMIZE! We should be using quadtrees or something...
            Rectangle view = Camera.Bounds;
            IList<Sprite> chars = new List<Sprite>();
            foreach (GameCharacter c in _characters)
            {
                if (c.Bounds.Intersects(view))
                {
                    Point position = new Point(c.Bounds.X - Camera.Position.X, c.Bounds.Y - Camera.Position.Y);
                    Point size = new Point(c.Width, c.Height);
                    Sprite spr = new Sprite(position, size, c.Texture);
                    chars.Add(spr);
                }
            }
            return chars;
        }

        /// <summary>
        /// Determines whether the provided bounding box is touching the ground.
        /// </summary>
        /// <param name="boundingBox">The bounding box to check.</param>
        /// <returns>A Boolean value that indicates whether the specified bounding box is touching the ground.</returns>
        public static bool ObjectIsOnGround(Rectangle boundingBox)
         {
            for (int x = boundingBox.Left; x <= boundingBox.Right; x++)
            {
                Block block = _map.BlockAtCoordinate(x, boundingBox.Bottom);
                if (block != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
