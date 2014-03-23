using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Yuuki2TheGame.Graphics
{
    /// <summary>
    /// Item drawn on screen. Has a name for texture that is translated into texture.
    /// </summary>
    class Sprite
    {
        private Point _align = new Point(0, 0);

        private Point _sourceSize = new Point(0, 0);

        private enum SourceSizeModeType
        {
            NONE, // used when graphic size or align have not been set.
            EXPLICIT, // used when it is set exactly.
            IMPLICIT // used when we calc it from texture2d.
        }

        /// <summary>
        /// Determines how the source rectangle is calculated. If this is set to NONE,
        /// null is returned for source rectangle. If it is set to EXPLICIT, the size
        /// of the source rectangle is taken from the exact value of TextureSize. If it
        /// is set to IMPLICIT, the size is calculated by taking the difference of the
        /// TextureOffset and the size of the Texture, but will return null if the
        /// texture has not yet been set.
        /// </summary>
        private SourceSizeModeType _sourceSizeMode = SourceSizeModeType.NONE;

        public Point Position { get; set; }

        public Point Size { get; set; }

        public Point Align
        {
            get
            {
                return _align;
            }

            set
            {
                if (_sourceSizeMode == SourceSizeModeType.NONE)
                {
                    _sourceSizeMode = SourceSizeModeType.IMPLICIT;
                }
                _align = value;
            }
        }

        public Point SourceSize
        {
            get
            {
                return _sourceSize;
            }

            set
            {
                _sourceSizeMode = SourceSizeModeType.EXPLICIT;
                _sourceSize = value;
            }
        }

        /// <summary>
        /// Combines Position and Size into destination rectangle.
        /// </summary>
        public Rectangle Destination
        {
            get
            {
                return new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
            }
        }

        /// <summary>
        /// Returns the source rectangle. This will be null if neither the Align nor the
        /// TextureSize have been set, or the appropriate value if either one have been set.
        /// If Align has been set, but not SourceSize, Source will be the entire texture
        /// starting with the coordinates in Align (note that this requires the Texture to
        /// have been set. If it hasn't been set, this will return null). If SourceSize is
        /// set but not Align, Align is assumed to be (0, 0).
        /// </summary>
        public Nullable<Rectangle> Source
        {
            get
            {
                Nullable<Rectangle> val = null;
                switch (_sourceSizeMode)
                {
                    case SourceSizeModeType.EXPLICIT:
                        val = new Nullable<Rectangle>(new Rectangle(_align.X, _align.Y, _sourceSize.X, _sourceSize.Y));
                        break;

                    case SourceSizeModeType.IMPLICIT:
                        if (Texture != null)
                        {
                            int w = Texture.Width - _align.X;
                            int h = Texture.Height - _align.Y;
                            val = new Nullable<Rectangle>(new Rectangle(_align.X, _align.Y, w, h));
                        }
                        else
                        {
                            val = null;
                        }
                        break;

                    case SourceSizeModeType.NONE:
                    default:
                        val = null;
                        break;
                }
                return val;
            }
        }

        public Texture2D Texture { get; set; }

        public string TextureID { get; set; }

        public Sprite(Point position, Point size, string textureid)
        {
            Position = position;
            Size = size;
            TextureID = textureid;
            Align = new Point(0, 0);
            Texture = null;
        }
    }
}
