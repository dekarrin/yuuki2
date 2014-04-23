using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Physics
{

    /// <summary>
    /// Given to the physics engine when added to it so that it has direct access to set
    /// properties that are not normally externally accessable.
    /// </summary>
    /// <typeparam name="T">The type of the property being set.</typeparam>
    /// <param name="value">What to set the value to.</param>
    delegate void PhysicsPrivateSetter<T>(T value);

    interface IPhysical : IQuadObject
    {

        #region properties

        bool IsOnGround { get; }

        Vector2 Force { get; }

        Vector2 PhysPosition { get; set; }

        Vector2 Velocity { get; set; }

        Vector2 Acceleration { get; set; }

        float Mass { get; set; }

        /// <summary>
        /// Percent of velocity lost per second. Used for fast simulation of friction.
        /// If a component is greater than 1, it will become a multiplied effect rather
        /// than a dividing effect, which may have unintended results.
        /// </summary>
        Vector2 Damping { get; set; }

        /// <summary>
        /// Acceleration imparted that cannot be removed except by explicitly setting GlobalAcceleration.
        /// </summary>
        Vector2 GlobalAcceleration { get; set; }

        #endregion

        #region methods

        void UpdatePhysics(float secs);

        void AddForce(Vector2 force);

        void AddForce(Vector2 force, string name);

        void AddForce(Vector2 force, string name, long duration);

        void AddForce(Vector2 force, string name, Vector2 maxVelocity);

        void ApplyImpulse(Vector2 force);

        void RemoveForce(string name);

        void RemoveAllForce();

        /// <summary>
        /// Called when the IPhysical is added to the physics engine.
        /// </summary>
        /// <param name="globalAcceleration">The amount of global acceleration that this IPhysical should undergo.</param>
        /// <returns>Returns a setter for the IsOnGround property.</returns>
        PhysicsPrivateSetter<bool> AddToEngine(Vector2 globalAcceleration);

        void RemoveFromEngine();

        #endregion
    }
}
