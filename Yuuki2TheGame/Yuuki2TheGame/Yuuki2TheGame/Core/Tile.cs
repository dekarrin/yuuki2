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
        public Block Block { get; set; }

        /// <summary>
        /// Location of this Tile on the screen.
        /// </summary>
        public Vector2 Location { get; set; }

        public Texture2D Texture { get; set; }

        public string TextureID { get; set; }

    }
}
