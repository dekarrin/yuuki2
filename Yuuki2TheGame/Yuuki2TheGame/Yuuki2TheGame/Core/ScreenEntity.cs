using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    /// <summary>
    /// Argument to a <see cref="PositionChangedEventHandler" />. Contains the new
    /// and old positions of the object that generated the event.
    /// </summary>
    class PositionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The position of the object prior to the move.
        /// </summary>
        public Point OldPosition { get; set; }

        /// <summary>
        /// The position of the object after the move.
        /// </summary>
        public Point NewPosition { get; set; }
    }
    
    /// <summary>
    /// Argument to a <see cref="SizeChangedEventHandler" />. Contains the new
    /// and old sizes of the object that generated the event.
    /// </summary>
    class SizeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The size of the object prior to the change.
        /// </summary>
        public Point OldSize { get; set; }

        /// <summary>
        /// The size of the object after the change.
        /// </summary>
        public Point NewSize { get; set; }
    }

    /// <summary>
    /// Event delegate for handling a change in position of a <see cref="ScreenEntity"/>.
    /// </summary>
    /// <param name="source">The <c>ScreenHandler</c> that generated the event.</param>
    /// <param name="e">Contains the old position and the new position.</param>
    delegate void PositionChangedEventHandler(ScreenEntity source, PositionChangedEventArgs e);

    /// <summary>
    /// Event delegate for handling a change in size of a <see cref="ScreenEntity"/>.
    /// </summary>
    /// <param name="source">The <c>ScreenHandler</c> that generated the event.</param>
    /// <param name="e">Contains the old size and the new size.</param>
    delegate void SizeChangedEventHandler(ScreenEntity source, SizeChangedEventArgs e);

    /// <summary>
    /// An object that exists on the screen. ScreenEntity instances have both size and position, as well
    /// as a texture string that identifies what texture should be used to draw them.
    /// </summary>
    class ScreenEntity : Yuuki2TheGame.Physics.IQuadObject
    {

        #region property implementation

        private int _x;

        private int _y;

        private int _width;

        private int _height;

        #endregion

        #region properties

        /// <summary>
        /// The X-coordinate of the upper-left corner of this ScreenEntity, measured in
        /// pixels. If subclasses override this, they must ensure that the setter calls
        /// OnMoved when the new value differs from the old one.
        /// </summary>
        public virtual int X
        {
            get
            {
                return _x;
            }
            set
            {
                if (_x != value && OnPositionChanged != null)
                {
                    Point oldV = new Point(_x, _y);
                    Point newV = new Point(value, _y);
                    PositionChangedEventArgs mea = new PositionChangedEventArgs();
                    mea.NewPosition = newV;
                    mea.OldPosition = oldV;
                    OnPositionChanged(this, mea);
                }
                _x = value;
            }
        }

        /// <summary>
        /// The Y-coordinate of the upper-left corner of this ScreenEntity, measured in
        /// pixels. If subclasses override this, they must ensure that the setter calls
        /// OnMoved when the new value differs from the old one.
        /// </summary>
        public virtual int Y
        {
            get
            {
                return _y;
            }
            set
            {
                if (_y != value && OnPositionChanged != null)
                {
                    Point oldV = new Point(_x, _y);
                    Point newV = new Point(_x, value);
                    PositionChangedEventArgs mea = new PositionChangedEventArgs();
                    mea.NewPosition = newV;
                    mea.OldPosition = oldV;
                    OnPositionChanged(this, mea);
                }
                _y = value;
            }
        }
        
        /// <summary>
        /// The width of this ScreenEntity, measured in pixels. If subclasses override this, they
        /// must ensure that the setter calls OnSizeChanged when the new value differs from the
        /// old one.
        /// </summary>
        public virtual int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (_width != value && OnSizeChanged != null)
                {
                    Point oldV = new Point(_width, _height);
                    Point newV = new Point(value, _height);
                    SizeChangedEventArgs mea = new SizeChangedEventArgs();
                    mea.NewSize = newV;
                    mea.OldSize = oldV;
                    OnSizeChanged(this, mea);
                }
                _width = value;
            }
        }

        /// <summary>
        /// The height of this ScreenEntity, measured in pixels. If subclasses override this, they
        /// must ensure that the setter calls OnSizeChanged when the new value differs from the old
        /// one.
        /// </summary>
        public virtual int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (_height != value && OnSizeChanged != null)
                {
                    Point oldV = new Point(_width, _height);
                    Point newV = new Point(_width, value);
                    SizeChangedEventArgs mea = new SizeChangedEventArgs();
                    mea.NewSize = newV;
                    mea.OldSize = oldV;
                    OnSizeChanged(this, mea);
                }
                _height = value;
            }
        }

        /// <summary>
        /// The absolute pixel location (relative to the upper left of the map) of the
        /// upper left corner of this ScreenEntity. If subclasses override this, they
        /// must ensure that the setter calls OnMoved when either the X or Y of the value
        /// differs from the current X or Y.
        /// </summary>
        public virtual Point Position
        {
            get
            {
                return new Point(X, Y);
            }
            set
            {
                if ((_x != value.X || _y != value.Y) && OnPositionChanged != null)
                {
                    Point oldV = new Point(_x, _y);
                    Point newV = new Point(value.X, value.Y);
                    PositionChangedEventArgs mea = new PositionChangedEventArgs();
                    mea.NewPosition = newV;
                    mea.OldPosition = oldV;
                    OnPositionChanged(this, mea);
                }
                _x = value.X;
                _y = value.Y;
            }
        }

        /// <summary>
        /// The size of this ScreenEntity, measured in pixels. If subclasses override this, they
        /// must ensure that the setter calls OnSizeChanged when either the X or Y of the new
        /// value differs from the current Width and Height, respectively.
        /// </summary>
        public virtual Point Size
        {
            get
            {
                return new Point(Width, Height);
            }
            set
            {
                if ((_width != value.X || _height != value.Y) && OnSizeChanged != null)
                {
                    Point oldV = new Point(_width, _height);
                    Point newV = new Point(value.X, value.Y);
                    SizeChangedEventArgs mea = new SizeChangedEventArgs();
                    mea.NewSize = newV;
                    mea.OldSize = oldV;
                    OnSizeChanged(this, mea);
                }
                _width = value.X;
                _height = value.Y;
            }
        }

        /// <summary>
        /// The bounding box for this ScreenEntity. This is determined by using
        /// the X, Y, Width, and Height properties of this ScreenEntity.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(X, Y, Width, Height);
            }
        }
        
        /// <summary>
        /// The ID of the texture that should be used when drawing this ScreenEntity.
        /// May be null if the ScreenEntity is never intended to actually be drawn.
        /// </summary>
        public virtual string Texture { get; set; }

        /// <summary>
        /// Gets the Sprite object for this ScreenEntity, which is intended to be
        /// passed to drawing functions. The Sprite simply takes the position, size and
        /// texture ID and combines them into one class.
        /// </summary>
        public Yuuki2TheGame.Graphics.Sprite Sprite
        {
            get
            {
                return new Graphics.Sprite(this);
            }
        }

        /// <summary>
        /// The X-coordinate of this ScreenEntity, measured in meters.
        /// </summary>
        public float BlockX
        {
            get
            {
                return _x / (float)Game1.METER_LENGTH;
            }
            set
            {
                int newX = (int)Math.Round(value * Game1.METER_LENGTH);
                X = newX;
            }
        }

        /// <summary>
        /// The Y-coordinate of this ScreenEntity, measured in meters.
        /// </summary>
        public float BlockY
        {
            get
            {
                return _y / (float)Game1.METER_LENGTH;
            }
            set
            {
                int newY = (int)Math.Round(value * Game1.METER_LENGTH);
                Y = newY;
            }
        }
        
        /// <summary>
        /// The width of this ScreenEntity, measured in meters.
        /// </summary>
        public float BlockWidth
        {
            get
            {
                return _width / (float)Game1.METER_LENGTH;
            }
            set
            {
                int newX = (int)Math.Round(value * Game1.METER_LENGTH);
                Width = newX;
            }
        }

        /// <summary>
        /// The height of this ScreenEntity, measured in meters.
        /// </summary>
        public float BlockHeight
        {
            get
            {
                return _height / (float)Game1.METER_LENGTH;
            }
            set
            {
                int newY = (int)Math.Round(value * Game1.METER_LENGTH);
                Height = newY;
            }
        }

        /// <summary>
        /// The position of this ScreenEntity measured in meters. The pixel size of a meter may
        /// be determined by referring to <see cref="Game1.METER_LENGTH"/> and
        /// <see cref="Game1.METER_LENGTH"/>.
        /// </summary>
        public Vector2 BlockPosition
        {
            get
            {
                return new Vector2(BlockX, BlockY);
            }
            set
            {
                int newX = (int)Math.Round(value.X * Game1.METER_LENGTH);
                int newY = (int)Math.Round(value.Y * Game1.METER_LENGTH);
                Position = new Point(newX, newY);
            }
        }

        /// <summary>
        /// The size of this ScreenEntity measured in meter. The pixel size of a meter may
        /// be determined by referring to <see cref="Game1.METER_LENGTH"/> and
        /// <see cref="Game1.METER_LENGTH"/>.
        /// </summary>
        public Vector2 BlockSize
        {
            get
            {
                return new Vector2(BlockWidth, BlockHeight);
            }
            set
            {
                int newX = (int)Math.Round(value.X * Game1.METER_LENGTH);
                int newY = (int)Math.Round(value.Y * Game1.METER_LENGTH);
                Size = new Point(newX, newY);
            }
        }

        #endregion

        #region events

        /// <summary>
        /// Called when the position of this ScreenEntity changes.
        /// </summary>
        public event PositionChangedEventHandler OnPositionChanged = delegate(ScreenEntity source, PositionChangedEventArgs args)
        {
            if (source.OnBoundsChanged != null)
            {
                source.OnBoundsChanged(source, null);
            }
        };
        
        /// <summary>
        /// Called when the size of this ScreenEntity changes.
        /// </summary>
        public event SizeChangedEventHandler OnSizeChanged = delegate(ScreenEntity source, SizeChangedEventArgs args)
        {
            if (source.OnBoundsChanged != null)
            {
                source.OnBoundsChanged(source, null);
            }
        };
        
        /// <summary>
        /// Called when either the position or the size changes. Note on deriving:
        /// calling OnMoved or OnSizeChanged automatically calls OnBoundsChanged.
        /// </summary>
        public event EventHandler OnBoundsChanged = null;

        #endregion

        #region ctors

        /// <summary>
        /// Creates a new ScreenEntity with a position of (0, 0) and a texture ID set to null.
        /// </summary>
        /// <param name="size">The size (in pixels) of the new ScreenEntity.</param>
        public ScreenEntity(Point size)
            : this(size, new Point(0, 0))
        {}

        /// <summary>
        /// Creates a new ScreenEntity with a texture ID set to null.
        /// </summary>
        /// <param name="size">The size (in pixels) of the new ScreenEntity.</param>
        /// <param name="position">The position (in pixels) of the new ScreenEntity.</param>
        public ScreenEntity(Point size, Point position)
            : this(size, position, null)
        {}

        /// <summary>
        /// Creates a new ScreenEntity.
        /// </summary>
        /// <param name="size">The size (in pixels) of the new ScreenEntity.</param>
        /// <param name="position">The position (in pixels) of the new ScreenEntity.</param>
        /// <param name="texture">The texture ID of the new ScreenEntity.</param>
        public ScreenEntity(Point size, Point position, string texture)
        {
            _width = size.X;
            _height = size.Y;
            _x = position.X;
            _y = position.Y;
            Texture = texture;
        }

        #endregion

        #region methods

        public virtual void Teleport(Point p)
        {
            Position = p;
        }

        #endregion
    }
}
