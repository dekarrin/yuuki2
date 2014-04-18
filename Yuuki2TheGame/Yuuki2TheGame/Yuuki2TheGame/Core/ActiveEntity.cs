using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
            public ForceListing(Vector2 force, long time, string Name)
            {
                this.force = force;
                this.time = time;
                this.name = name;
            }
        }

        #region implementation instance vars

        private Vector2 _dampening = new Vector2(0, 0);
        
        private IDictionary<string, ForceListing> forces = new Dictionary<string, ForceListing>();

        private Vector2 _velocity = new Vector2(0, 0);

        #endregion

        #region properties

        public Vector2 Force {
            get
            {
                Vector2 sum = new Vector2(0, 0);
                foreach (ForceListing fl in forces.Values)
                {
                    sum.X += fl.force.X;
                    sum.Y += fl.force.Y;
                }
                return sum;
            }
        }

        public Vector2 Velocity
        {
            get
            {
                return _velocity;
            }
            set
            {
                _velocity = new Vector2(value.X, value.Y);
            }
        }

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

        public Vector2 Dampening
        {
            get
            {
                return _dampening;
            }
            set
            {
                _dampening = new Vector2(value.X, value.Y);
            }
        }

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
        }

        public void ApplyImpulse(Vector2 force)
        {
            AddForce(force, "impulse", 1);
        }

        public void UpdatePhysics(GameTime time)
        {
            SetForce(time);
            SetVelocity(time);
            SetPosition(time);
        }

        #endregion

        private void SetVelocity(GameTime time)
        {
            Velocity = Velocity + Acceleration * (time.ElapsedGameTime.Milliseconds * 1000.0f);
        }

        private void SetPosition(GameTime time)
        {
            Vector2 ds = Velocity * (time.ElapsedGameTime.Milliseconds * 1000.0f);
            Position = new Point((int)Math.Round(X + ds.X), (int)Math.Round(Y + ds.Y));
        }

        private void SetForce(GameTime time)
        {
            Dampen(time);
            IList<string> toRemove = new List<string>();
            foreach (ForceListing fl in forces.Values)
            {
                if (fl.time <= 0 || fl.force == Vector2.Zero)
                {
                    toRemove.Add(fl.name);
                }
                else
                {
                    fl.time -= time.ElapsedGameTime.Milliseconds;
                }
            }
        }

        private void Dampen(GameTime time)
        {
            foreach (ForceListing fl in forces.Values)
            {
                fl.force -= Dampening * time.ElapsedGameTime.Seconds;
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
