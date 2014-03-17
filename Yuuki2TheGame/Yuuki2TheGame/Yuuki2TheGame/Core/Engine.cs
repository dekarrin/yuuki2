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

        private Vector2 spawn;

        public static Camera Camera { get; set; }           // Made static so it can be accesible from other areas of the game easily  -CA

        public Engine(Point size)
        {
            
            _map = new Map(size.X, size.Y);
            spawn = new Vector2(1, 1);
            // temp vars until we can meet with the team
            Player = new PlayerCharacter("Becky", spawn, 100, 10, 10);
            _characters.Add(Player);
            Camera = new Camera(Player, new Vector2(50, 50));

            Player.CreateSprite();  // Create the Player's camera-bound sprite image
            
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

        /// <summary>
        /// Gets all tiles that need to be displayed.
        /// </summary>
        /// <param name="numX">Number of tiles wide.</param>
        /// <param name="numY">Number of tiles high.</param>
        /// <param name="tileWidth">Width of a tile.</param>
        /// <param name="tileHeight">Height of a tile.</param>
        /// <returns></returns>
        public IList<Tile> GetView(int numX, int numY, int tileWidth, int tileHeight)
        {
            Point origin = Camera.Coordinates;
            Vector2 offsets = Camera.Offsets;
            IList<Tile> view = new List<Tile>();
            for (int i = 0; i < numX; i++)
            {
                for (int j = 0; j < numY; j++)
                {
                    Tile t = new Tile();
                    t.Block = Map[i, j];
                    t.Location = offsets + new Vector2(i * tileWidth, j * tileHeight);
                    view.Add(t);
                }
            }
            return view;
        }

        public Block[,] Map
        {
            get
            {
                return _map.GetView(); // TODO Implement MapData property
            }
        }

        public PlayerCharacter Player { get; private set; }

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
