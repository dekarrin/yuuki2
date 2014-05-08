using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    class BodyPart : ActiveEntity
    {
        public const int SPRITE_LENGTH = 16;

        public int Row { get; private set; }

        public int Col { get; private set; }

        public override Graphics.Sprite Sprite
        {
            get
            {
                Graphics.Sprite sp = base.Sprite;
                sp.SourceSize = new Point(SPRITE_LENGTH, SPRITE_LENGTH);
                sp.Align = new Point(SPRITE_LENGTH * Col, SPRITE_LENGTH * Row);
                sp.TextureID = @"Sprites\sprites";
                return sp;
            }
        }

        public BodyPart(int row, int col)
           : base(new Point(Game1.METER_LENGTH, Game1.METER_LENGTH))
        {
            Row = row;
            Col = col;
        }
    }
}
