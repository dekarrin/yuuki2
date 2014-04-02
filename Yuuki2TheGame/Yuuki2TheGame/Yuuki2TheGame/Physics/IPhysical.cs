using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace Yuuki2TheGame.Physics
{

    interface IPhysical : ICollidable
    {
        Body Body { get; }

        void SetWorld(World world);
    }
}
