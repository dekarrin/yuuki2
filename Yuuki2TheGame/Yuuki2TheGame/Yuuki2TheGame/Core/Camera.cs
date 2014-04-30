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

        public Rectangle Range { get; set; }

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
                    X = Math.Min(Math.Max(_follow.Position.X + _offsetx, Range.X), (Range.X + Range.Width - 1) - (Width - 1));
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
                    Y = Math.Min(Math.Max(_follow.Position.Y + _offsety, Range.Y), (Range.Y + Range.Height - 1) - (Height - 1));
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
                    _follow.OnPositionChanged -= HandleMovement;
                }
                _follow = value;
                if (_follow != null)
                {
                    _follow.OnPositionChanged += HandleMovement;
                    X = Math.Min(Math.Max(_follow.Position.X + TargetOffsetX, Range.X), (Range.X + Range.Width - 1) - (Width - 1));
                    Y = Math.Min(Math.Max(_follow.Position.Y + TargetOffsetY, Range.Y), (Range.Y + Range.Height - 1) - (Height - 1));
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

        public Camera(Point size, ScreenEntity gc, Point offset, Rectangle range) : base(size)
        {
            TargetOffsetX = offset.X; // must set targetoffset before target!
            TargetOffsetY = offset.Y;
            Target = gc;
            Range = range;
        }

        public void HandleMovement(object sender, PositionChangedEventArgs e)
        {
            X = Math.Min(Math.Max(e.NewPosition.X + TargetOffsetX, Range.X), (Range.X + Range.Width - 1) - (Width - 1));
            Y = Math.Min(Math.Max(e.NewPosition.Y + TargetOffsetY, Range.Y), (Range.Y + Range.Height - 1) - (Height - 1));
        }

    }
}
