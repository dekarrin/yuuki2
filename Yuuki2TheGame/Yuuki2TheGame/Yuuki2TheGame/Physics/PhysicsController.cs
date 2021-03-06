﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Yuuki2TheGame.Core;

namespace Yuuki2TheGame.Physics
{
    class PhysicsController
    {

        public bool RecordingQueryTime { get; private set; }

        /// <summary>
        /// Maps Phobs to their PhysicsPrivateMethod struct.
        /// </summary>
        private IDictionary<IPhysical, PhysicsPrivateMethods> accessors = new Dictionary<IPhysical, PhysicsPrivateMethods>();

        private Vector2 globalAcceleration;

        private const int CONTACT_EPSILON = 1;

        private const int MAX_ITEMS_PER_QUAD_LEAF = 0;

        /// <summary>
        /// This should be block_width times an odd number. Idk why. Fucking stupid quad tree.
        /// </summary>
        private const int MIN_QUAD_SIZE = Game1.METER_LENGTH * 3;

        private IList<IPhysical> activePhobs = new List<IPhysical>();

        private IList<IPhysical> inactivePhobs = new List<IPhysical>();

        private IList<IQuadObject> lands = new List<IQuadObject>();

        private QuadTree<IPhysical> phobTree = new QuadTree<IPhysical>(new Point(MIN_QUAD_SIZE, MIN_QUAD_SIZE), MAX_ITEMS_PER_QUAD_LEAF, false);

        private IList<long> queryTimes = new List<long>();

        private World map;
        
        private float mediumDensity;

        private float timescale;

        private float friction;

        private volatile bool inUpdate = false;

        private IList<IPhysical> deferredRemovals = new List<IPhysical>();

        private IList<IPhysical> deferredActivations = new List<IPhysical>();

        private IList<IPhysical> deferredDeactivations = new List<IPhysical>();

        private int minX = 0;

        private int minY = 0;

        private int maxX = 10000;

        private int maxY = 10000;

        private bool worldEdgeCollision = false;

        public PhysicsController(float wind, float gravity, float density, float friction, float timescale)
        {
            this.friction = friction;
            this.timescale = timescale;
            this.mediumDensity = density;
            this.globalAcceleration = new Vector2(wind, gravity);
        }

        public void SetBoundaryUse(bool useBounds)
        {
            this.worldEdgeCollision = useBounds;
        }

        public void SetWorldBoundaries(int minX, int minY, int maxX, int maxY)
        {
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;
        }

        public void AddMap(World map)
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
            inUpdate = true;
            foreach (IPhysical phob in activePhobs)
            {
                phob.UpdatePhysics(secs * timescale);
                foreach (ContactType type in Enum.GetValues(typeof(ContactType)))
                {
                    CheckWorldCollision(phob, type);
                }
                CheckPhobCollision(phob);
            }
            inUpdate = false;
            MakeDeferredChanges();
        }

        public void AddPhob(IPhysical obj)
        {
            PhysicsPrivateMethods acc = obj.AddToEngine(globalAcceleration, mediumDensity, friction);
            accessors[obj] = acc;
            obj.OnActivationChanged += HandlePhobActivationChange;
            if (obj.Active)
            {
                activePhobs.Add(obj);
                phobTree.Insert(obj);
            }
            else
            {
                inactivePhobs.Add(obj);
            }
        }

        public void RemovePhob(IPhysical obj)
        {
            // if we are in an update, we are iterating over activePhobs,
            // so we cannot remove from it
            if (!inUpdate)
            {
                if (obj.Active)
                {
                    activePhobs.Remove(obj);
                    phobTree.Remove(obj);
                }
                else
                {
                    inactivePhobs.Remove(obj);
                }
                obj.OnActivationChanged -= HandlePhobActivationChange;
                obj.RemoveFromEngine();
            }
            else
            {
                deferredRemovals.Add(obj);
            }
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

        public static float PixelsToMeters(int pixels)
        {
            return pixels / (float) Game1.METER_LENGTH;
        }

        public static int MetersToPixels(float physUnits)
        {
            return (int) Math.Round(physUnits * Game1.METER_LENGTH);
        }

        private void CorrectLandCollision(IPhysical phob, ContactType type, Point newPos)
        {
            phob.PhysPosition = new Vector2(PixelsToMeters(newPos.X), PixelsToMeters(newPos.Y));
            Vector2 vCorrection;
            switch (type)
            {
                default:
                case ContactType.INNER:
                case ContactType.DOWN:
                case ContactType.UP:
                    vCorrection = new Vector2(phob.Velocity.X, 0);
                    break;

                case ContactType.LEFT:
                case ContactType.RIGHT:
                    vCorrection = new Vector2(0, phob.Velocity.Y);
                    break;
            }
            phob.Velocity = vCorrection;
        }

        /// <summary>
        /// Checks for a collision between two phobs.
        /// </summary>
        /// <param name="phob"></param>
        private void CheckPhobCollision(IPhysical phob)
        {
            IList<IPhysical> results = phobTree.Query(phob.Bounds);
            foreach (IPhysical contacted in results)
            {
                if (contacted != phob)
                {
                    accessors[phob].notifyCollide(contacted);
                }
            }
        }

        private void CheckWorldCollision(IPhysical phob, ContactType type)
        {
            Block contact = CheckLandContact(phob, type);
            if (worldEdgeCollision && contact == null)
            {
                contact = CheckBoundaryContact(phob, type);
            }
            if (contact != null && !phob.IsInContact(type))
            {
                Point correction;
                switch (type)
                {
                    default:
                    case ContactType.INNER:
                    case ContactType.DOWN:
                        correction = new Point(phob.Bounds.X, contact.Bounds.Top - phob.Bounds.Height);
                        break;

                    case ContactType.UP:
                        correction = new Point(phob.Bounds.X, contact.Bounds.Bottom + CONTACT_EPSILON - 1);
                        break;

                    case ContactType.LEFT:
                        correction = new Point(contact.Bounds.Right + CONTACT_EPSILON - 1, phob.Bounds.Y);
                        break;

                    case ContactType.RIGHT:
                        correction = new Point(contact.Bounds.Left - phob.Bounds.Width, phob.Bounds.Y);
                        break;

                }
                CorrectLandCollision(phob, type, correction);
                accessors[phob].setContactMask(phob.ContactMask | (int)type);
            }
            else if (contact == null && phob.IsInContact(type))
            {
                accessors[phob].setContactMask(phob.ContactMask & ~(int)type);
            }
        }

        private Block CheckBoundaryContact(IPhysical toCheck, ContactType type)
        {
            Block contact = null;
            Rectangle box = toCheck.Bounds;
            switch (type)
            {
                default:
                case ContactType.DOWN:
                    if (box.Y >= maxY - box.Height)
                    {
                        contact = new Block(BlockID.Dirt, (int)PixelsToMeters(box.X), (int)PixelsToMeters(maxY));
                    }
                    break;

                case ContactType.UP:
                    if (box.Y <= minY)
                    {
                        contact = new Block(BlockID.Dirt, (int)PixelsToMeters(box.X), (int)PixelsToMeters(minY) - 1);
                    }
                    break;

                case ContactType.RIGHT:
                    if (box.X >= maxX - box.Width)
                    {
                        contact = new Block(BlockID.Dirt, (int)PixelsToMeters(maxX), (int)PixelsToMeters(box.Y));
                    }
                    break;

                case ContactType.LEFT:
                    if (box.X <= minX)
                    {
                        contact = new Block(BlockID.Dirt, (int)PixelsToMeters(minX) - 1, (int)PixelsToMeters(box.Y));
                    }
                    break;

                case ContactType.INNER:
                    // do nothing; this is fine as long as the phob never starts past one block
                    // outside the boundaries
                    break;
            }
            return contact;
        }

        private Block CheckLandContact(IPhysical toCheck, ContactType type)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            Rectangle bounds = toCheck.Bounds;
            Rectangle queryBounds;
            // TODO: this could probably be combined into one call to query
            switch (type)
            {
                default:
                case ContactType.DOWN:
                    queryBounds = new Rectangle(bounds.X, bounds.Y + bounds.Height, bounds.Width, CONTACT_EPSILON);
                    break;

                case ContactType.UP:
                    queryBounds = new Rectangle(bounds.X, bounds.Y - CONTACT_EPSILON, bounds.Width, CONTACT_EPSILON);
                    break;

                case ContactType.LEFT:
                    queryBounds = new Rectangle(bounds.X - CONTACT_EPSILON, bounds.Y, CONTACT_EPSILON, bounds.Height);
                    break;

                case ContactType.RIGHT:
                    queryBounds = new Rectangle(bounds.X + bounds.Width, bounds.Y, CONTACT_EPSILON, bounds.Height);
                    break;

                case ContactType.INNER:
                    queryBounds = bounds;
                    break;
            }
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
            if (query.Count > 0)
            {
                return query[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Makes all pending changes to the phob lists.
        /// </summary>
        private void MakeDeferredChanges()
        {
            foreach (IPhysical phob in deferredRemovals)
            {
                RemovePhob(phob);
            }
            foreach (IPhysical phob in deferredDeactivations)
            {
                HandlePhobDeactivation(phob);
            }
            foreach (IPhysical phob in deferredActivations)
            {
                HandlePhobActivation(phob);
            }
            deferredRemovals.Clear();
            deferredDeactivations.Clear();
            deferredActivations.Clear();
        }

        private void HandlePhobActivationChange(IPhysical phob)
        {
            if (phob.Active)
            {
                HandlePhobActivation(phob);
            }
            else
            {
                HandlePhobDeactivation(phob);
            }
        }

        private void HandlePhobDeactivation(IPhysical phob)
        {
            if (!inUpdate)
            {
                activePhobs.Remove(phob);
                phobTree.Remove(phob);
                inactivePhobs.Add(phob);
            }
            else
            {
                deferredDeactivations.Add(phob);
            }
        }

        private void HandlePhobActivation(IPhysical phob)
        {
            if (!inUpdate)
            {
                inactivePhobs.Remove(phob);
                activePhobs.Add(phob);
                phobTree.Insert(phob);
            }
            else
            {
                deferredActivations.Add(phob);
            }
        }
    }
}
