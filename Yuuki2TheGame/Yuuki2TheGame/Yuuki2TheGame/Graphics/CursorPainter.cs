using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Yuuki2TheGame.Graphics
{
    /// <summary>
    /// Draws the cursor on the screen.
    /// </summary>
    class CursorPainter : Painter
    {
        private static CursorPainter instance;

        private CursorPainter()
        { }

        public static CursorPainter GetInstance()
        {
            if (CursorPainter.instance == null)
            {
                CursorPainter.instance = new CursorPainter();
            }
            return CursorPainter.instance;
        }

        protected override void Init()
        { }

        protected override void Load()
        { }

        protected override void Unload()
        { }

        protected override void Paint(GameTime gameTime, SpriteBatch batch)
        {
            MouseState currentMouse = Mouse.GetState();
            Vector2 pos = new Vector2(currentMouse.X, currentMouse.Y);
            batch.Draw(DefaultTexture, pos, Color.White);
        }
    }
}
