using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    public class MovedEventArgs : EventArgs
    {
        public Point OldPosition { get; set; }
        public Point NewPosition { get; set; }
    }
    
    public class SizeChangedEventArgs : EventArgs
    {
        public Point OldSize { get; set; }
        public Point NewSize { get; set; }
    }

    delegate void MovedEventHandler(object source, MovedEventArgs e);

    delegate void SizeChangedEventHandler(object source, SizeChangedEventArgs e);

    /// <summary>
    /// Appears on the screen. Has position and can get locatable.
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

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(X, Y, Width, Height);
            }
        }        

        public Point Position
        {
            get
            {
                return new Point(X, Y);
            }
            set
            {
                if ((_x != value.X || _y != value.Y) && OnMoved != null)
                {
                    Point oldV = new Point(_x, _y);
                    Point newV = new Point(value.X, value.Y);
                    MovedEventArgs mea = new MovedEventArgs();
                    mea.NewPosition = newV;
                    mea.OldPosition = oldV;
                    OnMoved(this, mea);
                    BoundsChanged(this, null);
                }
                _x = value.X;
                _y = value.Y;
            }
        }

        public Point Size
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
                    BoundsChanged(this, null);
                }
                _width = value.X;
                _height = value.Y;
            }
        }

        public int Width
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
                    BoundsChanged(this, null);
                }
                _width = value;
            }
        }

        public int Height
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
                    BoundsChanged(this, null);
                }
                _height = value;
            }
        }

        public bool IsOnGround { get; set; }

        public Vector2 BlockPosition
        {
            get
            {
                return new Vector2(BlockX, BlockY);
            }
            set
            {
                int newX = (int)Math.Round(value.X * Game1.BLOCK_WIDTH);
                int newY = (int)Math.Round(value.Y * Game1.BLOCK_HEIGHT);
                Position = new Point(newX, newY);
            }
        }

        public Vector2 BlockSize
        {
            get
            {
                return new Vector2(BlockWidth, BlockHeight);
            }
            set
            {
                int newX = (int)Math.Round(value.X * Game1.BLOCK_WIDTH);
                int newY = (int)Math.Round(value.Y * Game1.BLOCK_HEIGHT);
                Size = new Point(newX, newY);
            }
        }

        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                if (_x != value && OnMoved != null)
                {
                    Point oldV = new Point(_x, _y);
                    Point newV = new Point(value, _y);
                    MovedEventArgs mea = new MovedEventArgs();
                    mea.NewPosition = newV;
                    mea.OldPosition = oldV;
                    OnMoved(this, mea);
                    BoundsChanged(this, null);
                }
                _x = value;
            }
        }

        public float BlockX
        {
            get
            {
                return _x / (float) Game1.BLOCK_WIDTH;
            }
            set
            {
                int newX = (int) Math.Round(value * Game1.BLOCK_WIDTH);
                X = newX;
            }
        }

        public float BlockWidth
        {
            get
            {
                return _width / (float)Game1.BLOCK_WIDTH;
            }
            set
            {
                int newX = (int)Math.Round(value * Game1.BLOCK_WIDTH);
                Width = newX;
            }
        }

        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                if (_y != value && OnMoved != null)
                {
                    Point oldV = new Point(_x, _y);
                    Point newV = new Point(_x, value);
                    MovedEventArgs mea = new MovedEventArgs();
                    mea.NewPosition = newV;
                    mea.OldPosition = oldV;
                    OnMoved(this, mea);
                    BoundsChanged(this, null);
                }
                _y = value;
            }
        }

        public float BlockY
        {
            get
            {
                return _y / (float)Game1.BLOCK_HEIGHT;
            }
            set
            {
                int newY = (int) Math.Round(value * Game1.BLOCK_HEIGHT);
                Y = newY;
            }
        }

        public float BlockHeight
        {
            get
            {
                return _height / (float)Game1.BLOCK_HEIGHT;
            }
            set
            {
                int newY = (int)Math.Round(value * Game1.BLOCK_HEIGHT);
                Height = newY;
            }
        }

        public string Texture { get; set; }

        public Yuuki2TheGame.Graphics.Sprite Sprite
        {
            get
            {
                return new Graphics.Sprite(Position, Size, Texture);
            }
        }

        #endregion

        #region events

        public event MovedEventHandler OnMoved = null;

        public event SizeChangedEventHandler OnSizeChanged = null;

        public event EventHandler BoundsChanged = null;

        #endregion

        public ScreenEntity(Point size)
            : this(size, new Point(0, 0))
        {}

        public ScreenEntity(Point size, Point position)
            : this(size, position, null)
        {}

        public ScreenEntity(Point size, Point position, string texture)
        {
            _width = size.X;
            _height = size.Y;
            _x = position.X;
            _y = position.Y;
            Texture = texture;
            OnMoved = delegate(Object sender, MovedEventArgs mea)
            {
                if (IsOnGround && mea.NewPosition.Y != mea.OldPosition.Y)
                {
                    IsOnGround = false;
                }
            };
        }
    }
}
