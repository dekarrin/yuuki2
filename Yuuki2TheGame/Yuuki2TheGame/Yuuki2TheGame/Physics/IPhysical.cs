using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Physics
{

    interface IPhysical : ICollidable
    {
        /// <summary>
        /// In kilograms.
        /// </summary>
        float Mass { get; }

        float XPosition { get; set; }

        float YPosition { get; set; }

        float XVelocity { get; set; }

        float YVelocity { get; set; }

        float XAcceleration { get; set; }

        float YAcceleration { get; set; }

        float XForce { get; }

        float YForce { get; }

        /// <summary>
        /// Adds a named force that is continuously applied until later explicitly removed.
        /// Force is in Newtons. If force with name already exists, it is overwritten.
        /// </summary>
        /// <param name="?"></param>
        void AddForce(string name, Vector2 force);

        /// <summary>
        /// Removes a named force.
        /// </summary>
        /// <param name="name"></param>
        void RemoveForce(string name);

        /// <summary>
        /// Checks if there is a force with the given name acting on this Phob.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasForce(string name);

        /// <summary>
        /// Applies a force for a certain amount of time, after which it stops.
        /// </summary>
        /// <param name="duration">milliseconds to apply force for.</param>
        /// <param name="force">Force vector in Newtons.</param>
        void ApplyImpulse(long duration, Vector2 force);
    }
}
