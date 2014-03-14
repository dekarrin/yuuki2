using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    class Map : IUpdateable
    {
        public Block[,] world { get; private set; }

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

        public Block[,] GenerateWorld(int height, int width)
        {
            world = new Block[height, width];
            for(int i = 0; i < height/2; i++){ //Temporary algorithm: Iterates through all blocks on the bottom half of the map.
                for (int j = 0; j < width; j++)
                {
                    world[i, j] = new Block(1); //Uses Blocks of ID = 1 for the time being.
                }
            }
            return world;
        }

        public void Update(GameTime gameTime)
        {
            // apply physics to blocks
            foreach (Block b in world)
            {
                if (b != null) {
                    b.Update(gameTime);
                }
            }
        }

        public Block BlockAt(Point p)
        {
            return world[p.Y, p.X];
        }

        public Block[,] GetView()
        {
            Block[,] worldCopy = new Block[world.GetLength(0), world.GetLength(1)];

            for (int i = 0; i < world.GetLength(0); ++i)
                Array.Copy(world, i * world.GetLength(1), worldCopy, i * worldCopy.GetLength(1), world.GetLength(1));

            return worldCopy;
        }
    }
}
