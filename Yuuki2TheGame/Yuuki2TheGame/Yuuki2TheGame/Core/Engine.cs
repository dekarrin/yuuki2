using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    class Engine
    {
        private List<GameCharacter> _characters = new List<GameCharacter>();

        private List<Item> _items = new List<Item>();

        private Map _map;

        public Engine()
        {

        }

        public void Update(GameTime gametime)
        {

        }

        public Block[,] MapData
        {
            get
            {
                return null; // TODO Implement MapData property
            }
        }

        public List<GameCharacter> Characters
        {
            get
            {
                return null; // TODO Implement Character property
            }
        }

        public List<Item> Items
        {
            get
            {
                return null; // TODO Implement Items property
            }
        }
    }
}
