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

        public Vector2 TargetOffset { get; set; }

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
                return new Point((int) Math.Truncate(Location.X), (int) Math.Truncate(Location.Y));
            }
        }

        /// <summary>
        /// Amount we are offset from the upper left of the block that we are over.
        /// </summary>
        public Vector2 Offsets
        {
            get
            {
                float x = Location.X;
                float y = Location.Y;
                return new Vector2(x - (float) Math.Truncate(x), y - (float) Math.Truncate(y));
            }
        }

        public Camera(ILocatable gc, Vector2 offset)
        {
            Target = gc;
            TargetOffset = offset;
        }

        public void HandleMovement(object sender, MovedEventArgs e)
        {
            Location = e.NewLocation - TargetOffset;
        }

    }
}
