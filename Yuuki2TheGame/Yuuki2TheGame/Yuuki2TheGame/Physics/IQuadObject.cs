using System;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Physics
{
    public interface IQuadObject
    {
        Rectangle Bounds { get; }
        event EventHandler BoundsChanged;
    }
}