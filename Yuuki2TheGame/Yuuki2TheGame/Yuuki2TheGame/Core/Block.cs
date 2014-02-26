using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame
{
    class Block
    {
        int LevelRequired;
        int MiningResistance;
        Point Position;
        
        public Block(int BlockHealth, int ItemLevel, Point Map)
        {
            MiningResistance = BlockHealth;
            LevelRequired = ItemLevel;
            Position = Map;
        }

        public void Destroy(Point MapPosition)
        {
            if (Position.Equals(MapPosition)){
                Position.X = 0;
                Position.Y = 0;
            }
        }

        public void BlockPlaced(Point MapPosition)
        {
            if(Position.X == null || Position.Y == null){
                Position = MapPosition;
            }
        }
    }
}
