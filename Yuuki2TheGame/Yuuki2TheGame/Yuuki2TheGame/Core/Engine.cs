using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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

        public bool InHelpScreen { get; set; }

        private MineState mine = new MineState();

        internal static SoundEngine AudioEngine;

        public bool RecordPhysStep
        {
            get
            {
                return World.RecordPhysStep;
            }
            set
            {
                World.RecordPhysStep = value;
            }
        }

        public bool ManualPhysStepMode
        {
            get
            {
                return World.ManualPhysStepMode;
            }
            set
            {
                World.ManualPhysStepMode = value;
            }
        }

        private World World { get; set; }

        private Point Spawn { get; set; }

        public Camera Camera { get; set; }

        public PlayerCharacter Player { get; private set; }

        public Engine(Point size)
        {
            InInventoryScreen = false;
            InDebugMode = false;
            CreateWorld(size);
            Spawn = new Point(50, (size.Y / 2) * Game1.METER_LENGTH - 50);
            // temp vars until we can meet with the team
            Player = new PlayerCharacter("Becky", Spawn, 100, 10, 10);
            World.AddCharacter(Player);
            Rectangle camLimits = new Rectangle(0, 0, Game1.METER_LENGTH * Game1.WORLD_WIDTH, Game1.METER_LENGTH * Game1.WORLD_HEIGHT);
            Rectangle camBox = new Rectangle(Game1.GAME_WIDTH / 4, Game1.GAME_HEIGHT / 4, Game1.GAME_WIDTH / 2, Game1.GAME_HEIGHT / 2);
            Camera = new Camera(new Point(Game1.GAME_WIDTH, Game1.GAME_HEIGHT), Player, camBox, camLimits);
            AudioEngine = new SoundEngine(Game1.GameAudio);
        }

        public void ChangeClothes(int amount)
        {
            Player.CostumeBase += amount;
            if (Player.CostumeBase > 2)
            {
                Player.CostumeBase = -1;
            }
            else if (Player.CostumeBase < -1)
            {
                Player.CostumeBase = 2;
            }
        }
        public void ChangeSkin(int amount)
        {
            Player.SpriteBase += amount;
            if (Player.SpriteBase > 3)
            {
                Player.SpriteBase = 0;
            }
            else if (Player.SpriteBase < 0)
            {
                Player.SpriteBase = 3;
            }
        }

        IList<BodyPart> parts = new List<BodyPart>();

        public void Dismember()
        {
            if (Player.Active)
            {
                AudioEngine.PlaySquish();
                foreach (BodyPart part in parts)
                {
                    World.RemoveEntity(part);
                }
                parts.Clear();
                Player.Active = false;
                parts.Add(new BodyPart(WorldPainter.CHAR_ROW_HEAD, Player.SpriteBase));
                parts.Add(new BodyPart(WorldPainter.CHAR_ROW_TORSO, Player.SpriteBase));
                parts.Add(new BodyPart(WorldPainter.CHAR_ROW_PELVIS, Player.SpriteBase));
                parts.Add(new BodyPart(WorldPainter.CHAR_ROW_LEG_FRONT_START + Player.LegAnimationFrame, Player.SpriteBase));
                parts.Add(new BodyPart(WorldPainter.CHAR_ROW_LEG_BACK_START + Player.LegAnimationFrame + WorldPainter.CHAR_ANIM_OFFSET_LEG, Player.SpriteBase));
                parts.Add(new BodyPart(WorldPainter.CHAR_ROW_FOOT_FRONT_START + Player.LegAnimationFrame, Player.SpriteBase));
                parts.Add(new BodyPart(WorldPainter.CHAR_ROW_FOOT_BACK_START + Player.LegAnimationFrame + WorldPainter.CHAR_ANIM_OFFSET_LEG, Player.SpriteBase));
                parts.Add(new BodyPart(WorldPainter.CHAR_ROW_ARM_FRONT_START + Player.ArmAnimationFrame, Player.SpriteBase));
                parts.Add(new BodyPart(WorldPainter.CHAR_ROW_ARM_BACK_START + Player.ArmAnimationFrame + WorldPainter.CHAR_ANIM_OFFSET_ARM, Player.SpriteBase));
                parts.Add(new BodyPart(WorldPainter.CHAR_ROW_HAND_FRONT_START + Player.ArmAnimationFrame, Player.SpriteBase));
                parts.Add(new BodyPart(WorldPainter.CHAR_ROW_HAND_BACK_START + Player.ArmAnimationFrame + WorldPainter.CHAR_ANIM_OFFSET_ARM, Player.SpriteBase));
                parts.Add(new BodyPart(WorldPainter.CHAR_ROW_EYES, Player.SpriteBase));
                if (Player.CostumeBase != -1)
                {
                    parts.Add(new BodyPart(WorldPainter.CHAR_ROW_HEAD, WorldPainter.CHAR_COL_BASE_END + 1 + Player.CostumeBase));
                    parts.Add(new BodyPart(WorldPainter.CHAR_ROW_TORSO, WorldPainter.CHAR_COL_BASE_END + 1 + Player.CostumeBase));
                    parts.Add(new BodyPart(WorldPainter.CHAR_ROW_PELVIS, WorldPainter.CHAR_COL_BASE_END + 1 + Player.CostumeBase));
                    parts.Add(new BodyPart(WorldPainter.CHAR_ROW_LEG_FRONT_START + Player.LegAnimationFrame, WorldPainter.CHAR_COL_BASE_END + 1 + Player.CostumeBase));
                    parts.Add(new BodyPart(WorldPainter.CHAR_ROW_LEG_BACK_START + Player.LegAnimationFrame + WorldPainter.CHAR_ANIM_OFFSET_LEG, WorldPainter.CHAR_COL_BASE_END + 1 + Player.CostumeBase));
                    parts.Add(new BodyPart(WorldPainter.CHAR_ROW_FOOT_FRONT_START + Player.LegAnimationFrame, WorldPainter.CHAR_COL_BASE_END + 1 + Player.CostumeBase));
                    parts.Add(new BodyPart(WorldPainter.CHAR_ROW_FOOT_BACK_START + Player.LegAnimationFrame + WorldPainter.CHAR_ANIM_OFFSET_LEG, WorldPainter.CHAR_COL_BASE_END + 1 + Player.CostumeBase));
                    parts.Add(new BodyPart(WorldPainter.CHAR_ROW_ARM_FRONT_START + Player.ArmAnimationFrame, WorldPainter.CHAR_COL_BASE_END + 1 + Player.CostumeBase));
                    parts.Add(new BodyPart(WorldPainter.CHAR_ROW_ARM_BACK_START + Player.ArmAnimationFrame + WorldPainter.CHAR_ANIM_OFFSET_ARM, WorldPainter.CHAR_COL_BASE_END + 1 + Player.CostumeBase));
                    parts.Add(new BodyPart(WorldPainter.CHAR_ROW_HAND_FRONT_START + Player.ArmAnimationFrame, WorldPainter.CHAR_COL_BASE_END + 1 + Player.CostumeBase));
                    parts.Add(new BodyPart(WorldPainter.CHAR_ROW_HAND_BACK_START + Player.ArmAnimationFrame + WorldPainter.CHAR_ANIM_OFFSET_ARM, WorldPainter.CHAR_COL_BASE_END + 1 + Player.CostumeBase));
                }
                Random rng = new Random();
                foreach (BodyPart part in parts)
                {
                    World.AddEntity(part);
                    Vector2 r = new Vector2();
                    r.X = rng.Next(-10, 10) * 1000;
                    r.Y = rng.Next(-10, 10) * 1000;
                    part.Mass = 10;
                    part.Position = Player.Position;
                    part.Velocity = Player.Velocity;
                    part.ApplyImpulse(r);
                }
            }
        }

        private void CreateWorld(Point size)
        {
            World.PhysicalConstants phys = new World.PhysicalConstants();
            phys.Wind = PHYS_WIND;
            phys.Gravity = PHYS_GRAVITY;
            phys.MediumDensity = PHYS_MEDIUM_DENSITY;
            phys.SurfaceFriction = PHYS_SURFACE_FRICTION;
            phys.Timescale = PHYS_TIMESCALE;
            World = new World(size.X, size.Y, phys);
        }

        public IList<InventorySlot> GetQuickSlots()
        {
            return Player.Inventory.QuickSlots;
        }

        public void Update(GameTime gameTime)
        {
            World.Update(gameTime);
            if (mine.Mining && (gameTime.TotalGameTime - mine.TimeLastMined).Milliseconds > MINE_TIME_DELAY)
            {
                mine.TimeLastMined = gameTime.TotalGameTime;
                MouseState mse = Mouse.GetState();
                MineBlock(mse.X, mse.Y);
                Point p = new Point(mse.X, mse.Y);
            }
        }

        public void Respawn()
        {
            AudioEngine.PlayTele();
            Player.Teleport(Spawn);
            Player.Active = true;
        }

        public void StepPhysics()
        {
            World.StepPhysics();
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
            Block b = World.BlockAt(coords);
            if (b != mine.Block && mine.Block != null && mine.Block.Health > 0)
            {
                mine.Block.Restore();
            }
            mine.Block = b;
            InventorySlot activeSlot = Player.Inventory.ActiveSlot;
            Item toUse = ((activeSlot.Item != null) ? activeSlot.Item : new Item(ItemID.Hands));
            int used = toUse.Use(pos, coords, World, Player);
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
                mine.Block = World.BlockAt(coords);
                mine.TimeLastMined = new TimeSpan(0, 0, 0);
            }
            else
            {
                int used = toUse.Use(pos, coords, World, Player);
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
            IList<Block> blocks = World.QueryPixels(Camera.Bounds);
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

        public IList<Sprite> GetEntities()
        {
            // TODO: OPTIMIZE! We should be using quadtrees or something...
            Rectangle view = Camera.Bounds;
            return World.GetEntities(view);
        }

        public IList<Sprite> GetCharacters()
        {
            Rectangle view = Camera.Bounds;
            return World.GetCharacters(view);
        }
    }
}
