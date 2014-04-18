using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuuki2TheGame.Physics;
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
    class Block : ScreenEntity
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

        public Block(int id, int mapx, int mapy) : base(new Point(Game1.BLOCK_WIDTH, Game1.BLOCK_HEIGHT))
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
            BlockPosition = new Vector2(mapx, mapy);
		}
    }
}
