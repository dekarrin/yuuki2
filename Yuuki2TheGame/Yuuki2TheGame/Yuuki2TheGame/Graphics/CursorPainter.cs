using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Yuuki2TheGame.Core;

namespace Yuuki2TheGame.Graphics
{
    /// <summary>
    /// Draws the cursor on the screen.
    /// </summary>
    class CursorPainter : Painter
    {
        private static CursorPainter instance;

        private readonly Engine engine;

        public static readonly Color CURSOR_COLOR = new Color(0xff, 0x52, 0x7a);

        public const int ITEM_LENGTH = 15;

        public const int ITEM_OFFSET = 5;

        private CursorPainter(Engine gameEngine)
        {
            this.engine = gameEngine;
        }

        public static CursorPainter GetInstance(Engine gameEngine)
        {
            if (CursorPainter.instance == null)
            {
                CursorPainter.instance = new CursorPainter(gameEngine);
            }
            return CursorPainter.instance;
        }

        protected override void Init()
        { }

        protected override void Load()
        {
            LoadTexture("cursor");
            LoadTexture(@"Items\dirt");
            LoadTexture(@"Items\grass");
            LoadTexture(@"Items\stone");
            LoadTexture(@"Items\wood");
        }

        protected override void Unload()
        { }

        protected override void Paint(GameTime gameTime, SpriteBatch batch)
        {
            MouseState currentMouse = Mouse.GetState();
            Vector2 pos = new Vector2(currentMouse.X, currentMouse.Y);
            Texture2D tex = TextureFromID("cursor");
            batch.Draw(tex, pos, CURSOR_COLOR);
            InventorySlot slot = engine.Player.Inventory.ActiveSlot;
            if (slot.Item != null)
            {
                Texture2D itemTex = TextureFromID(slot.Item.Texture);
                Rectangle itemRect = new Rectangle((int)pos.X + ITEM_OFFSET, (int)pos.Y + ITEM_OFFSET, ITEM_LENGTH, ITEM_LENGTH);
                batch.Draw(itemTex, itemRect, Color.White);
            }
        }
    }
}
