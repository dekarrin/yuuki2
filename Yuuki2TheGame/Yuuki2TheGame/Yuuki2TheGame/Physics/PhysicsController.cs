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
        /// <summary>
        /// Contains the setters that give the engine access to private physical properties.
        /// </summary>
        private struct PhobAccessors
        {
            public PhysicsPrivateSetter<int> setContactMask;
        }

        public bool RecordingQueryTime { get; private set; }

        /// <summary>
        /// Maps Phobs to their PhobAccessors struct.
        /// </summary>
        private IDictionary<IPhysical, PhobAccessors> accessors = new Dictionary<IPhysical,PhobAccessors>();

        private Vector2 globalAcceleration;

        private const int GROUND_EPSILON = 1;

        private const int MAX_ITEMS_PER_QUAD_LEAF = 0;

        /// <summary>
        /// This should be block_width times an odd number. Idk why. Fucking stupid quad tree.
        /// </summary>
        private const int MIN_QUAD_SIZE = Game1.BLOCK_WIDTH * 3;

        private IList<IPhysical> phobs = new List<IPhysical>();

        private IList<IPhysical> grounded = new List<IPhysical>();

        private IList<IPhysical> airborne = new List<IPhysical>();

        private IList<IQuadObject> lands = new List<IQuadObject>();

        private QuadTree<IPhysical> phobTree = new QuadTree<IPhysical>(new Point(MIN_QUAD_SIZE, MIN_QUAD_SIZE), MAX_ITEMS_PER_QUAD_LEAF, false);

        private IList<long> queryTimes = new List<long>();

        private Map map;
        
        private float mediumDensity;

        private float timescale;

        private float friction;

        public PhysicsController(float wind, float gravity, float density, float friction, float timescale)
        {
            this.friction = friction;
            this.timescale = timescale;
            this.mediumDensity = density;
            this.globalAcceleration = new Vector2(wind, gravity);
        }

        public void AddMap(Map map)
        {
            this.map = map;
        }

        public void Update(GameTime time)
        {
            if (RecordingQueryTime && queryTimes.Count >= 250)
            {
                StopRecording();
            }
            Step(time.ElapsedGameTime.Milliseconds / 1000.0f);
        }

        public void Step(float secs)
        {
            foreach (IPhysical phob in phobs)
            {
                phob.UpdatePhysics(secs * timescale);
            }
            IList<IPhysical> toGround = GetLandingPhobs();
            IList<IPhysical> toAir = GetLaunchingPhobs();
            MoveToGrounded(toGround);
            MoveToAirborne(toAir);
        }

        public void AddPhob(IPhysical obj)
        {
            PhobAccessors acc = new PhobAccessors();
            acc.setContactMask = obj.AddToEngine(globalAcceleration, mediumDensity, friction);
            accessors[obj] = acc;
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
            obj.RemoveFromEngine();
        }

        public void StartRecording()
        {
            if (RecordingQueryTime)
            {
                StopRecording();
            }
            RecordingQueryTime = true;
            queryTimes = new List<long>();
        }

        public void StopRecording()
        {
            RecordingQueryTime = false;
            if (queryTimes.Count > 0)
            {
                long avg = queryTimes.Sum() / queryTimes.Count;
                Game1.Debug("Avg: " + avg + " [" + queryTimes.Min() + ", " + queryTimes.Max() + "]");
            }
            else
            {
                Game1.Debug("No data for average query time");
            }
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
                if (!CheckGroundContact(phob))
                {
                    CorrectGroundLaunch(phob);
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
                IList<Block> objs = map.QueryPixels(phob.Bounds);
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
            phob.Velocity = new Vector2(phob.Velocity.X, 0);
            accessors[phob].setContactMask(phob.ContactMask | (int)ContactType.DOWN);
        }

        private void CorrectGroundLaunch(IPhysical phob)
        {
            accessors[phob].setContactMask(phob.ContactMask & ~(int)ContactType.DOWN);
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

        private bool CheckGroundContact(IPhysical toCheck)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            Rectangle bounds = toCheck.Bounds;
            Rectangle queryBounds = new Rectangle(bounds.X, bounds.Y + bounds.Height, bounds.Width, GROUND_EPSILON);
            if (RecordingQueryTime)
            {
                watch.Start();
            }
            IList<Block> query = map.QueryPixels(queryBounds);
            watch.Stop();
            if (RecordingQueryTime)
            {
                queryTimes.Add(watch.Elapsed.Ticks);
            }
            return query.Any();
        }
    }
}
