using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    class Camera
    {

        private IPixelLocatable _follow;

        public Point Location { get; private set; }

        public Point TargetOffset { get; set; }

        public IPixelLocatable Target {
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
                return new Point(Location.X / Game1.BLOCK_WIDTH, Location.Y / Game1.BLOCK_HEIGHT);
            }
        }

        /// <summary>
        /// Amount we are offset from the upper left of the block that we are over.
        /// </summary>
        public Point Offsets
        {
            get
            {
                int x = Location.X;
                int y = Location.Y;
                return new Point(x % Game1.BLOCK_WIDTH, y % Game1.BLOCK_HEIGHT);
            }
        }

        public Camera(IPixelLocatable gc, Point offset)
        {
            Target = gc;
            TargetOffset = offset;
        }

        public void HandleMovement(object sender, MovedEventArgs e)
        {
            Point newLoc = new Point(e.NewLocation.X - TargetOffset.X, e.NewLocation.Y - TargetOffset.Y);
            Location = newLoc;
        }

    }
}
