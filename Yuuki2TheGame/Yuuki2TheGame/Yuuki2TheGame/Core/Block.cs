using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame
{
    class Block : IUpdateable
    {

        private int levelrequired;
        private int blockhealth;
        private int id;

        // Sprite Information;
        private int blockSpriteID;  // the spritesheet id that serves as a basis for all drawcalls of that block type   -CA




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

        public event EventHandler<EventArgs> UpdateOrderChanged = null;

        public event EventHandler<EventArgs> EnabledChanged = null;


        public int SpriteID
        {
            get { return blockSpriteID; }
            set { blockSpriteID = value; }
        }

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
