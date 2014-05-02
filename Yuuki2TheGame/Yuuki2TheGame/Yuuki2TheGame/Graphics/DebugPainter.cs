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
    /// Draws debug messages
    /// </summary>
    class DebugPainter : Painter
    {
        private static DebugPainter instance;

        private DebugPainter(GraphicsDevice device)
            : base(device)
        { }

        public static DebugPainter GetInstance(GraphicsDevice device)
        {
            if (DebugPainter.instance == null)
            {
                DebugPainter.instance = new DebugPainter(device);
            }
            return DebugPainter.instance;
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
