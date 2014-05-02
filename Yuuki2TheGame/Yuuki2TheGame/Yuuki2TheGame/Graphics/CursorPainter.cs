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
    /// Draws the cursor on the screen.
    /// </summary>
    class CursorPainter : Painter
    {
        private static CursorPainter instance;

        private CursorPainter(GraphicsDevice device)
            : base(device)
        { }

        public static CursorPainter GetInstance(GraphicsDevice device)
        {
            if (CursorPainter.instance == null)
            {
                CursorPainter.instance = new CursorPainter(device);
            }
            return CursorPainter.instance;
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
