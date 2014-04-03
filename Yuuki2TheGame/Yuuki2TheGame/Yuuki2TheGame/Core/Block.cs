using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuuki2TheGame.Physics;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Yuuki2TheGame;

namespace Yuuki2TheGame.Core
{
    enum DirtType
    {
        LevelRequired = 1,
        Health = 100,
        Type = 1
        
    }
    enum GroundType
    {
       LevelRequired = 1,
       Health = 200,
       Type = 2
    }
    enum WoodType
    {
        LevelRequired = 2,
        Health = 250,
        Type = 3
    }
    class Block : IUpdateable, IPhysical, IPixelLocatable
    {

        private int levelrequired;
        private int blockhealth;
        private int id;
        private int _updateOrder = 0;
        private int type;

        public int Type
        {
            get { return type; }
            set { type = value; }
        }

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

        private int _pixelx;

        private int _pixely;

        public Point PixelLocation
        {
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
                    Body.Position = ConvertUnits.ToSimUnits(value.X + (Game1.BLOCK_WIDTH / 2), value.Y + (Game1.BLOCK_HEIGHT / 2));
                }
            }
        }

        public int PixelX
        {
            get
            {
                if (Body != null)
                {
                    PixelX = (int)Math.Round(ConvertUnits.ToDisplayUnits(Body.Position.X)) - (Game1.BLOCK_WIDTH / 2);
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
                    Body.Position = ConvertUnits.ToSimUnits(value + (Game1.BLOCK_WIDTH / 2), ConvertUnits.ToDisplayUnits(Body.Position.Y));
                }
            }
        }

        public int PixelY
        {
            get
            {
                if (Body != null)
                {
                    PixelY = (int)Math.Round(ConvertUnits.ToDisplayUnits(Body.Position.Y)) - (Game1.BLOCK_HEIGHT / 2);
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
                    Body.Position = ConvertUnits.ToSimUnits(ConvertUnits.ToDisplayUnits(Body.Position.X), value + (Game1.BLOCK_HEIGHT / 2));
                }
            }
        }

        public event MovedEventHandler OnMoved = null;

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(PixelX, PixelY, Game1.BLOCK_WIDTH, Game1.BLOCK_HEIGHT);
            }
        }

        private bool _enabled = true;

        /// <summary>
        /// Gets the name of the texture that should be used to display this Block.
        /// </summary>
        /// <returns></returns>
        public string Texture { get; set; }

        public Body Body { get; private set; }

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

        public event EventHandler<EventArgs> UpdateOrderChanged = null;

        public event EventHandler<EventArgs> EnabledChanged = null;

        
        public int LevelRequired
        {
            get { return levelrequired; }
            set { levelrequired = value; }
        }

        public void Update(GameTime gt)
        {
        }

        public int MiningHealth
        {
            get { return blockhealth; }
            set { blockhealth = value; } 
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public Block(int id, int mapx, int mapy)
        {
            this.ID = id;
            this.Texture = @"Tiles/dirt";

            //TODO Have correct implementation 
            if (ID <= 16){
                this.LevelRequired = (int)DirtType.LevelRequired;
                this.MiningHealth = (int)DirtType.Health;
                this.Type = (int)DirtType.Type;
            }
            
            if (ID > 16 && ID < 32){
                this.LevelRequired = (int)WoodType.LevelRequired;
                this.MiningHealth = (int)WoodType.Health;
                this.Type = (int)DirtType.Type;
            }

            if(ID >= 32){
                this.LevelRequired = (int)GroundType.LevelRequired;
                this.MiningHealth = (int)GroundType.Health;
                this.Type = (int)DirtType.Type;
			}
            PixelLocation = new Point(mapx * Game1.BLOCK_WIDTH, mapy * Game1.BLOCK_HEIGHT);
		}

        public void SetWorld(World w)
        {
            if (w != null)
            {
                Point pos = PixelLocation; // must get before setting body
                Body = BodyFactory.CreateRectangle(w, ConvertUnits.ToSimUnits(Game1.BLOCK_WIDTH), ConvertUnits.ToSimUnits(Game1.BLOCK_HEIGHT), 1f);
                Body.Position = ConvertUnits.ToSimUnits(pos.X + (Game1.BLOCK_WIDTH / 2), pos.Y + (Game1.BLOCK_WIDTH / 2));
                Body.IsStatic = true;
                Body.BodyType = BodyType.Static;
            }
            else
            {
                this.Body = null;
            }
        }
    }
}
