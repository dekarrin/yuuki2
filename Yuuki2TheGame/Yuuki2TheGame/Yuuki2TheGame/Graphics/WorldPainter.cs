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
    /// Paints blocks, entities, and characters.
    /// </summary>
    class WorldPainter : Painter
    {
        private static WorldPainter instance;

        public const int TILE_LENGTH = 32;

        private Engine engine;

        private WorldPainter(Engine game)
        {
            this.engine = game;
        }

        public static WorldPainter GetInstance(Engine game)
        {
            if (WorldPainter.instance == null)
            {
                WorldPainter.instance = new WorldPainter(game);
            }
            return WorldPainter.instance;
        }

        protected override void Init()
        { }

        protected override void Load()
        {
            LoadTexture(@"Tiles\wood");
            LoadTexture(@"Tiles\stone");
            LoadTexture(@"Tiles\dirt");
            LoadTexture(@"Tiles\grass");
            LoadTexture(@"Tiles\damage");
        }

        protected override void Unload()
        { }

        protected override void Paint(GameTime gameTime, SpriteBatch batch)
        {
            IList<Sprite> tiles = engine.GetView();
            IList<Sprite> chars = engine.GetCharacters();
            ConvertIDs(tiles);
            ConvertIDs(chars);
            foreach (Sprite sp in tiles)
            {
                DrawBlock(batch, sp);
            }
            foreach (Sprite sp in chars)
            {
                batch.Draw(sp.Texture, sp.Destination, sp.Source, Color.White);
            }
        }

        private void DrawBlock(SpriteBatch batch, Sprite sp)
        {
            batch.Draw(sp.Texture, sp.Destination, sp.Source, Color.White);
            Block b = (Block)sp.Creator;
            float percentDamage = 1 - (b.Health / (float)b.MaxHealth);
            Rectangle damageSourceRect = DamageToSourceRect(percentDamage);
            Texture2D damageSprites = TextureFromID(@"Tiles\damage");
            batch.Draw(damageSprites, sp.Destination, damageSourceRect, Color.White);
        }

        private Rectangle DamageToSourceRect(float damage)
        {
            Rectangle spriteSheet = TextureFromID(@"Tiles\damage").Bounds;
            int sections = spriteSheet.Width / TILE_LENGTH;
            int frame = (int) Math.Floor(sections * damage);
            if (frame == sections)
            {
                frame--;
            }
            Rectangle source = new Rectangle(frame * TILE_LENGTH, 0, TILE_LENGTH, TILE_LENGTH);
            return source;
        }
    }
}
