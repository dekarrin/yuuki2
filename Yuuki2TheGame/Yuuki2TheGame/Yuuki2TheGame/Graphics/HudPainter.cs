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

        public const int SLOT_LENGTH = 40;

        public const int SLOT_BORDER = 2;

        public const int SLOT_SPACING = 2;

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
            dict["$slot"] = CreateRectTexture(SLOT_LENGTH - (SLOT_BORDER * 2), SLOT_LENGTH - (SLOT_BORDER * 2));
            dict["$slot_border"] = CreateRectTexture(SLOT_LENGTH, SLOT_LENGTH);
            return dict;
        }

        protected override void Unload(ContentManager content)
        { }

        protected override void Paint(GameTime gameTime, SpriteBatch batch)
        {
            Texture2D fullScreenSolid = IDToTexture("$overlay");
            Texture2D invenSlotSolid = IDToTexture("$slot");
            Texture2D invenSlotBorder = IDToTexture("$slot_border");
            if (engine.InInventoryScreen)
            {
                batch.Draw(fullScreenSolid, fullScreenSolid.Bounds, new Color(0, 0, 0, 128));
            }
            IList<InventorySlot> quicks = engine.Player.Inventory.QuickSlots;
            Rectangle drawRect = invenSlotBorder.Bounds;
            drawRect.Y = SLOT_SPACING;
            drawRect.X = 0;
            for (int i = 0; i < quicks.Count; i++)
            {
                drawRect.X += SLOT_SPACING;
                Rectangle innerDraw = new Rectangle(drawRect.X + SLOT_BORDER, drawRect.Y + SLOT_BORDER, drawRect.Width - (SLOT_BORDER * 2), drawRect.Height - (SLOT_BORDER * 2));
                batch.Draw(invenSlotBorder, drawRect, new Color(24, 24, 24));
                Color color = new Color(217, 154, 154);
                if (quicks[i].IsActive)
                {
                    color = new Color(255, 122, 122);
                }
                batch.Draw(invenSlotSolid, innerDraw, color);
                drawRect.X += SLOT_LENGTH;
            }
        }
    }
}
