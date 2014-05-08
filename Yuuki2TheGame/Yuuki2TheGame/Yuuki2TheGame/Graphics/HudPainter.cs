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

        public const int SLOT_TEXT_PADDING = 3;

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
            CreateRect("$help_screen", 5 * (Game1.GAME_WIDTH / 6), 5 * (Game1.GAME_HEIGHT / 6));
            LoadTexture(@"Items\dirt");
            LoadTexture(@"Items\grass");
            LoadTexture(@"Items\stone");
            LoadTexture(@"Items\wood");
            LoadFont(@"Fonts\Inven");
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
            if (engine.InHelpScreen)
            {
                DrawHelp(batch);
            }
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

        private void DrawHelp(SpriteBatch batch)
        {
            Texture2D screen = TextureFromID("$help_screen");
            Rectangle dest = screen.Bounds;
            dest.X = Game1.GAME_WIDTH / 12;
            dest.Y = Game1.GAME_HEIGHT / 12;
            batch.Draw(screen, dest, Color.White);
            IList<string> helps = new List<string>();
            helps.Add("Yuuki 2: The Game - Help");
            helps.Add("");
            helps.Add("Click a block with no item selected to mine it!");
            helps.Add("Click empty space with a block selected to place it!");
            helps.Add("");
            helps.Add("");
            helps.Add("Key Bindings");
            helps.Add("");
            helps.Add("D, Left       - Move left");
            helps.Add("A, Right      - Move right");
            helps.Add("W, Up, Space  - Jump");
            helps.Add("Esc           - Open inventory");
            helps.Add("Scroll        - Change selected item");
            helps.Add("0-7           - Select quick slots");
            helps.Add("F1            - Toggle this help");
            helps.Add("F2            - Dismember player");
            helps.Add("F3            - Give items");
            helps.Add("F4            - Toggle Debug information");
            helps.Add("F5            - Toggle manual physics stepping");
            helps.Add("F6            - Record physics timing info");
            helps.Add("F7            - Respawn");
            helps.Add("F8            - Step physics (when manual stepping is on)");
            helps.Add("F9            - Change skin down");
            helps.Add("F10           - Change skin up");
            helps.Add("F11           - Change outfit down");
            helps.Add("F12           - Change outfit up");
            int x = Game1.GAME_WIDTH / 12 + 5;
            int y = Game1.GAME_HEIGHT / 12 + 5;
            foreach (string h in helps)
            {
                batch.DrawString(DefaultFont, h, new Vector2(x, y), Color.Black);
                y += 15;
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
            if (slot.Item != null)
            {
                Texture2D itemTex = TextureFromID(slot.Item.Texture);
                Rectangle itemRect = new Rectangle();
                itemRect.Width = itemRect.Height = SLOT_LENGTH / 2;
                itemRect.X = x + (SLOT_LENGTH / 2) - (itemRect.Width / 2);
                itemRect.Y = y + (SLOT_LENGTH / 2) - (itemRect.Height / 2);
                batch.Draw(itemTex, itemRect, Color.White);
                if (slot.Count > 1)
                {
                    SpriteFont f = FontFromID(@"Fonts\Inven");
                    Vector2 pos = new Vector2();
                    string cnt = slot.Count.ToString();
                    Vector2 size = f.MeasureString(cnt);
                    pos.X = x + SLOT_BORDER_WIDTH + SLOT_TEXT_PADDING;
                    pos.Y = y + SLOT_LENGTH - size.Y - SLOT_TEXT_PADDING;
                    batch.DrawString(f, slot.Count.ToString(), pos, Color.Black);
                }
            }
        }
    }
}
