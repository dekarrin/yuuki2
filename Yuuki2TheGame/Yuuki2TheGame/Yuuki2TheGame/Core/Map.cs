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
    class Map : IUpdateable
    {
        public int Height { get; private set; }

        public int Width { get; private set; }

        public List<List<Block>> world { get; private set; }

        private int _updateOrder = 0;

        public int UpdateOrder
        {
            get
            {
                return _updateOrder;
            }
            set
            {
                bool diff = _updateOrder != value;
                _updateOrder = value;
                if (diff && UpdateOrderChanged != null)
                {
                    UpdateOrderChanged(this, new EventArgs());
                }
            }
        }

        private bool _enabled = true;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                bool diff = _enabled != value;
                _enabled = value;
                if (diff && EnabledChanged != null)
                {
                    EnabledChanged(this, new EventArgs());
                }
            }
        }

        public event EventHandler<EventArgs> UpdateOrderChanged = null;

        public event EventHandler<EventArgs> EnabledChanged = null;

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
                    else if (x == 0 && y == 0) {
                        slice.Add(new Block(1, x, y));
                    }
                    else
                    {
                        slice.Add(null);
                    }
                }
                world.Add(slice);
            }
            return world;
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
