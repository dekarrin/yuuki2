using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Yuuki2TheGame.Graphics;

namespace Yuuki2TheGame.Core
{
    class Engine
    {
        private List<GameCharacter> _characters = new List<GameCharacter>();

        private List<Item> _items = new List<Item>();

        private Map _map;

        private Point spawn;

        public Camera Camera { get; set; }

        public Engine(Point size)
        {
            _map = new Map(size.X, size.Y);
            spawn = new Point(0, (size.Y / 2) * Game1.BLOCK_HEIGHT);
            // temp vars until we can meet with the team
            Player = new PlayerCharacter("Becky", spawn, 100, 10, 10);
            _characters.Add(Player);
            Camera = new Camera(Player, new Point(-20, 0));
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
        public IList<Sprite> GetView(int numX, int numY, int tileWidth, int tileHeight)
        {
            Point coords = Camera.Coordinates;
            Point offsets = Camera.Offsets;
            IList<Sprite> view = new List<Sprite>();
            for (int i = 0; i < numX; i++)
            {
                for (int j = 0; j < numY; j++)
                {
                    Point tilecoords = new Point(coords.X + i, coords.Y + j);
                    if (tilecoords.X >= 0 && tilecoords.Y >= 0 && _map.BlockAt(tilecoords) != null)
                    {
                        Point position = new Point(i * tileWidth - offsets.X, j * tileHeight - offsets.Y);
                        Sprite spr = new Sprite(position, new Point(tileWidth, tileHeight), _map.BlockAt(tilecoords).Texture);
                        view.Add(spr);
                    }
                }
            }
            return view;
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

        public Sprite GetBackground(int screenWidth, int screenHeight)
        {
            int x = Math.Abs(Math.Min(Camera.Location.X, 0));
            int y = Math.Abs(Math.Min(Camera.Location.Y, 0));
            int width = screenWidth - x;
            int height = screenHeight - y;
            // Above will need to be changed if we want to make worlds that are smaller than the screen
            Sprite spr = new Sprite(new Point(x, y), new Point(width, height), null);
            // TODO: Correct position based on camera
            return spr;
        }
    }
}
