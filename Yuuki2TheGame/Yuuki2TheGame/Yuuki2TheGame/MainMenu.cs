using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Yuuki2TheGame
{
    class MainMenu
    {
        public bool selected = false;
        public string text = "MainMenu";
        public Rectangle area;

        public MainMenu(Rectangle Area, string Text)
        {
            area = Area;
            text = Text;

        }

        public void mouseOver(MouseState mbd)
        {
            if (mbd.X > area.X && mbd.X < area.X + area.Width && mbd.Y > area.Y && mbd.Y < area.Y + area.Height)
            {
                selected = true;
            }
            else
            {
                selected = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            if (selected == false)
            {
                spriteBatch.DrawString(font, text, new Vector2(area.X, area.Y), Color.Violet);

            }
            if (selected == true)
            {
                spriteBatch.DrawString(font, text, new Vector2(area.X, area.Y), Color.Aqua);
            }
        }
    }
}
