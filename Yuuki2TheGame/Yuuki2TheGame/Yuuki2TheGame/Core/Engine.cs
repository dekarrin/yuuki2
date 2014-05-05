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
    struct MineState
    {
        public bool Mining;

        public Block Block;

        public TimeSpan TimeLastMined;
    }

    class Engine
    {
        public const float PHYS_WIND = 0f;

        public const float PHYS_GRAVITY = 9.806f;

        public const float PHYS_TIMESCALE = 1.0f;

        public const float PHYS_MEDIUM_DENSITY = 1.225f;

        public const float PHYS_SURFACE_FRICTION = 0.3f;

        /// <summary>
        /// This is in ms.
        /// </summary>
        public const int MINE_TIME_DELAY = 100;

        public bool InInventoryScreen { get; set; }

        public bool InDebugMode { get; set; }

        private bool _recording = false;

        private MineState mine = new MineState();

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

        private Point spawn;

        private PhysicsController physics;

        public Camera Camera { get; set; }

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

        public Engine(Point size)
        {
            InInventoryScreen = false;
            ManualPhysStepMode = false;
            InDebugMode = false;
            _map = new Map(size.X, size.Y);
            spawn = new Point(0, (size.Y / 2) * Game1.METER_LENGTH - 30);
            // temp vars until we can meet with the team
            Player = new PlayerCharacter("Becky", spawn, 100, 10, 10);
            _characters.Add(Player);
            Rectangle camLimits = new Rectangle(0, 0, Game1.METER_LENGTH * Game1.WORLD_WIDTH, Game1.METER_LENGTH * Game1.WORLD_HEIGHT);
            Camera = new Camera(new Point(Game1.GAME_WIDTH, Game1.GAME_HEIGHT), Player, new Point(-100, -300), camLimits);
            physics = new PhysicsController(PHYS_WIND, PHYS_GRAVITY, PHYS_MEDIUM_DENSITY, PHYS_SURFACE_FRICTION, PHYS_TIMESCALE);
            physics.AddMap(_map);
            physics.AddPhob(Player);
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

        public IList<InventorySlot> GetQuickSlots()
        {
            return Player.Inventory.QuickSlots;
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
            if (mine.Mining && (gameTime.TotalGameTime - mine.TimeLastMined).Milliseconds > MINE_TIME_DELAY)
            {
                mine.TimeLastMined = gameTime.TotalGameTime;
                MouseState mse = Mouse.GetState();
                MineBlock(mse.X, mse.Y);
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

        public void Click(MouseButtonEventArgs e)
        {
            if (InInventoryScreen)
            {

            }
        }

        public void Press(MouseButtonEventArgs e)
        {
            if (!InInventoryScreen)
            {
                PressWorld(e.X, e.Y);
            }
        }

        public void Release(MouseButtonEventArgs e)
        {
            if (mine.Block != null)
            {
                mine.Block.Restore();
            }
            mine.Block = null;
            mine.Mining = false;
        }

        private void MineBlock(int x, int y)
        {
            int globalx = (x + this.Camera.Position.X) / Game1.METER_LENGTH;
            int globaly = (y + this.Camera.Position.Y) / Game1.METER_LENGTH;
            Point coords = new Point(globalx, globaly);
            Point pos = new Point(x + Camera.Position.X, y + Camera.Position.Y);
            Block b = _map.BlockAt(coords);
            if (b != mine.Block && mine.Block != null && mine.Block.Health > 0)
            {
                mine.Block.Restore();
            }
            mine.Block = b;
            InventorySlot activeSlot = Player.Inventory.ActiveSlot;
            Item toUse = ((activeSlot.Item != null) ? activeSlot.Item : new Item(ItemID.Hands));
            int used = toUse.Use(pos, coords, _map, Player);
            if (toUse == activeSlot.Item)
            {
                activeSlot.Count -= used;
            }
        }

        private void PressWorld(int x, int y)
        {
            int globalx = (x + this.Camera.Position.X) / Game1.METER_LENGTH;
            int globaly = (y + this.Camera.Position.Y) / Game1.METER_LENGTH;
            Point coords = new Point(globalx, globaly);
            Point pos = new Point(x + Camera.Position.X, y + Camera.Position.Y);
            InventorySlot activeSlot = Player.Inventory.ActiveSlot;
            Item toUse = ((activeSlot.Item != null) ? activeSlot.Item : new Item(ItemID.Hands));
            if (toUse.IsTool)
            {
                mine.Mining = true;
                mine.Block = _map.BlockAt(coords);
                mine.TimeLastMined = new TimeSpan(0, 0, 0);
            }
            else
            {
                int used = toUse.Use(pos, coords, _map, Player);
                if (toUse == activeSlot.Item)
                {
                    activeSlot.Count -= used;
                }
            }
        }

        /// <summary>
        /// Gets all tiles that need to be displayed. All Sprites returned have the original block that they
        /// came from set as their Creator property.
        /// </summary>
        /// <returns></returns>
        public IList<Sprite> GetView()
        {
            Vector2 pos = Camera.BlockPosition;
            Point coords = new Point((int) pos.X, (int) pos.Y);
            Point offsets = Camera.BlockOffsets;
            IList<Sprite> view = new List<Sprite>();
            IList<Block> blocks = _map.QueryPixels(Camera.Bounds);
            foreach (Block b in blocks)
            {
                Sprite spr = b.Sprite;
                spr.Position = new Point(spr.Position.X - Camera.X, spr.Position.Y - Camera.Y);
                view.Add(spr);
            }
            return view;
        }

        public void GiveItem(ItemID id, int count)
        {
            Item item = new Item(id);
            for (int i = 0; i < count; i++)
            {
                Player.Inventory.Add(item);
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

        public IList<Sprite> GetCharacters()
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
    }
}
