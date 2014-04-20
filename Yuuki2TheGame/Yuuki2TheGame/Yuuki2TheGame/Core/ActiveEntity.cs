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
            public Vector2 force;
            public long time;
            public string name;
            public ForceListing(Vector2 force, long time, string name)
            {
                this.force = force;
                this.time = time;
                this.name = name;
            }
        }

        #region implementation instance vars
        
        private IDictionary<string, ForceListing> forces = new Dictionary<string, ForceListing>();

        private Vector2 _position = Vector2.Zero;

        private Vector2 _global_force = Vector2.Zero;

        #endregion

        #region properties

        public PhysicsController PhysicsEngine { get; set; }

        public Vector2 GlobalForce
        {
            get
            {
                return _global_force;
            }
            set
            {
                _global_force = value;
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

        public Vector2 Dampening { get; set; }

        public float Mass { get; set; }

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
            ForceListing fl = forces[null];
            fl.force += force;
            fl.time += time;
        }

        public void RemoveForce(string name)
        {
            forces.Remove(name);
        }

        public void RemoveAllForce()
        {
            forces.Clear();
            GlobalForce = Vector2.Zero;
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
            OnMoved += delegate(Object sender, MovedEventArgs mea)
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
        }

        private void SetPosition(float secs)
        {
            Vector2 ds = Velocity * secs;

            // if we were on the ground, and we're still on the ground, we'd better stay on the ground!
            if (IsOnGround && ds.Y > 0 && PhysicsEngine.BoxIsOnGround(Bounds))
            {
                ds.Y = 0;
            }
            PhysPosition = new Vector2(PhysPosition.X + ds.X, PhysPosition.Y + ds.Y);
        }

        private void SetForce(float secs)
        {
            Dampen(secs);
            IList<string> toRemove = new List<string>();
            foreach (ForceListing fl in forces.Values)
            {
                if (fl.time <= 0 || fl.force == Vector2.Zero)
                {
                    toRemove.Add(fl.name);
                }
                else
                {
                    fl.time -= (int) Math.Round(secs * 1000);
                }
            }
        }

        private void Dampen(float secs)
        {
            foreach (ForceListing fl in forces.Values)
            {
                fl.force -= Dampening * secs;
                if (fl.force.X < 0)
                {
                    fl.force.X = 0;
                }
                if (fl.force.Y < 0)
                {
                    fl.force.Y = 0;
                }
            }
        }
    }
}
