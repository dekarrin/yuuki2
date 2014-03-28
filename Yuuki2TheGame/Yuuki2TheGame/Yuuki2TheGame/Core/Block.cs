using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame
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
    class Block : IUpdateable
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

        public Block(int id)
        {
            this.ID = id;


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
        }
    }
}
