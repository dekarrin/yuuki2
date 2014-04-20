using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    class Camera : ScreenEntity
    {

        private ScreenEntity _follow;

        private int _offsetx;

        private int _offsety;

        public int TargetOffsetX
        {
            get
            {
                return _offsetx;
            }
            set
            {
                _offsetx = value;
                if (_follow != null)
                {
                    X = _follow.Position.X + _offsetx;
                }
            }
        }

        public int TargetOffsetY
        {
            get
            {
                return _offsety;
            }
            set
            {
                _offsety = value;
                if (_follow != null)
                {
                    Y = _follow.Position.Y + _offsety;
                }
            }
        }

        public ScreenEntity Target {
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
                    X = _follow.Position.X + TargetOffsetX;
                    Y = _follow.Position.Y + TargetOffsetY;
                }
            }
        }

        /// <summary>
        /// Amount we are offset from the upper left of the block that we are over.
        /// </summary>
        public Point BlockOffsets
        {
            get
            {
                return new Point(X % Game1.BLOCK_WIDTH, Y % Game1.BLOCK_HEIGHT);
            }
        }

        public Camera(Point size, ScreenEntity gc, Point offset) : base(size)
        {
            TargetOffsetX = offset.X; // must set targetoffset before target!
            TargetOffsetY = offset.Y;
            Target = gc;
        }

        public void HandleMovement(object sender, MovedEventArgs e)
        {
            X = e.NewPosition.X + TargetOffsetX;
            Y = e.NewPosition.Y + TargetOffsetY;
        }

    }
}
