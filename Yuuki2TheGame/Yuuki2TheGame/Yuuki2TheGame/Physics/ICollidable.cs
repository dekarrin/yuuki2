using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Physics
{
    interface ICollidable
    {
        /// <summary>
        /// 
        /// </summary>
        Rectangle BoundingBox { get; }
    }
}
