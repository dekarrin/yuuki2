using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Yuuki2TheGame.Core
{
    class Button
    {
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;
        Color color = new Color(255, 255, 255, 255);
        public Vector2 size;
        public Button(Texture2D newTexture, GraphicsDevice graphics)
        {
            texture = newTexture;
            size = new Vector2(graphics.Viewport.Width / 3, graphics.Viewport.Height / 10);
        }
        private bool firstHoverUpdate = false;
        public bool isclicked;
        public void Update(MouseState mouse)
        {
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);
            if (mouseRectangle.Intersects(rectangle))
            {
                if (!firstHoverUpdate)
                {
                    firstHoverUpdate = true;
                    Engine.AudioEngine.PlayOnHover();
                    color.B = 130;
                }
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    isclicked = true;
                    Engine.AudioEngine.PlayOnSelect();
                }
            }
            else if(color.B < 255)
            {
                firstHoverUpdate = false;
                color.B = 255;
                isclicked = false;
            }
        }
            public void setPosition(Vector2 newPosition)
            {
                position = newPosition;
            }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, color);
        }
        
    }
}
