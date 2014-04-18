using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Data
{
    [IgnoreFirst(2)]
    [DelimitedRecord(",")]
    public sealed class GameObjectData
    {
        public int ID;

        public string Name;

        public string Type;

        public int Level;

        public bool Stack;

        public int MaxStack;

        public bool Equipable;

        public int BlockDamage;

        public int Health;
    }
}
