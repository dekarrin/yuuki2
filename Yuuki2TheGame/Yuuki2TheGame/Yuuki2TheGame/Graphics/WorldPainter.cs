using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Yuuki2TheGame.Core;

namespace Yuuki2TheGame.Graphics
{
    /// <summary>
    /// Paints blocks, entities, and characters.
    /// </summary>
    class WorldPainter : Painter
    {
        private static WorldPainter instance;

        private Engine engine;

        private int xBlocks;

        private int yBlocks;

        private WorldPainter(Engine game, int xBlocks, int yBlocks)
        {
            this.engine = game;
            this.xBlocks = xBlocks;
            this.yBlocks = yBlocks;
        }

        public static WorldPainter GetInstance(Engine game, int xBlocks, int yBlocks)
        {
            if (WorldPainter.instance == null)
            {
                WorldPainter.instance = new WorldPainter(game, xBlocks, yBlocks);
            }
            return WorldPainter.instance;
        }

        protected override void Init()
        { }

        protected override IDictionary<string, Texture2D> Load(ContentManager content)
        {
            return new Dictionary<string, Texture2D>();
        }

        protected override void Unload(ContentManager content)
        { }

        protected override void Paint(GameTime gameTime, SpriteBatch batch)
        {
            IList<Sprite> tiles = engine.GetView(xBlocks, yBlocks);
            IList<Sprite> chars = engine.GetCharacters();
            ConvertIDs(tiles);
            ConvertIDs(chars);
            foreach (Sprite sp in tiles)
            {
                batch.Draw(sp.Texture, sp.Destination, sp.Source, Color.White);
            }
            foreach (Sprite sp in chars)
            {
                batch.Draw(sp.Texture, sp.Destination, sp.Source, Color.White);
            }
        }
    }
}
