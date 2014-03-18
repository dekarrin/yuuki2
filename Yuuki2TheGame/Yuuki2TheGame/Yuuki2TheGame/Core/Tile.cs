using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Yuuki2TheGame.Core
{

    /// <summary>
    /// Holds information on a Block that is displayed on the screen.
    /// </summary>
    class Tile
    {
        public static Texture2D BlockSpriteSheet;           // Sprite Sheet for all block types
        public Block Block { get; set; }

        /// <summary>
        /// Location of this Tile on the screen.
        /// </summary>
        public Vector2 Location { get; set; }

        public Rectangle TextureFromID(int id)
        {
            return new Rectangle(id % 16, (int)(id / 16)*16, 16, 16);
        }

        public int TextureID { get; set; }  // no longer needed

    }
}
