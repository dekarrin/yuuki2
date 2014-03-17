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

        private int _x;

        private int _y;

        private int _targetx;

        private int _targety;

        public Point Location
        {
            get
            {
                return new Point(_x, _y);
            }
        }

        public int TargetOffsetX
        {
            get
            {
                return _targetx;
            }
            set
            {
                _targetx = value;
                if (_follow != null)
                {
                    _x = _follow.PixelLocation.X + _targetx;
                }
            }
        }

        public int TargetOffsetY
        {
            get
            {
                return _targety;
            }
            set
            {
                _targety = value;
                if (_follow != null)
                {
                    _y = _follow.PixelLocation.Y + _targety;
                }
            }
        }

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
                if (_follow != null)
                {
                    _follow.OnMoved += HandleMovement;
                    _x = _follow.PixelLocation.X + TargetOffsetX;
                    _y = _follow.PixelLocation.Y + TargetOffsetY;
                }
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
            TargetOffsetX = offset.X; // must set targetoffset before target!
            TargetOffsetY = offset.Y;
            Target = gc;
        }

        public void HandleMovement(object sender, MovedEventArgs e)
        {
            _x = e.NewLocation.X + TargetOffsetX;
            _y = e.NewLocation.Y + TargetOffsetY;
        }

    }
}
