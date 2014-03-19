using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    class Block : IUpdateable
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

        private bool _enabled = true;

        /// <summary>
        /// Gets the name of the texture that should be used to display this Block.
        /// </summary>
        /// <returns></returns>
        public string Texture { get; set; }

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

        public Block(int ID)
        {
            this.ID = ID;
            levelrequired = 0;
            blockhealth = 1;
        }
    }
}
