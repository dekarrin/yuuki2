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
    /// Draws the background.
    /// </summary>
    class BackgroundPainter : Painter
    {
        private static BackgroundPainter instance;

        public static int SPRITE_HEIGHT = 124;

        public static int SPRITE_WIDTH = 512;

        public static double LAYER_OFFSET_TOP = 1.2;

        public static double LAYER_OFFSET_MID = 1.1;

        public static double LAYER_OFFSET_BOT = 1.05;

        public static int Y_START = 100;

        public static int LAYER_Y_OFFSET_TOP = 0;

        public static int LAYER_Y_OFFSET_MID = 90;

        public static int LAYER_Y_OFFSET_BOT = 180;

        private Engine engine;

        private int width;

        private int height;

        private BackgroundPainter(Engine game, int width, int height)
        {
            engine = game;
            this.width = width;
            this.height = height;
        }

        public static BackgroundPainter GetInstance(Engine game, int width, int height)
        {
            if (BackgroundPainter.instance == null)
            {
                BackgroundPainter.instance = new BackgroundPainter(game, width, height);
            }
            return BackgroundPainter.instance;
        }

        protected override void Init()
        { }

        protected override void Load()
        {
            LoadTexture(@"Backgrounds\plains");
            LoadTexture(@"Backgrounds\plains_fade");
            CreateRect("$backdrop", Game1.GAME_WIDTH, Game1.GAME_HEIGHT);
        }

        protected override void Unload()
        { }

        protected override void Paint(GameTime gameTime, SpriteBatch batch)
        {
            DrawLayer(batch, 0, LAYER_OFFSET_TOP, LAYER_Y_OFFSET_TOP);
            DrawLayer(batch, 1, LAYER_OFFSET_MID, LAYER_Y_OFFSET_MID);
            DrawLayer(batch, 2, LAYER_OFFSET_BOT, LAYER_Y_OFFSET_BOT);
            DrawFade(batch);
        }

        private void DrawFade(SpriteBatch batch)
        {
            Texture2D drop = TextureFromID("$backdrop");
            Rectangle dropDest = drop.Bounds;
            dropDest.Y = Y_START + LAYER_Y_OFFSET_BOT + SPRITE_HEIGHT + 28;
            batch.Draw(drop, dropDest, new Color(169, 121, 62));
            Texture2D fade = TextureFromID(@"Backgrounds\plains_fade");
            int offX = ((int)(engine.Camera.X * LAYER_OFFSET_BOT)) % SPRITE_WIDTH;
            int y = Y_START + LAYER_Y_OFFSET_BOT + SPRITE_HEIGHT - 2;
            Rectangle dest = new Rectangle(-offX, y, fade.Width, fade.Height);
            while (dest.X < engine.Camera.Width)
            {
                batch.Draw(fade, dest, Color.White);
                dest.X += SPRITE_WIDTH;
            }
        }

        private void DrawLayer(SpriteBatch batch, int sectionNum, double layerOffset, int layerYOffset)
        {
            Texture2D sheet = TextureFromID(@"Backgrounds\plains");
            Rectangle source = SectionToSourceRect(sectionNum);
            int offX = ((int)(engine.Camera.X * layerOffset)) % SPRITE_WIDTH;
            Rectangle dest = new Rectangle(-offX, Y_START + layerYOffset, source.Width, source.Height);
            while (dest.X < engine.Camera.Width)
            {
                batch.Draw(sheet, dest, source, Color.White);
                dest.X += SPRITE_WIDTH;
            }
        }

        private Rectangle SectionToSourceRect(int frame)
        {
            Rectangle spriteSheet = TextureFromID(@"Backgrounds\plains").Bounds;
            int sections = spriteSheet.Height / SPRITE_HEIGHT;
            if (frame >= sections)
            {
                frame = sections - 1;
            }
            Rectangle source = new Rectangle(0, frame * SPRITE_HEIGHT, spriteSheet.Width, SPRITE_HEIGHT);
            return source;
        }
    }
}
