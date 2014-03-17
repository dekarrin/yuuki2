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


        // This Method is a static scope method that will take a non-grid position ( XXXX.xx, YYYY.yy) and 
        // will give you the camera origin specific pixel coordinates for that position. This will be used for
        // drawing things on the screen that are not fixed to the grid such as items, and characters.               -CA
        public static Point PixPosFromVector2(Vector2 gridCoord)
        {
            Camera cm = Engine.Camera;
            Point screenPosition;

            int x, y;
            float a, b;
            x = (int)Math.Truncate(gridCoord.X);    // XXXX of XXXX.xx
            a = (gridCoord.X - x);                  // 0.xx of XXXX.xx
            y = (int)Math.Truncate(gridCoord.Y);    // YYYY of YYYY.yy
            b = (gridCoord.Y - y);                  // 0.yy of YYYY.yy

            x = cm.Coordinates.X - x;                  // difference of grid points (gp)
            y = cm.Coordinates.Y - y;                  // see above ^
            a = cm.Offsets.X - a;                      // difference of partial gp offsets ( 00 - 16)
            b = cm.Offsets.Y - b;                      // see above ^

            x = x * 16;                             // x * blocksize = rough pixel width pos
            y = y * 16;                             // y * blocksize = rough pixel height pos

            a = a * 100;                            // 0.xx = XX pixels
            b = b * 100;                            // 0.yy = YY pixels

            x = x + (int)a;                         // XXXX + XX = actual pixel location from (0,0)
            y = y + (int)b;                         // YYYY + YY = actual pixel location from (0,0)

            return screenPosition = new Point(x,y);
        }

    }
}
