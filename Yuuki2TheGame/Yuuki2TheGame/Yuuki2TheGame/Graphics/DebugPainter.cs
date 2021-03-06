﻿using System;
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
    /// Draws debug messages
    /// </summary>
    class DebugPainter : Painter
    {
        public static int START_Y = 510;

        private static DebugPainter instance;

        private Engine engine;

        private DebugPainter(Engine engine)
        {
            this.engine = engine;
        }

        public static DebugPainter GetInstance(Engine engine)
        {
            if (DebugPainter.instance == null)
            {
                DebugPainter.instance = new DebugPainter(engine);
            }
            return DebugPainter.instance;
        }

        protected override void Init()
        { }

        protected override void Load()
        {
            CreateRect("$debug_back", Game1.GAME_WIDTH, Game1.GAME_HEIGHT - (START_Y - 2));
        }

        protected override void Unload()
        { }

        protected override void Paint(GameTime gameTime, SpriteBatch batch)
        {
            if (engine.InDebugMode)
            {
                DrawDebugInfo(batch);
            }
            if (engine.ManualPhysStepMode)
            {
                DrawManualPhysInfo(batch);
            }
            if (engine.RecordPhysStep)
            {
                DrawRecordingPhysStep(batch);
            }
        }

        private void DrawDebugInfo(SpriteBatch batch)
        {
            Texture2D backdrop = TextureFromID("$debug_back");
            PlayerCharacter pc = engine.Player;
            Vector2 s = pc.PhysPosition;
            Vector2 v = pc.Velocity;
            Vector2 a = pc.Acceleration;
            Vector2 fric = pc.Friction;
            Vector2 drag = pc.Drag;
            Vector2 f = pc.Force;
            string[] debug = new string[6];
            debug[0] = string.Format("Pos:({0:N7}, {1:N7})", s.X, s.Y);
            debug[1] = string.Format("Vel:({0:N7}, {1:N7})", v.X, v.Y);
            debug[2] = string.Format("Acc:({0:N7}, {1:N7})", a.X, a.Y);
            debug[3] = string.Format("Force:({0:N7}, {1:N7}), Fric:({2:N7}, {3:N7}), Drag:({4:N7}, {5:N7})", f.X, f.Y, fric.X, fric.Y, drag.X, drag.Y);
            debug[4] = string.Format("Mass:{0:N2}  Contact:{1}", pc.Mass, Convert.ToString(pc.ContactMask, 2).PadLeft(5, '0'));
            debug[5] = string.Format("ArmFrame:{0}  LegFrame:{1}  Flipped:{2}", pc.ArmAnimationFrame, pc.LegAnimationFrame, pc.Flipped);
            Rectangle destRect = new Rectangle(0, (START_Y - 2), backdrop.Width, backdrop.Height);
            batch.Draw(backdrop, destRect, new Color(0x80, 0x80, 0x80, 0x80));
            for (int i = 0; i < debug.Length; i++)
            {
                batch.DrawString(DefaultFont, debug[i], new Vector2(5, START_Y + (i * 15)), Color.Red);
            }
        }

        private void DrawManualPhysInfo(SpriteBatch batch)
        {
            string debug = "Hit F8 to step physics or F5 to turn on auto step";
            batch.DrawString(DefaultFont, debug, new Vector2(200, 0), Color.Red);
        }

        private void DrawRecordingPhysStep(SpriteBatch batch)
        {
            string debug = "(recording)";
            batch.DrawString(DefaultFont, debug, new Vector2(720, 0), Color.Red);
        }
    }
}
