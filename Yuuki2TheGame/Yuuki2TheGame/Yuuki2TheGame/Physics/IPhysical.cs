using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Physics
{

    delegate bool GroundContactChecker(Rectangle bounds);

    interface IPhysical : IQuadObject
    {

        bool IsOnGround { get; set; }

        Vector2 Force { get; }

        Vector2 PhysPosition { get; set; }

        Vector2 Velocity { get; set; }

        Vector2 Acceleration { get; set; }

        float Mass { get; set; }

        Vector2 Damping { get; set; }

        /// <summary>
        /// Acceleration imparted that cannot be removed except by explicitly setting GlobalAcceleration.
        /// </summary>
        Vector2 GlobalAcceleration { get; set; }

        GroundContactChecker CheckGroundContact { get; set; }

        void UpdatePhysics(float secs);

        void AddForce(Vector2 force);

        void AddForce(Vector2 force, string name);

        void AddForce(Vector2 force, string name, long duration);

        void AddForce(Vector2 force, string name, Vector2 maxVelocity);

        void ApplyImpulse(Vector2 force);

        void RemoveForce(string name);

        void RemoveAllForce();
    }
}
