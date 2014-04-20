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

        private const int GROUND_EPSILON = 1;

        private const int MAX_ITEMS_PER_QUAD_LEAF = 20;

        private const int MIN_QUAD_SIZE = 256;

        private IList<IPhysical> phobs = new List<IPhysical>();

        private IList<IPhysical> grounded = new List<IPhysical>();

        private IList<IPhysical> airborne = new List<IPhysical>();

        private IList<IQuadObject> lands = new List<IQuadObject>();

        private QuadTree<IPhysical> phobTree = new QuadTree<IPhysical>(new Point(MIN_QUAD_SIZE, MIN_QUAD_SIZE), MAX_ITEMS_PER_QUAD_LEAF, false);

        private QuadTree<IQuadObject> landTree = new QuadTree<IQuadObject>(new Point(MIN_QUAD_SIZE, MIN_QUAD_SIZE), MAX_ITEMS_PER_QUAD_LEAF, false);

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
            foreach (IPhysical phob in phobs)
            {
                phob.UpdatePhysics((time.ElapsedGameTime.Milliseconds / 1000.0f) * timescale);
            }
            // now do ground collision checking using post-collide algo
            IList<IPhysical> toGround = new List<IPhysical>();
            foreach (IPhysical phob in airborne)
            {
                List<IQuadObject> objs = landTree.Query(phob.Bounds);
                if (objs.Count() > 0)
                {
                    IQuadObject top = null;
                    foreach (IQuadObject iqo in objs)
                    {
                        if (top == null || top.Bounds.Top < iqo.Bounds.Top)
                        {
                            top = iqo;
                        }
                    }
                    // bump them out of the ground
                    phob.PhysPosition = new Vector2(phob.PhysPosition.X, top.Bounds.Top - phob.Bounds.Height);
                    phob.IsOnGround = true;
                    toGround.Add(phob);
                }
            }
            // check if any ground based is now airborne
            IList<IPhysical> toAir = new List<IPhysical>();
            foreach (IPhysical phob in grounded)
            {
                if (!phob.IsOnGround)
                {
                    toAir.Add(phob);
                }
            }
            // take ones going to ground and add them to ground list and remove from air list
            foreach (IPhysical phob in toGround)
            {
                grounded.Add(phob);
                airborne.Remove(phob);
            }
            // take ones going to air and add them to air list and remove from ground list
            foreach (IPhysical phob in toAir)
            {
                airborne.Add(phob);
                grounded.Remove(phob);
            }
        }

        public void AddPhob(IPhysical obj)
        {
            obj.GlobalForce = globalForce;
            obj.PhysicsEngine = this;
            phobs.Add(obj);
            airborne.Add(obj);
            phobTree.Insert(obj);
        }

        public void RemovePhob(IPhysical obj)
        {
            phobs.Remove(obj);
            airborne.Remove(obj);
            grounded.Remove(obj);
            phobTree.Remove(obj);
            obj.GlobalForce = Vector2.Zero;
            obj.PhysicsEngine = null;
        }

        /// <summary>
        /// Checks if the given rect lies within epsilon pixels of a ground object. The tolerance, epsilon, is defined
        /// in this class as a constant.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool BoxIsOnGround(Rectangle rect)
        {
            Rectangle queryBounds = new Rectangle(rect.X, rect.Y + rect.Height, rect.Width, GROUND_EPSILON);
            return landTree.Query(queryBounds).Any();
        }
    }
}
