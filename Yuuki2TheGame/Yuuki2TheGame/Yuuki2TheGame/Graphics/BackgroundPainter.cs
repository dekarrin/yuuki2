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
    /// Draws the background.
    /// </summary>
    class BackgroundPainter : Painter
    {
        private static BackgroundPainter instance;

        private Engine engine;

        private int width;

        private int height;

        private BackgroundPainter(Engine game, int width, int height)
        {
            engine = game;
            this.width = width;
            this.height = height;
        }

        public static BackgroundPainter GetInstance(Engine game, int width, int height)
        {
            if (BackgroundPainter.instance == null)
            {
                BackgroundPainter.instance = new BackgroundPainter(game, width, height);
            }
            return BackgroundPainter.instance;
        }

        protected override void Init()
        { }

        protected override void Load()
        { }

        protected override void Unload()
        { }

        protected override void Paint(GameTime gameTime, SpriteBatch batch)
        {
            Sprite bg = engine.GetBackground(width, height);
            bg.Texture = TextureFromID(bg.TextureID);
            batch.Draw(bg.Texture, bg.Destination, bg.Source, Color.Pink);

        }
    }
}
