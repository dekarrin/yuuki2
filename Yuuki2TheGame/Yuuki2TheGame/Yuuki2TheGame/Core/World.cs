using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuuki2TheGame.Physics;

namespace Yuuki2TheGame.Core
{
    /// <summary>
    /// Please note that block array coordinates are x, y, 0 is upper left.
    /// </summary>
    class World
    {
        public struct PhysicalConstants
        {
            public float Wind;
            public float Gravity;
            public float MediumDensity;
            public float SurfaceFriction;
            public float Timescale;
        }

        public int Height { get; private set; }

        public int Width { get; private set; }

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

        private IList<ActiveEntity> _entities = new List<ActiveEntity>();

        private IList<GameCharacter> _characters = new List<GameCharacter>();

        public IList<ActiveEntity> Entities
        {
            get
            {
                return ((List<ActiveEntity>)_entities).AsReadOnly();
            }
        }

        public IList<IList<Block>> Map { get; private set; }

        private PhysicsController physics;

        private bool _recording = false;

        public World(int width, int height, PhysicalConstants phys)
        {
            ManualPhysStepMode = false;
            Map = GenerateMap(width, height);
            physics = new PhysicsController(phys.Wind, phys.Gravity, phys.MediumDensity, phys.SurfaceFriction, phys.Timescale);
            physics.AddMap(this);
            physics.SetBoundaryUse(true);
            physics.SetWorldBoundaries(0, 0, PhysicsController.MetersToPixels(width), PhysicsController.MetersToPixels(height));
        }

        public void AddEntity(ActiveEntity ent)
        {
            _entities.Add(ent);
            physics.AddPhob(ent);
        }

        public void RemoveEntity(ActiveEntity ent)
        {
            physics.RemovePhob(ent);
            _entities.Remove(ent);
        }

        public void AddCharacter(GameCharacter ch)
        {
            _characters.Add(ch);
            physics.AddPhob(ch);
        }

        public void RemoveCharacter(GameCharacter ch)
        {
            physics.RemovePhob(ch);
            _characters.Remove(ch);
        }

        public IList<IList<Block>> GenerateMap(int width, int height)
        {
            Width = width;
            Height = height;
            IList<IList<Block>> map = new List<IList<Block>>();
            for(int x = 0; x < Width; x++){ //Temporary algorithm: Iterates through all blocks on the bottom half of the map.
                IList<Block> slice = new List<Block>();
                for (int y = 0; y < Height; y++)
                {
                    if (y > Height / 2)
                    {
                        int type = Math.Min(x / (Width / 3), 2);
                        slice.Add(CreateBlock(type, x, y));
                    }
                    else
                    {
                        slice.Add(null);
                    }
                }
                map.Add(slice);
            }
            CheckAllDirtBlocks(map);
            return map;
        }

        public IList<Block> QueryPixels(Rectangle rect)
        {
            int x1 = (int)(rect.X / (float) Game1.METER_LENGTH);
            int y1 = (int)(rect.Y / (float) Game1.METER_LENGTH);
            int x2 = (int)((rect.X + rect.Width - 1) / (float)Game1.METER_LENGTH);
            int y2 = (int)((rect.Y + rect.Height - 1) / (float)Game1.METER_LENGTH);
            if (rect.X < 0)
            {
                x1--;
                if (rect.X <= -rect.Width) 
                {
                    x2--;
                }
            }
            if (rect.Y < 0)
            {
                y1--;
                if (rect.Y <= -rect.Height)
                {
                    y2--;
                }
            }
            int w = Math.Abs(x2 - x1) + 1;
            int h = Math.Abs(y2 - y1) + 1;
            IList<Block> bl = Query(new Rectangle(x1, y1, w, h));
            return bl;
        }

        /// <summary>
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public IList<Block> Query(Rectangle rect)
        {
            IList<Block> results = new List<Block>();
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j < rect.Height; j++)
                {
                    int x = rect.X + i;
                    int y = rect.Y + j;
                    if (x >= 0 && y >= 0 && x < Map.Count && y < Map[x].Count)
                    {
                        Block b = Map[x][y];
                        if (b != null)
                        {
                            results.Add(b);
                        }
                    }
                }
            }
            return results;
        }

        public void Update(GameTime gameTime)
        {
            if (RecordPhysStep)
            {
                _recording = physics.RecordingQueryTime;
            }
            // update blocks
            foreach (List<Block> slice in Map)
            {
                foreach (Block b in slice)
                {
                    if (b != null)
                    {
                        b.Update(gameTime);
                    }
                }
            }
            // update phobs
            foreach (ActiveEntity ent in _entities)
            {
                ent.Update(gameTime);
            }
            foreach (GameCharacter gc in _characters)
            {
                gc.Update(gameTime);
            }
            if (!ManualPhysStepMode)
            {
                physics.Update(gameTime);
            }
        }

        public void StepPhysics()
        {
            physics.Step(0.016f);
        }

        public IList<Yuuki2TheGame.Graphics.Sprite> GetEntities(Rectangle view)
        {
            IList<Yuuki2TheGame.Graphics.Sprite> ents = new List<Yuuki2TheGame.Graphics.Sprite>();
            foreach (ActiveEntity e in _entities)
            {
                if (e.Bounds.Intersects(view))
                {
                    Yuuki2TheGame.Graphics.Sprite spr = e.Sprite;
                    spr.Position = new Point(spr.Position.X - view.X, spr.Position.Y - view.Y);
                    ents.Add(spr);
                }
            }
            return ents;
        }

        public IList<Yuuki2TheGame.Graphics.Sprite> GetCharacters(Rectangle view)
        {
            IList<Yuuki2TheGame.Graphics.Sprite> chs = new List<Yuuki2TheGame.Graphics.Sprite>();
            foreach (ActiveEntity c in _characters)
            {
                if (c.Bounds.Intersects(view))
                {

                    Yuuki2TheGame.Graphics.Sprite spr = c.Sprite;
                    spr.Position = new Point(spr.Position.X - view.X, spr.Position.Y - view.Y); 
                    chs.Add(spr);
                }
            }
            return chs;
        }

        /// <summary>
        /// Returns the Block at the given coordinates relative to map origin (lower left).
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Block BlockAt(Point p)
        {
            return Map[p.X][p.Y];
        }

        public void RemoveBlock(Point p)
        {
            Block b = Map[p.X][p.Y];
            Map[p.X][p.Y] = null;
            for (int i = p.Y + 1; i < Height; i++)
            {
                if (CheckDirtGrassSwitch(Map, p.X, i))
                {
                    Map[p.X][i] = CreateBlock(BlockID.Grass, p.X, i);
                    break;
                }
            }
            
        }

        public bool AddBlock(BlockID id, Point p)
        {
            if (Map[p.X][p.Y] == null)
            {
                Block b = CreateBlock(id, p.X, p.Y);
                Map[p.X][p.Y] = b;
                if (CheckDirtGrassSwitch(Map, p.X, p.Y))
                {
                    SetBlock(BlockID.Grass, p.X, p.Y);
                }
                if (Map[p.X][p.Y + 1] != null && Map[p.X][p.Y + 1].ID == BlockID.Grass)
                {
                    SetBlock(BlockID.Dirt, p.X, p.Y + 1);
                }
                
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Drops an item on to the map.
        /// </summary>
        /// <param name="item">The item being dropped.</param>
        /// <param name="position">The absolute pixel coordinates where the item should be dropped.</param>
        public void AddItem(Item item, Point position)
        {
            ItemEntity ent = new ItemEntity(item, position);
            ent.OnPicked += RemoveEntity;
            AddEntity(ent);
        }

        private void SetBlock(BlockID id, int x, int y)
        {
            Map[x][y] = CreateBlock(id, x, y);
        }

        //Sets dirts to grasses if they are the top
        private void CheckAllDirtBlocks(IList<IList<Block>> map)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (CheckDirtGrassSwitch(map, x, y))
                    {
                        map[x][y] = CreateBlock(BlockID.Grass, x, y);
                        break;
                    }
                }
            }
        }

        private bool CheckDirtGrassSwitch(IList<IList<Block>> map, int x, int y)
        {
            bool switchBlock = false;
            if (map[x][y] != null && map[x][y].ID == BlockID.Dirt)
            {
                if (y > 0)
                {
                    switchBlock = (map[x][y - 1] == null);
                }
            }
            return switchBlock;
        }

        private void HandleBlockDestruction(Block b)
        {
            Item drop = b.Item;
            Point p = new Point();
            p.X = (int)b.BlockPosition.X;
            p.Y = (int)b.BlockPosition.Y;
            RemoveBlock(p);
            AddItem(drop, b.Position);
        }

        private Block CreateBlock(int id, int x, int y)
        {
            Block b = new Block(id, x, y);
            b.OnDestroy += this.HandleBlockDestruction;
            return b;
        }

        private Block CreateBlock(BlockID id, int x, int y)
        {
            Block b = new Block(id, x, y);
            b.OnDestroy += this.HandleBlockDestruction;
            return b;
        }
    }
}
