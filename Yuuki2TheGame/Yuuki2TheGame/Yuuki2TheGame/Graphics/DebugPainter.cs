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
    /// Draws debug messages
    /// </summary>
    class DebugPainter : Painter
    {
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
        { }

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
            PlayerCharacter pc = engine.Player;
            Vector2 s = pc.PhysPosition;
            Vector2 v = pc.Velocity;
            Vector2 a = pc.Acceleration;
            Vector2 f = pc.Force;
            string[] debug = new string[5];
            debug[0] = string.Format("P:({0}, {1})", s.X, s.Y);
            debug[1] = string.Format("V:({0}, {1})", v.X, v.Y);
            debug[2] = string.Format("A:({0}, {1})", a.X, a.Y);
            debug[3] = string.Format("F:({0}, {1})", f.X, f.Y);
            debug[4] = string.Format("M:{0}  C:{1}  G:{2}", pc.Mass, Convert.ToString(pc.ContactMask, 2).PadLeft(4, '0'), pc.IsOnGround());
            for (int i = 0; i < debug.Length; i++)
            {
                batch.DrawString(DefaultFont, debug[i], new Vector2(5, i * 15), Color.Red);
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
