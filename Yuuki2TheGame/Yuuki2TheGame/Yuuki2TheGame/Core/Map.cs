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

        public List<List<Block>> world { get; private set; }

        public Map(int height, int width)
        {
            GenerateWorld(height, width);
        }

        public List<List<Block>> GenerateWorld(int height, int width)
        {
            Width = width;
            Height = height;
            world = new List<List<Block>>();
            for(int x = 0; x < Width; x++){ //Temporary algorithm: Iterates through all blocks on the bottom half of the map.
                List<Block> slice = new List<Block>();
                for (int y = 0; y < Height; y++)
                {
                    if (y > Height / 2)
                    {
                        slice.Add(new Block(1, x, y)); //Uses Blocks of ID = 1 for the time being.
                    }
                    else
                    {
                        slice.Add(null);
                    }
                }
                world.Add(slice);
            }
            world[3][(Height / 2)] = new Block(1, 3, (Height / 2));
            return world;
        }

        public IList<Block> QueryPixels(Rectangle rect, Physics.ContactType type)
        {
            int x1 = (int)(rect.X / (float) Game1.BLOCK_WIDTH);
            int y1 = (int)(rect.Y / (float) Game1.BLOCK_HEIGHT);
            int x2 = (int)((rect.X + rect.Width - 1) / (float)Game1.BLOCK_WIDTH);
            int y2 = (int)((rect.Y + rect.Height - 1) / (float)Game1.BLOCK_HEIGHT);
            if (rect.X < 0)
            {
                x1--;
                if (rect.X < -rect.Width) 
                {
                    x2--;
                }
            }
            if (rect.Y < 0)
            {
                y1--;
                if (rect.Y < -rect.Height)
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
                    if (x >= 0 && y >= 0 && x < world.Count && y < world[x].Count)
                    {
                        Block b = world[x][y];
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
            foreach (List<Block> slice in world)
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
            return world[p.X][p.Y];
        }
    }
}
