using System;
using Microsoft.Xna.Framework;

namespace CSharpQuadTree
{
    public interface IQuadObject
    {
        Rectangle Bounds { get; }
        event EventHandler BoundsChanged;
    }
}