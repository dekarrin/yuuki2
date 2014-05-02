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
    /// Paints blocks, entities, and characters.
    /// </summary>
    class WorldPainter : Painter
    {
        private static WorldPainter instance;

        private WorldPainter(GraphicsDevice device)
            : base(device)
        { }

        public static WorldPainter GetInstance(GraphicsDevice device)
        {
            if (WorldPainter.instance == null)
            {
                WorldPainter.instance = new WorldPainter(device);
            }
            return WorldPainter.instance;
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
