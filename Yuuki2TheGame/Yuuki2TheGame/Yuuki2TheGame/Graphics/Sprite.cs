using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Yuuki2TheGame.Graphics
{
    /// <summary>
    /// Item drawn on screen. Has a name for texture that is translated into texture.
    /// </summary>
    class Sprite
    {

        public Point Position { get; set; }

        public Point Size { get; set; }

        public Point Align { get; set; }

        public Texture2D Texture { get; set; }

        public string TextureID { get; set; }

        public Sprite(Point position, Point size, string textureid)
        {
            Position = position;
            Size = size;
            TextureID = textureid;
            Align = new Point(0, 0);
            Texture = null;
        }
    }
}
