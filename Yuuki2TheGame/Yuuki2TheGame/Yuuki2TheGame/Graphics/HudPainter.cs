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
    /// Draws the heads-up display
    /// </summary>
    class HudPainter : Painter
    {
        private static HudPainter instance;

        public const int INVEN_SLOT_LENGTH = 30;

        private Engine engine;

        private int width;

        private int height;

        private HudPainter(Engine engine, int width, int height)
        {
            this.engine = engine;
            this.width = width;
            this.height = height;
        }

        public static HudPainter GetInstance(Engine game, int width, int height)
        {
            if (HudPainter.instance == null)
            {
                HudPainter.instance = new HudPainter(game, width, height);
            }
            return HudPainter.instance;
        }

        protected override void Init()
        { }

        protected override IDictionary<string, Texture2D> Load(ContentManager content)
        {
            IDictionary<string, Texture2D> dict = new Dictionary<string, Texture2D>();
            dict["$overlay"] = CreateRectTexture(width, height);
            dict["$slot"] = CreateRectTexture(INVEN_SLOT_LENGTH, INVEN_SLOT_LENGTH);
            return dict;
        }

        protected override void Unload(ContentManager content)
        { }

        protected override void Paint(GameTime gameTime, SpriteBatch batch)
        {
            Texture2D fullScreenSolid = IDToTexture("$overlay");
            Texture2D invenSlotSolid = IDToTexture("$slot");
            if (engine.InInventoryScreen)
            {
                batch.Draw(fullScreenSolid, fullScreenSolid.Bounds, new Color(0, 0, 0, 128));
            }
            IList<InventorySlot> quicks = engine.Player.Inventory.QuickSlots;
            Rectangle drawRect = invenSlotSolid.Bounds;
            drawRect.Y = 10;
            drawRect.X = 0;
            for (int i = 0; i < quicks.Count; i++)
            {
                drawRect.X += 10;
                batch.Draw(invenSlotSolid, drawRect, new Color(217, 154, 154));
                drawRect.X += INVEN_SLOT_LENGTH;
            }
        }
    }
}
