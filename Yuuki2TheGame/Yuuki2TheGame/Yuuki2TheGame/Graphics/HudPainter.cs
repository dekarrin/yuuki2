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
    /// Draws the heads-up display
    /// </summary>
    class HudPainter : Painter
    {
        private static HudPainter instance;

        private HudPainter(GraphicsDevice device)
            : base(device)
        { }

        public static HudPainter GetInstance(GraphicsDevice device)
        {
            if (HudPainter.instance == null)
            {
                HudPainter.instance = new HudPainter(device);
            }
            return HudPainter.instance;
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
