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
        private int mininghealth;
        private int id;

        public int LevelRequired
        {
            get { return levelrequired; }
            set { levelrequired = value; }
        }

        public override virtual void Update(GameTime gt)
        {
            // do physics of block here
        }

        public int MiningHealth
        {
            get { return mininghealth; }
            set { mininghealth = value; }
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
            mininghealth = 1;
        }
    }
}
