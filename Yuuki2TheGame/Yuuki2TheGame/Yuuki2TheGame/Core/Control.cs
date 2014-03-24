using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace Yuuki2TheGame.Core
{
    class Control
    {
        Vector2 position;
        Vector2 velocity;
        //  readonly Vector2 gravity = new Vector2(0, -9.8f);
        KeyboardState KeyState = new KeyboardState();
        bool hasJumped;
        //float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
        //    velocity += gravity * time;
        //    position += velocity * time;


        // Press A to move sprite left.
        public void moveLeft(GameTime gameTime)
        {
            velocity.X -= 1f;

        }

        // Press D to move sprite right.
        public void moveRight(GameTime gameTime)
        {
            //float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //velocity += gravity * time;
            //position += velocity * time;
            velocity.X += 1f;


        }

        // Press W to move sprite up.
        public void moveUp(GameTime gameTime)
        {
            //float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //velocity += gravity * time;
            //position += velocity * time;

            velocity.Y += 1f;
        }

        // Press S to move sprite down.

        public void moveDown(GameTime gameTime)
        {
            //float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //velocity += gravity * time;
            //position += velocity * time;
            velocity.Y -= 1f;

        }

        public void jump()
        {
            if (hasJumped == false)
            {
                position.Y -= 10f;
                velocity.Y -= 5f;
                hasJumped = true;
            }
            if (hasJumped == true)
            {
                float i = 1;
                velocity.Y += 0.15f * i;
            }

        }


    }
}
