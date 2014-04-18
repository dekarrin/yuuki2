using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Yuuki2TheGame.Core;

namespace Yuuki2TheGame.Physics
{
    class PhysicsController
    {
        private Vector2 globalForce;

        private const int MAX_ITEMS_PER_QUAD_LEAF = 20;

        private const int MIN_QUAD_SIZE = 256;

        private IList<IPhysical> phobs = new List<IPhysical>();

        private IList<IQuadObject> lands = new List<IQuadObject>();

        private QuadTree<IPhysical> phobTree = new QuadTree<IPhysical>(new Point(MIN_QUAD_SIZE, MIN_QUAD_SIZE), MAX_ITEMS_PER_QUAD_LEAF, false);

        private QuadTree<IQuadObject> landTree = new QuadTree<IQuadObject>(new Point(MIN_QUAD_SIZE, MIN_QUAD_SIZE), MAX_ITEMS_PER_QUAD_LEAF, false);

        public const double CONVERSION_FACTOR = 1 / Game1.BLOCK_WIDTH;

        private float timescale;

        public PhysicsController(float wind, float gravity, float timescale)
        {
            this.timescale = timescale;
            this.globalForce = new Vector2(wind, gravity);
        }

        public void AddMap(Yuuki2TheGame.Core.Map map)
        {
            Point coords = new Point();
            Block block = null;
            for (coords.X = 0; coords.X < map.Width; coords.X++)
            {
                for (coords.Y = 0; coords.Y < map.Height; coords.Y++)
                {
                    block = map.BlockAt(coords);
                    if (block != null)
                    {
                        AddLand(block);
                    }
                }
            }
        }

        public void AddLand(IQuadObject land)
        {
            lands.Add(land);
            landTree.Insert(land);
        }

        public void Update(GameTime time)
        {
        }

        public void AddPhob(IPhysical obj)
        {
            phobs.Add(obj);
            phobTree.Insert(obj);
        }

        public void RemovePhob(IPhysical obj)
        {
            phobs.Remove(obj);
        }
    }
}
