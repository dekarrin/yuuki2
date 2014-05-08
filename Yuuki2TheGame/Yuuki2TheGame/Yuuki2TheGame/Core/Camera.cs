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

        public Rectangle TargetBox { get; private set; }

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
                    Focus(_follow.Position);
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
                return new Point(X % Game1.METER_LENGTH, Y % Game1.METER_LENGTH);
            }
        }

        public Camera(Point size, ScreenEntity gc, Rectangle targetBox, Rectangle range)
            : base(size)
        {
            TargetBox = targetBox;
            Range = range;
            Target = gc;
        }

        public void HandleMovement(object sender, PositionChangedEventArgs e)
        {
            Point p = e.NewPosition;
            Focus(p);
        }

        private void Focus(Point p)
        {
            if (p.X > X + TargetBox.X + TargetBox.Width - 1)
            {
                X = Math.Min(p.X - (TargetBox.Width - 1) - TargetBox.X, (Range.X + Range.Width - 1) - (Width - 1));
            }
            else if (p.X < X + TargetBox.X)
            {
                X = Math.Max(p.X - TargetBox.X, Range.X);
            }
            if (p.Y > Y + TargetBox.Y + TargetBox.Height - 1)
            {
                Y = Math.Min(p.Y - (TargetBox.Height - 1) - TargetBox.Y, (Range.Y + Range.Height - 1) - (Height - 1));
            }
            else if (p.Y < Y + TargetBox.Y)
            {
                Y = Math.Max(p.Y - TargetBox.Y, Range.Y);
            }
        }

        private void IsInTargetBox(Point p)
        {
            bool inHorz = (p.X >= X + TargetBox.X);
        }

    }
}
