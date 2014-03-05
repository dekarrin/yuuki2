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
        private int mininghealth;
        private Point position;

        public int LevelRequired
        {
            get { return levelrequired; }
            set { levelrequired = value; }
        }

        public int MiningHealth
        {
            get { return mininghealth; }
            set { mininghealth = value; }
        }
        public Point Position
        {
            get { return position; }
            set { position = value; }
        }


        public Block(int BlockHealth, int ItemLevel, Point MapPosition)
        {
            MiningHealth = BlockHealth;
            LevelRequired = ItemLevel;
            Position = MapPosition;
        }

        public void Destroy(Point MapPosition)
        {
            if (position.Equals(MapPosition)){
                position.X = 0;
                position.Y = 0;
            }
        }

        public void BlockPlaced(Point MapPosition)
        {
            if(position.X == 0 || position.Y == 0){
                position = MapPosition;
            }
        }
    }
}
