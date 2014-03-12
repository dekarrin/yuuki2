using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    class PlayerCharacter : GameCharacter
    {
        public PlayerCharacter(string name, Vector2 location, int health, int baseAttack, int baseArmor) : base(name, location, health, baseAttack, baseArmor)
        {
        }
    }
}
