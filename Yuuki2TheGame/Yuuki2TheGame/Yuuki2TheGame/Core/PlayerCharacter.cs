using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    class PlayerCharacter : GameCharacter
    {
        public const int WIDTH = Game1.BLOCK_WIDTH * 1;
        public const int HEIGHT = Game1.BLOCK_HEIGHT * 2;

        public PlayerCharacter(string name, Point pixelLocation, int health, int baseAttack, int baseArmor) : base(name, pixelLocation, new Point(WIDTH, HEIGHT), health, baseAttack, baseArmor)
        {
        }
    }
}
