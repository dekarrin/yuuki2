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

        public Engine(Point size)
        {
            _map = new Map(size.X, size.Y);
        }

        public void Update(GameTime gameTime)
        {
            foreach (GameCharacter c in _characters)
            {
                c.Update(gameTime);
            }
            foreach (Item i in _items)
            {
                i.Update(gameTime);
            }
            _map.Update(gameTime);
        }

        public Block[,] MapData
        {
            get
            {
                return _map.GetView(); // TODO Implement MapData property
            }
        }

        public IList<GameCharacter> Characters
        {
            get
            {
                return _characters.AsReadOnly();
            }
        }

        public IList<Item> Items
        {
            get
            {
                return _items.AsReadOnly();
            }
        }
    }
}
