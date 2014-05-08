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

        public const int CHAR_ROW_HAIR = 0;

        public const int CHAR_ROW_HEAD = 1;

        public const int CHAR_ROW_TORSO = 2;

        public const int CHAR_ROW_PELVIS = 3;

        public const int CHAR_ROW_LEG_FRONT_START = 4;

        public const int CHAR_ROW_LEG_FRONT_END = 7;

        public const int CHAR_ROW_LEG_BACK_START = 8;

        public const int CHAR_ROW_LEG_BACK_END = 11;

        public const int CHAR_ROW_FOOT_FRONT_START = 12;

        public const int CHAR_ROW_FOOT_FRONT_END = 15;

        public const int CHAR_ROW_FOOT_BACK_START = 16;

        public const int CHAR_ROW_FOOT_BACK_END = 19;

        public const int CHAR_ROW_HAND_FRONT_START = 20;

        public const int CHAR_ROW_HAND_FRONT_END = 21;

        public const int CHAR_ROW_HAND_BACK_START = 22;

        public const int CHAR_ROW_HAND_BACK_END = 23;

        public const int CHAR_ROW_ARM_FRONT_START = 24;

        public const int CHAR_ROW_ARM_FRONT_END = 25;

        public const int CHAR_ROW_ARM_BACK_START = 26;

        public const int CHAR_ROW_ARM_BACK_END = 27;

        public const int CHAR_ROW_EYES = 28;

        public const int CHAR_ROW_CAPE = 29;

        public const int CHAR_COL_BASE_START = 0;

        public const int CHAR_COL_BASE_END = 3;

        public const int CHAR_SPRITE_SIZE = 16;

        public const int CHAR_FRAME_HEAD = 0;

        public const int CHAR_FRAME_TORSO = 1;

        public const int CHAR_FRAME_PELVIS = 2;

        public const int CHAR_ANIM_OFFSET_ARM = 1;

        public const int CHAR_ANIM_OFFSET_LEG = 2;

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
            LoadTexture(@"Sprites\sprites");
        }

        protected override void Unload()
        { }

        protected override void Paint(GameTime gameTime, SpriteBatch batch)
        {
            IList<Sprite> tiles = engine.GetView();
            IList<Sprite> ents = engine.GetEntities();
            IList<Sprite> chars = engine.GetCharacters();
            ConvertIDs(tiles);
            ConvertIDs(ents);
            foreach (Sprite sp in tiles)
            {
                DrawBlock(batch, sp);
            }
            foreach (Sprite sp in ents)
            {
                batch.Draw(sp.Texture, sp.Destination, sp.Source, Color.White);
            }
            foreach (Sprite sp in chars)
            {
                DrawCharacter(batch, sp);
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
            if (frame >= sections)
            {
                frame = sections - 1;
            }
            Rectangle source = new Rectangle(frame * TILE_LENGTH, 0, TILE_LENGTH, TILE_LENGTH);
            return source;
        }

        private void DrawCharacter(SpriteBatch batch, Sprite sp)
        {
            DrawRearHand(batch, sp);
            DrawRearArm(batch, sp);
            DrawRearFoot(batch, sp);
            DrawRearLeg(batch, sp);
            DrawPelvis(batch, sp);
            DrawForeLeg(batch, sp);
            DrawTorso(batch, sp);
            DrawForeFoot(batch, sp);
            DrawForeArm(batch, sp);
            DrawForeHand(batch, sp);
            DrawHead(batch, sp);
        }

        private void DrawHead(SpriteBatch batch, Sprite sp)
        {
            DrawStaticBodyPart(batch, sp, CHAR_FRAME_HEAD, CHAR_ROW_HEAD);
        }

        private void DrawForeHand(SpriteBatch batch, Sprite sp)
        {
            DrawAnimBodyPart(batch, sp, ((GameCharacter)sp.Creator).ArmAnimationFrame, CHAR_FRAME_TORSO, CHAR_ROW_HAND_FRONT_START, CHAR_ROW_HAND_FRONT_END);
        }

        private void DrawForeArm(SpriteBatch batch, Sprite sp)
        {
            DrawAnimBodyPart(batch, sp, ((GameCharacter)sp.Creator).ArmAnimationFrame, CHAR_FRAME_TORSO, CHAR_ROW_ARM_FRONT_START, CHAR_ROW_ARM_FRONT_END);
        }

        private void DrawForeFoot(SpriteBatch batch, Sprite sp)
        {
            DrawAnimBodyPart(batch, sp, ((GameCharacter)sp.Creator).LegAnimationFrame, CHAR_FRAME_PELVIS, CHAR_ROW_FOOT_FRONT_START, CHAR_ROW_FOOT_FRONT_END);
        }

        private void DrawTorso(SpriteBatch batch, Sprite sp)
        {
            DrawStaticBodyPart(batch, sp, CHAR_FRAME_TORSO, CHAR_ROW_TORSO);
        }

        private void DrawForeLeg(SpriteBatch batch, Sprite sp)
        {
            DrawAnimBodyPart(batch, sp, ((GameCharacter)sp.Creator).LegAnimationFrame, CHAR_FRAME_PELVIS, CHAR_ROW_LEG_FRONT_START, CHAR_ROW_LEG_FRONT_END);
        }

        private void DrawPelvis(SpriteBatch batch, Sprite sp)
        {
            DrawStaticBodyPart(batch, sp, CHAR_FRAME_PELVIS, CHAR_ROW_PELVIS);
        }

        private void DrawRearLeg(SpriteBatch batch, Sprite sp)
        {
            DrawAnimBodyPart(batch, sp, ((GameCharacter)sp.Creator).LegAnimationFrame + CHAR_ANIM_OFFSET_LEG, CHAR_FRAME_PELVIS, CHAR_ROW_LEG_BACK_START, CHAR_ROW_LEG_BACK_END);
        }

        private void DrawRearFoot(SpriteBatch batch, Sprite sp)
        {
            DrawAnimBodyPart(batch, sp, ((GameCharacter)sp.Creator).LegAnimationFrame + CHAR_ANIM_OFFSET_LEG, CHAR_FRAME_PELVIS, CHAR_ROW_FOOT_BACK_START, CHAR_ROW_FOOT_BACK_END);
        }

        private void DrawRearHand(SpriteBatch batch, Sprite sp)
        {
            DrawAnimBodyPart(batch, sp, ((GameCharacter)sp.Creator).ArmAnimationFrame + CHAR_ANIM_OFFSET_ARM, CHAR_FRAME_TORSO, CHAR_ROW_ARM_BACK_START, CHAR_ROW_ARM_BACK_END);
        }

        private void DrawRearArm(SpriteBatch batch, Sprite sp)
        {
            DrawAnimBodyPart(batch, sp, ((GameCharacter)sp.Creator).ArmAnimationFrame + CHAR_ANIM_OFFSET_ARM, CHAR_FRAME_TORSO, CHAR_ROW_HAND_BACK_START, CHAR_ROW_HAND_BACK_END);
        }

        private void DrawAnimBodyPart(SpriteBatch batch, Sprite sp, int frame, int destFrame, int frameRowStart, int frameRowEnd)
        {

            DrawAnimFrame(batch, frame, ((GameCharacter)sp.Creator).SpriteBase, destFrame, sp.Destination, frameRowStart, frameRowEnd, ((GameCharacter)sp.Creator).Flipped);
            if (((GameCharacter)sp.Creator).CostumeBase != -1)
            {
                int col = CHAR_COL_BASE_END + 1 + ((GameCharacter)sp.Creator).CostumeBase;
                DrawAnimFrame(batch, frame, col, destFrame, sp.Destination, frameRowStart, frameRowEnd, ((GameCharacter)sp.Creator).Flipped);
            }
        }

        private void DrawStaticBodyPart(SpriteBatch batch, Sprite sp, int destFrame, int row)
        {
            DrawFrame(batch, row, ((GameCharacter)sp.Creator).SpriteBase, destFrame, sp.Destination, ((GameCharacter)sp.Creator).Flipped);
            if (((GameCharacter)sp.Creator).CostumeBase != -1)
            {
                int col = CHAR_COL_BASE_END + 1 + ((GameCharacter)sp.Creator).CostumeBase;
                DrawFrame(batch, row, col, destFrame, sp.Destination, ((GameCharacter)sp.Creator).Flipped);
            }
        }

        private void DrawAnimFrame(SpriteBatch batch, int frame, int col, int destFrame, Rectangle dest, int frameStartRow, int frameEndRow, bool flip)
        {
            int displayedHeight = dest.Height / 3;
            Texture2D tex = TextureFromID(@"Sprites\sprites");
            Rectangle source = AnimSourceRect(frame, frameStartRow, frameEndRow, col);
            Rectangle destRect = new Rectangle(dest.X, dest.Y + (destFrame * displayedHeight), dest.Width, displayedHeight);
            SpriteEffects effects = (flip) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            batch.Draw(tex, destRect, source, Color.White, 0.0f, Vector2.Zero, effects, 0.0f);
        }

        private void DrawFrame(SpriteBatch batch, int row, int col, int destFrame, Rectangle dest, bool flip)
        {
            int displayedHeight = dest.Height / 3;
            Texture2D tex = TextureFromID(@"Sprites\sprites");
            Rectangle source = CharSourceRect(row, col);
            Rectangle destRect = new Rectangle(dest.X, dest.Y + (destFrame * displayedHeight), dest.Width, displayedHeight);
            SpriteEffects effects = (flip) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            batch.Draw(tex, destRect, source, Color.White, 0.0f, Vector2.Zero, effects, 0.0f);
        }

        private Rectangle AnimSourceRect(int frame, int frameStartRow, int frameEndRow, int col)
        {
            int domain = 1 + frameEndRow - frameStartRow;
            frame %= domain;
            return CharSourceRect(frameStartRow + frame, col);
        }

        private Rectangle CharSourceRect(int row, int col)
        {
            return new Rectangle(col * CHAR_SPRITE_SIZE, row * CHAR_SPRITE_SIZE, CHAR_SPRITE_SIZE, CHAR_SPRITE_SIZE);
        }
    }
}
