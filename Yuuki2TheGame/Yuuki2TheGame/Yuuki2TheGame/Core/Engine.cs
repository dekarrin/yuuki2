﻿using System;
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
        public IList<Tile> GetView(int numX, int numY, int tileWidth, int tileHeight)
        {
            Point coords = Camera.Coordinates;
            Point offsets = Camera.Offsets;
            IList<Tile> view = new List<Tile>();
            for (int i = 0; i < numX; i++)
            {
                for (int j = 0; j < numY; j++)
                {
                    Point tilecoords = new Point(coords.X + i, coords.Y + j);
                    if (tilecoords.X >= 0 && tilecoords.Y >= 0 && _map.BlockAt(tilecoords) != null)
                    {
                        Tile t = new Tile();
                        t.Block = _map.BlockAt(tilecoords);
                        t.Location = new Vector2(i * tileWidth - offsets.X, j * tileHeight - offsets.Y);
                        view.Add(t);
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
    }
}
