using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Physics
{

    interface IPhysical : IQuadObject
    {

        bool IsOnGround { get; set; }

        Vector2 Force { get; }

        Vector2 PhysPosition { get; set; }

        Vector2 Velocity { get; set; }

        Vector2 Acceleration { get; set; }

        float Mass { get; set; }

        Vector2 Dampening { get; set; }

        PhysicsController PhysicsEngine { get; set; }

        Vector2 GlobalForce { get; set; }

        void UpdatePhysics(float secs);

        void AddForce(Vector2 force);

        void AddForce(Vector2 force, string name);

        void AddForce(Vector2 force, string name, long duration);

        void ApplyImpulse(Vector2 force);

        void RemoveForce(string name);

        void RemoveAllForce();
    }
}
