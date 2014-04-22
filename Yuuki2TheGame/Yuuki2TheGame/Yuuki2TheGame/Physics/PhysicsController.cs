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
        private Vector2 globalAcceleration;

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
            this.globalAcceleration = new Vector2(wind, gravity);
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
            IList<IPhysical> toGround = GetLandingPhobs();
            IList<IPhysical> toAir = GetLaunchingPhobs();
            MoveToGrounded(toGround);
            MoveToAirborne(toAir);
        }

        public void AddPhob(IPhysical obj)
        {
            obj.GlobalAcceleration = globalAcceleration;
            obj.CheckGroundContact = delegate(Rectangle bounds)
            {
                Rectangle queryBounds = new Rectangle(bounds.X, bounds.Y + bounds.Height, bounds.Width, GROUND_EPSILON);
                return landTree.Query(queryBounds).Any();
            };
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
            obj.GlobalAcceleration = Vector2.Zero;
            obj.CheckGroundContact = null;
        }
        
        /// <summary>
        /// Returns list of previously grounded phobs that have left the ground.
        /// </summary>
        /// <returns></returns>
        private IList<IPhysical> GetLaunchingPhobs()
        {
            IList<IPhysical> toAir = new List<IPhysical>();
            foreach (IPhysical phob in grounded)
            {
                if (!phob.IsOnGround)
                {
                    toAir.Add(phob);
                }
            }
            return toAir;
        }

        /// <summary>
        /// Returns list of previously airborne phobs that have collided with the ground.
        /// </summary>
        /// <returns></returns>
        private IList<IPhysical> GetLandingPhobs()
        {
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
                    CorrectGroundCollision(phob, top.Bounds);
                    toGround.Add(phob);
                }
            }
            return toGround;
        }

        private float PixelsToMeters(int pixels)
        {
            return pixels / (float) Game1.BLOCK_WIDTH;
        }

        private int MetersToPixels(float physUnits)
        {
            return (int) Math.Round(physUnits * Game1.BLOCK_WIDTH);
        }

        private void CorrectGroundCollision(IPhysical phob, Rectangle groundBounds)
        {
            phob.PhysPosition = new Vector2(phob.PhysPosition.X, PixelsToMeters(groundBounds.Top - phob.Bounds.Height));
            phob.IsOnGround = true;
        }

        private void MoveToGrounded(IList<IPhysical> phobs)
        {
            foreach (IPhysical phob in phobs)
            {
                grounded.Add(phob);
                airborne.Remove(phob);
            }
        }

        private void MoveToAirborne(IList<IPhysical> phobs)
        {
            foreach (IPhysical phob in phobs)
            {
                airborne.Add(phob);
                grounded.Remove(phob);
            }
        }
    }
}
