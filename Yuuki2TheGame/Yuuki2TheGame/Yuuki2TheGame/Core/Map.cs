using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    /// <summary>
    /// Please note that block array coordinates are x, y, 0 is upper left.
    /// </summary>
    class Map
    {
        public int Height { get; private set; }

        public int Width { get; private set; }

        public IList<IList<Block>> World { get; private set; }

        public Map(int width, int height)
        {
            World = GenerateWorld(width, height);
        }

        public IList<IList<Block>> GenerateWorld(int width, int height)
        {
            Width = width;
            Height = height;
            IList<IList<Block>> world = new List<IList<Block>>();
            for(int x = 0; x < Width; x++){ //Temporary algorithm: Iterates through all blocks on the bottom half of the map.
                IList<Block> slice = new List<Block>();
                for (int y = 0; y < Height; y++)
                {
                    if (y > Height / 2)
                    {
                        int type = Math.Min(x / (Width / 3), 2);
                        slice.Add(new Block(type, x, y));
                    }
                    else
                    {
                        slice.Add(null);
                    }
                }
                world.Add(slice);
            }
            CheckAllDirtBlocks(world);
            return world;
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
                    if (x >= 0 && y >= 0 && x < World.Count && y < World[x].Count)
                    {
                        Block b = World[x][y];
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
            // apply physics to blocks
            foreach (List<Block> slice in World)
            {
                foreach (Block b in slice)
                {
                    if (b != null)
                    {
                        b.Update(gameTime);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the Block at the given coordinates relative to map origin (lower left).
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Block BlockAt(Point p)
        {
            return World[p.X][p.Y];
        }

        public void DestroyBlock(Point p)
        {
            Block b = World[p.X][p.Y];
            World[p.X][p.Y] = null;
            for (int i = p.Y + 1; i < Height; i++)
            {
                if (CheckDirtGrassSwitch(World, p.X, i))
                {
                    World[p.X][i] = new Block(BlockID.Grass, p.X, i);
                    break;
                }
            }
        }

        public bool AddBlock(BlockID id, Point p)
        {
            if (World[p.X][p.Y] == null)
            {
                Block b = new Block(id, p.X, p.Y);
                World[p.X][p.Y] = b;
                if (CheckDirtGrassSwitch(World, p.X, p.Y))
                {
                    SetBlock(BlockID.Grass, p.X, p.Y);
                }
                if (World[p.X][p.Y + 1] != null && World[p.X][p.Y + 1].ID == BlockID.Grass)
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
            // TODO: Implement item dropping
        }

        private void SetBlock(BlockID id, int x, int y)
        {
            World[x][y] = new Block(id, x, y);
        }

        //Sets dirts to grasses if they are the top
        private void CheckAllDirtBlocks(IList<IList<Block>> world)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (CheckDirtGrassSwitch(world, x, y))
                    {
                        world[x][y] = new Block(BlockID.Grass, x, y);
                        break;
                    }
                }
            }
        }

        private bool CheckDirtGrassSwitch(IList<IList<Block>> world, int x, int y)
        {
            bool switchBlock = false;
            if (world[x][y] != null && world[x][y].ID == BlockID.Dirt)
            {
                if (y > 0)
                {
                    switchBlock = (world[x][y - 1] == null);
                }
            }
            return switchBlock;
        }
    }
}
