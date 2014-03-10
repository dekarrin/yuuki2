﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    class MovedEventArgs : EventArgs
    {
        public Vector2 OldLocation { get; set; }
        public Vector2 NewLocation { get; set; }
    }

    delegate void MovedEventHandler(object source, MovedEventArgs e);

    interface ILocatable
    {
        Vector2 Location { get; }

        event MovedEventHandler OnMoved;
    }
}
