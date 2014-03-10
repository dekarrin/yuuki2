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

        public override void Update(GameTime gameTime)
        {
            // apply physics to blocks
            foreach (Block b in world)
            {
                b.Update(gameTime);
            }
        }

        public Block BlockAt(Point p)
        {
            return world[p.X, p.Y];
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
