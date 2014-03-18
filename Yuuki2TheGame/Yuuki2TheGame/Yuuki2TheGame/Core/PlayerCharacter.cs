using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    class PlayerCharacter : GameCharacter
    {
        public PlayerCharacter(string name, Point pixelLocation, int health, int baseAttack, int baseArmor) : base(name, pixelLocation, health, baseAttack, baseArmor)
        {
        }
    }
}
