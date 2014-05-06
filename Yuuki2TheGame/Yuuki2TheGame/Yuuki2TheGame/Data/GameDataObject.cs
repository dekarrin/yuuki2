using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace Yuuki2TheGame.Data
{
    [IgnoreFirst(1)]
    [DelimitedRecord(",")]
    public sealed class GameDataObject
    {
        public int ID;

        public string Name;

        public int LevelRequired;

        public int Health;

        public string FilePath;
    }
}
