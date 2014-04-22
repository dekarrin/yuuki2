using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Yuuki2TheGame.Physics;

namespace Yuuki2TheGame.Core
{
    /// <summary>
    /// Capable of having physics applied.
    /// </summary>
    class ActiveEntity : ScreenEntity, Yuuki2TheGame.Physics.IPhysical
    {

        private const string DEFAULT_FORCE_NAME = "__DEFAULT__";

        private class ForceListing {
            public Vector2 originalForce;
            public Vector2 force;
            public long duration;
            public string name;
            public bool timed;
            public bool hasMaxVelocity = false;
            public Vector2 maxVelocity = Vector2.Zero;
            public ForceListing(Vector2 force, long time, string name)
            {
                this.force = originalForce = force;
                this.duration = time;
                this.name = name;
                this.timed = time != 0;
            }
        }

        #region implementation instance vars
        
        private IDictionary<string, ForceListing> forces = new Dictionary<string, ForceListing>();

        private Vector2 _position = Vector2.Zero;

        private Vector2 _global_force = Vector2.Zero;

        private Vector2 _global_accel = Vector2.Zero;

        private float _mass = 0.0f;

        #endregion

        #region properties

        public Vector2 GlobalAcceleration
        {
            get
            {
                return _global_accel;
            }
            set
            {
                _global_accel = value;
                _global_force = value * Mass;
            }
        }

        public Vector2 PhysPosition
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                BlockPosition = value;
            }
        }

        public bool IsOnGround { get; set; }

        public Vector2 Force {
            get
            {
                Vector2 sum = new Vector2(0, 0);
                foreach (ForceListing fl in forces.Values)
                {
                    sum.X += fl.force.X;
                    sum.Y += fl.force.Y;
                }
                sum.X += _global_force.X;
                sum.Y += _global_force.Y;
                return sum;
            }
        }

        public Vector2 Velocity { get; set; }

        public Vector2 Acceleration
        {
            get
            {
                return Force / Mass;
            }
            set
            {
                RemoveAllForce();
                AddForce(value * Mass);
            }
        }

        public Vector2 Damping { get; set; }

        /// <summary>
        /// If this is never set or is set to 0 (which is physically impossible), it will be assumed that the item has
        /// a density of 50kg / m^2 and the mass will be set appropriately.
        /// </summary>
        public float Mass
        {
            get
            {
                if (_mass == 0.0f)
                {
                    float area = BlockSize.X * BlockSize.Y;
                    _mass = 50.0f * area;
                }
                return _mass;
            }
            set
            {
                float oldMass = _mass;
                _mass = value;
                float newMass = Mass; // so we get error checking in case mass was set to 0.
                if (newMass != oldMass && GlobalAcceleration != Vector2.Zero)
                {
                    // we must recalculate global force
                    _global_force = _global_accel * newMass;
                }
            }
        }

        public GroundContactChecker CheckGroundContact { get; set; }

        #endregion

        #region public methods

        public void AddForce(Vector2 force)
        {
            AddForce(force, DEFAULT_FORCE_NAME, 0);
        }

        public void AddForce(Vector2 force, string name)
        {
            AddForce(force, name, 0);
        }

        public void AddForce(Vector2 force, string name, long time)
        {
            if (!forces.ContainsKey(name))
            {
                forces[name] = new ForceListing(new Vector2(0, 0), 0, name);
            }
            ForceListing fl = forces[name];
            fl.force += force;
            fl.duration += time;
        }

        public void AddForce(Vector2 force, string name, Vector2 maxVelocity)
        {
            if (!forces.ContainsKey(name))
            {
                forces[name] = new ForceListing(new Vector2(0, 0), 0, name);
            }
            ForceListing fl = forces[name];
            fl.force += force;
            fl.maxVelocity = maxVelocity;
        }

        public void RemoveForce(string name)
        {
            forces.Remove(name);
        }

        public void RemoveAllForce()
        {
            forces.Clear();
            GlobalAcceleration = Vector2.Zero;
        }

        public void ApplyImpulse(Vector2 force)
        {
            AddForce(force, "impulse", 1);
        }

        public void UpdatePhysics(float secs)
        {
            SetForce(secs);
            SetVelocity(secs);
            SetPosition(secs);
        }

        #endregion

        public ActiveEntity(Point size)
            : this(size, new Point(0, 0))
        { }

        public ActiveEntity(Point size, Point position)
            : this(size, position, null)
        { }

        public ActiveEntity(Point size, Point position, string texture)
            : base(size, position, texture)
        {
            _position = BlockPosition;
            OnPositionChanged += delegate(ScreenEntity sender, PositionChangedEventArgs mea)
            {
                if (IsOnGround && mea.NewPosition.Y != mea.OldPosition.Y)
                {
                    IsOnGround = false;
                }
            };
        }

        private void SetVelocity(float secs)
        {
            Velocity = Velocity + Acceleration * secs;
            Dampen(secs);
        }

        private void SetPosition(float secs)
        {
            Vector2 ds = Velocity * secs;
            // if we were on the ground, and we're still on the ground, we'd better stay on the ground!
            if (IsOnGround && ds.Y > 0 && CheckGroundContact != null && CheckGroundContact(Bounds))
            {
                ds.Y = 0;
            }
            PhysPosition = new Vector2(PhysPosition.X + ds.X, PhysPosition.Y + ds.Y);
        }

        private void SetForce(float secs)
        {
            IList<string> toRemove = new List<string>();
            foreach (ForceListing fl in forces.Values)
            {
                if (fl.timed && fl.duration <= 0)
                {
                    toRemove.Add(fl.name);
                }
                else
                {
                    if (fl.timed)
                    {
                        fl.duration -= (int)Math.Round(secs * 1000);
                    }
                    LimitByVelocity(fl);
                }
            }
        }

        private void LimitByVelocity(ForceListing fl)
        {
            if (fl.hasMaxVelocity && (fl.maxVelocity.X != 0 || fl.maxVelocity.Y != 0))
            {
                bool xIsAffected = (fl.maxVelocity.X > 0 && Velocity.X > fl.maxVelocity.X) || (fl.maxVelocity.X < 0 && Velocity.X < fl.maxVelocity.X);
                bool yIsAffected = (fl.maxVelocity.Y > 0 && Velocity.Y > fl.maxVelocity.Y) || (fl.maxVelocity.Y < 0 && Velocity.Y < fl.maxVelocity.Y);
                if (xIsAffected)
                {
                    fl.force.X = 0;
                }
                else
                {
                    fl.force.X = fl.originalForce.X;
                }
                if (yIsAffected)
                {
                    fl.force.Y = 0;
                }
                else
                {
                    fl.force.Y = fl.originalForce.Y;
                }
            }
        }

        private void Dampen(float secs)
        {
            int multX = (Velocity.X == 0) ? 0 : (Velocity.X < 0) ? -1 : 1;
            int multY = (Velocity.Y == 0) ? 0 : (Velocity.Y < 0) ? -1 : 1;
            Velocity -= new Vector2(multX * Damping.X, multY * Damping.Y) * secs;
        }
    }
}
