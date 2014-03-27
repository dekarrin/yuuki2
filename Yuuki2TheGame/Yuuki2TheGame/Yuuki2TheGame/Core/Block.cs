using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuuki2TheGame.Physics;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace Yuuki2TheGame
{
    enum BlockType
    {
        Dirt,
        Stone,
        Copper,
        Iron,
        Sand,
        Ice,
        Swamp,
        Wood,
        Smoothstone,
        Brick,
        Titanite,
        Twinkling,
    }

    enum PhysicsType
    {
        Liquid,
        Solid,
    }
    class Block : IUpdateable, IPhysical
    {

        private int levelrequired;
        private int blockhealth;
        private int id;

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

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X * Game1.BLOCK_WIDTH, ((int)Position.Y * Game1.BLOCK_HEIGHT) - Game1.BLOCK_HEIGHT, Game1.BLOCK_WIDTH, Game1.BLOCK_HEIGHT);
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
            // do physics of block here
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

        public Block(int ID, int mapx, int mapy)
        {
            this.ID = ID;
            levelrequired = 0;
            blockhealth = 1;
            Position = new Vector2(mapx, mapy);
        }

        private Vector2 _position;

        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                if (this.Body != null)
                {
                    Body.Position = ConvertUnits.ToSimUnits(Position);
                }
            }
        }

        public void SetWorld(World w)
        {
            if (w != null)
            {
                this.Body = BodyFactory.CreateRectangle(w, ConvertUnits.ToSimUnits(Game1.BLOCK_WIDTH), ConvertUnits.ToSimUnits(Game1.BLOCK_HEIGHT), 1f);
                this.Body.Position = ConvertUnits.ToSimUnits(Position);
                this.Body.IsStatic = true;
            }
            else
            {
                this.Body = null;
            }
        }
    }
}
