using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    class Camera
    {

        private ILocatable _follow;

        public Vector2 Location { get; private set; }

        public Vector2 Offset { get; set; }

        public ILocatable Target {
            get
            {
                return _follow;
            }
            set
            {
                if (_follow != null)
                {
                    _follow.OnMoved -= HandleMovement;
                }
                _follow = value;
                _follow.OnMoved += HandleMovement;
            }
        }

        /// <summary>
        /// Coordinates of the block that this camera is over.
        /// </summary>
        public Point Coordinates
        {
            get
            {
                return new Point((int) Math.Floor(Location.X), (int) Math.Floor(Location.Y));
            }
        }

        public Camera(ILocatable gc, Vector2 offset)
        {
            Target = gc;
            Offset = offset;
        }

        public void HandleMovement(object sender, MovedEventArgs e)
        {
            Location = e.NewLocation - Offset;
        }

    }
}
