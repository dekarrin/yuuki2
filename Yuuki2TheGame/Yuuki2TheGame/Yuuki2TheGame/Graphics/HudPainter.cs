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

        public const int SLOT_BORDER_WIDTH = 2;

        public const int SLOT_SPACING = 2;

        public static readonly Color SLOT_BORDER_COLOR = new Color(24, 24, 24);

        public static readonly Color QUICK_SLOT_COLOR = new Color(217, 154, 154);

        public static readonly Color ACTIVE_SLOT_COLOR = new Color(255, 122, 122);

        public static readonly Color OVERLAY_SLOT_COLOR = new Color(232, 255, 217);

        public static readonly Color OVERLAY_COLOR = new Color(0, 0, 0, 128);

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

        protected override void Load()
        {
            CreateRect("$overlay", width, height);
            CreateRect("$slot", SLOT_LENGTH - (SLOT_BORDER_WIDTH * 2), SLOT_LENGTH - (SLOT_BORDER_WIDTH * 2));
            CreateRect("$slot_border", SLOT_LENGTH, SLOT_LENGTH);
        }

        protected override void Unload()
        { }

        protected override void Paint(GameTime gameTime, SpriteBatch batch)
        {
            if (engine.InInventoryScreen)
            {
                DrawOverlay(batch);
            }
            DrawQuickSlots(batch);
        }

        private void DrawOverlay(SpriteBatch batch)
        {
            IList<InventorySlot> quicks = engine.Player.Inventory.QuickSlots;
            IList<InventorySlot> slots = engine.Player.Inventory.Slots;
            Texture2D overlay = TextureFromID("$overlay");
            Rectangle overlayRect = new Rectangle(0, 0, Game1.GAME_WIDTH, Game1.GAME_HEIGHT);
            batch.Draw(overlay, overlayRect, OVERLAY_COLOR);
            for (int i = engine.Player.Inventory.QuickSlotsCount; i < slots.Count; i++)
            {
                int x = SLOT_SPACING + ((i % quicks.Count) * (SLOT_LENGTH + SLOT_SPACING));
                int y = SLOT_SPACING + ((i / quicks.Count) * (SLOT_LENGTH + SLOT_SPACING));
                DrawSlot(batch, slots[i], x, y, OVERLAY_SLOT_COLOR);
            }
        }

        private void DrawQuickSlots(SpriteBatch batch)
        {
            IList<InventorySlot> slots = engine.Player.Inventory.QuickSlots;
            int x = 0;
            for (int i = 0; i < slots.Count; i++)
            {
                x += SLOT_SPACING;
                DrawSlot(batch, slots[i], x, SLOT_SPACING, QUICK_SLOT_COLOR);
                x += SLOT_LENGTH;
            }
        }

        private void DrawSlot(SpriteBatch batch, InventorySlot slot, int x, int y, Color color)
        {
            Texture2D invenSlotMain = TextureFromID("$slot");
            Texture2D invenSlotBorder = TextureFromID("$slot_border");
            Rectangle slotRect = invenSlotMain.Bounds;
            Rectangle borderRect = invenSlotBorder.Bounds;
            borderRect.X = x;
            borderRect.Y = y;
            slotRect.X = x + SLOT_BORDER_WIDTH;
            slotRect.Y = y + SLOT_BORDER_WIDTH;
            batch.Draw(invenSlotBorder, borderRect, SLOT_BORDER_COLOR);
            Color insideColor = (slot.IsActive) ? ACTIVE_SLOT_COLOR : color;
            batch.Draw(invenSlotMain, slotRect, insideColor);
        }
    }
}
