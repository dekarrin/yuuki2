using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Yuuki2TheGame.Graphics
{
    /// <summary>
    /// Draws the background.
    /// </summary>
    class BackgroundPainter : Painter
    {
        private static BackgroundPainter instance;

        private BackgroundPainter(GraphicsDevice device)
            : base(device)
        { }

        public static BackgroundPainter GetInstance(GraphicsDevice device)
        {
            if (BackgroundPainter.instance == null)
            {
                BackgroundPainter.instance = new BackgroundPainter(device);
            }
            return BackgroundPainter.instance;
        }

        protected override void Init()
        { }

        protected override void Load(ContentManager content)
        { }

        protected override void Unload(ContentManager content)
        { }

        protected override void Paint(GameTime gameTime, SpriteBatch batch)
        { }
    }
}
