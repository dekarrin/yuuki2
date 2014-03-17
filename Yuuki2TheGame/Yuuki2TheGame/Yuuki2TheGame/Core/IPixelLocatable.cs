using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    class MovedEventArgs : EventArgs
    {
        public Point OldLocation { get; set; }
        public Point NewLocation { get; set; }
    }

    delegate void MovedEventHandler(object source, MovedEventArgs e);

    /// <summary>
    /// Object that has a specific pixel location and that notifies listeners whenever it moves.
    /// </summary>
    interface IPixelLocatable
    {
        Point PixelLocation { get; }

        event MovedEventHandler OnMoved;
    }
}
