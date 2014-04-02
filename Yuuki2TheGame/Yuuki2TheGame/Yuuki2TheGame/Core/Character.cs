using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace Yuuki2TheGame.Core
{
    class InteractEventArgs : EventArgs
    {
        public GameCharacter Actor { get; set; }
    }

    /// <summary>
    /// NOTE: Pixel coordinates is relative to lower left of character!
    /// </summary>
    class GameCharacter : IUpdateable, IPixelLocatable, Yuuki2TheGame.Physics.IPhysical
    {

        private int _updateOrder = 0;

        public int UpdateOrder
        {
            get
            {
                return _updateOrder;
            }
            set
            {
                bool diff = _updateOrder != value;
                _updateOrder = value;
                if (diff && UpdateOrderChanged != null)
                {
                    UpdateOrderChanged(this, new EventArgs());
                }
            }
        }

        private bool _enabled = true;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                bool diff = _enabled != value;
                _enabled = value;
                if (diff && EnabledChanged != null)
                {
                    EnabledChanged(this, new EventArgs());
                }
            }
        }

        public Body Body { get; private set; }

        public void SetWorld(World w)
        {
            if (w != null)
            {
                Point currentPos = PixelLocation; // must get before setting body
                this.Body = BodyFactory.CreateRectangle(w, ConvertUnits.ToSimUnits(Game1.BLOCK_WIDTH), ConvertUnits.ToSimUnits(Game1.BLOCK_HEIGHT), 1f);
                this.Body.Position = ConvertUnits.ToSimUnits(currentPos.X, currentPos.Y);
                this.Body.BodyType = BodyType.Dynamic;
            }
            else
            {
                this.Body = null;
            }
        }

        public event EventHandler<EventArgs> UpdateOrderChanged = null;

        public event EventHandler<EventArgs> EnabledChanged = null;
        
        private int _health = 0;

        private int _pixelx;

        private int _pixely;

        public Point PixelLocation {
            get
            {
                return new Point(PixelX, PixelY);
            }
            private set
            {
                if ((_pixelx != value.X || _pixely != value.Y) && OnMoved != null)
                {
                    Point oldV = new Point(_pixelx, _pixely);
                    Point newV = new Point(value.X, value.Y);
                    MovedEventArgs mea = new MovedEventArgs();
                    mea.NewLocation = newV;
                    mea.OldLocation = oldV;
                    OnMoved(this, mea);
                }
                _pixelx = value.X;
                _pixely = value.Y;
                if (Body != null)
                {
                    Body.Position = ConvertUnits.ToSimUnits(value.X, value.Y);
                }
            }
        }

        public int PixelX
        {
            get
            {
                if (Body != null)
                {
                    PixelX = (int)Math.Round(ConvertUnits.ToDisplayUnits(Body.Position.X));
                }
                return _pixelx;
            }
            set
            {
                if (_pixelx != value && OnMoved != null)
                {
                    Point oldV = new Point(_pixelx, _pixely);
                    Point newV = new Point(value, _pixely);
                    MovedEventArgs mea = new MovedEventArgs();
                    mea.NewLocation = newV;
                    mea.OldLocation = oldV;
                    OnMoved(this, mea);
                }
                _pixelx = value;
                if (Body != null)
                {
                    Body.Position = ConvertUnits.ToSimUnits(value, ConvertUnits.ToDisplayUnits(Body.Position.Y));
                }
            }
        }

        public int PixelY
        {
            get
            {
                if (Body != null)
                {
                    PixelY = (int)Math.Round(ConvertUnits.ToDisplayUnits(Body.Position.Y));
                }
                return _pixely;
            }
            set
            {
                if (_pixely != value && OnMoved != null)
                {
                    Point oldV = new Point(_pixelx, _pixely);
                    Point newV = new Point(_pixelx, value);
                    MovedEventArgs mea = new MovedEventArgs();
                    mea.NewLocation = newV;
                    mea.OldLocation = oldV;
                    OnMoved(this, mea);
                }
                _pixely = value;
                if (Body != null)
                {
                    Body.Position = ConvertUnits.ToSimUnits(ConvertUnits.ToDisplayUnits(Body.Position.X), value);
                }
            }
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(PixelX, PixelY - Height, Width, Height);
            }
        }

        public int MaxHealth { get; private set; }

        public int Health {
            get {
                return _health;
            }
            set {
                // range check value
                _health = Math.Max(Math.Min(MaxHealth, value), 0);
                if (_health == 0)
                {
                    if (OnDeath != null)
                    {
                        OnDeath(this, null);
                    }
                }
            }
        }

        public string Name { get; set; }

        public int BaseAttack { get; set; }

        public int BaseArmor { get; set; }

        public object Inventory { get; set; }

        public delegate void DeathHandler(object source, EventArgs e);

        public event DeathHandler OnDeath = null;

        public delegate void InteractHandler(object source, InteractEventArgs e);

        public event InteractHandler OnInteract = null;

        public event MovedEventHandler OnMoved = null;

        public string Texture { get; private set; }

        public GameCharacter(string name, Point pixelLocation, Point size, int health, int baseAttack, int baseArmor)
        {
            this.Name = name;
            this.PixelLocation = pixelLocation;
            this.Width = size.X;
            this.Height = size.Y;
            this.MaxHealth = health;
            this.Health = health;
            this.BaseArmor = baseArmor;
            this.BaseAttack = baseAttack;
        }

        /// <summary>
        /// Interacts with this character.
        /// </summary>
        /// <param name="interactor"></param>
        public void Interact(GameCharacter interactor)
        {
            if (OnInteract != null)
            {
                InteractEventArgs e = new InteractEventArgs();
                e.Actor = interactor;
                OnInteract(this, e);
            }
        }

        /// <summary>
        /// Called by game engine; tells instance to update self.
        /// </summary>
        /// <param name="ts">Amount of time passed since last update.</param>
        public void Update(GameTime ts)
        {
            
        }

        public void MoveLeft()
        {
            Body.ApplyLinearImpulse(new Vector2(-0.25f, 0));
        }

        public void MoveRight()
        {
            Body.ApplyLinearImpulse(new Vector2(0.25f, 0));
        }

        public void Jump()
        {
            Body.ApplyLinearImpulse(new Vector2(0, -10));
        }


    }
}
