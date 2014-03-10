using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame
{
    class Block
    {
        private int levelrequired;
        private int blockhealth;
        private int id;

        public int LevelRequired
        {
            get { return levelrequired; }
            set { levelrequired = value; }
        }

        public int BlockHealth
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
