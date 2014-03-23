using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    class Background
    {
        public static Texture2D PlainsBackdrop;

        private Rectangle farA;     // leftmost rectangle of far distance
        private Rectangle farB;     // rightmost
        private Rectangle midA;     // leftmost rectangle of middle distance
        private Rectangle midB;     // rightmost
        private Rectangle closeA;   // leftmost rectangle of close distance
        private Rectangle closeB;   // rightmost

        private Rectangle spF;      // sprite rectangle of far
        private Rectangle spM;      // sprite rectangle of mid
        private Rectangle spC;      // sprite rectangle of close

        private int leftBound;
        private int rightBound;



        private char direction;

    

        int cF;
        int cM;
        int cC;

        public Background()
        {
            leftBound = -Game1.GAME_WIDTH;
            rightBound = 0;

            cF = 0;
            cM = 0;
            cC = 0;

            farA = new Rectangle(0, 250, Game1.GAME_WIDTH, 200);
            farB = new Rectangle(Game1.GAME_WIDTH, 250, Game1.GAME_WIDTH, 200);

            midA = new Rectangle(0, 400, Game1.GAME_WIDTH, 200);
            midB = new Rectangle(Game1.GAME_WIDTH, 400, Game1.GAME_WIDTH, 200);

            closeA = new Rectangle(0, 500, Game1.GAME_WIDTH, 100);
            closeB = new Rectangle(Game1.GAME_WIDTH, 500, Game1.GAME_WIDTH, 100);

            spF = new Rectangle(0, 128, 512, 128);
            spM = new Rectangle(0, 320, 512, 128);
            spC = new Rectangle(0, 0, 512, 64);

            direction = 'n';

        }

        public void Update()
        {
            KeyboardState kb = Keyboard.GetState();

            int a, b, c;
            a = 1;  // pixels per move for far layer
            b = 2;  // pixels per move for middle layer
            c = 4;  // pixels per move for close layer
            

            if(kb.IsKeyDown(Keys.D))
            {
                    Direction = 'r';

                    // Far Layer
                    if (cF < leftBound)
                    {
                        farA = farB;
                        farB = new Rectangle(Game1.GAME_WIDTH - a, farB.Y, farB.Width, farB.Height);
                        cF += Game1.GAME_WIDTH;

                    }
                    else
                    {
                        farA.X -= a;
                        farB.X -= a;
                        cF -= a;


                    }

                    // Middle Layer
                    if (cM < leftBound)
                    {
                        midA = midB;
                        midB = new Rectangle(Game1.GAME_WIDTH - b, midB.Y, midB.Width, midB.Height);
                        cM += Game1.GAME_WIDTH;

                    }
                    else
                    {
                        midA.X -= b;
                        midB.X -= b;
                        cM -= b;
                    }

                    // Close Layer
                    if (cC < leftBound)
                    {
                        closeA = closeB;
                        closeB = new Rectangle(Game1.GAME_WIDTH - c, closeB.Y, closeB.Width, closeB.Height);
                        cC += Game1.GAME_WIDTH;

                    }
                    else
                    {
                        closeA.X -= c;
                        closeB.X -= c;
                        cC -= c;
                    }

                
            }


            if (kb.IsKeyDown(Keys.A))
            {

                    Direction = 'l';

                    // Far Layer
                    if (cF == rightBound || cF > rightBound + Game1.GAME_WIDTH)
                    {
                        farB = farA;
                        farA = new Rectangle(-Game1.GAME_WIDTH + a, farB.Y, farB.Width, farB.Height);
                        cF -= Game1.GAME_WIDTH;
                    }
                    else
                    {
                        farA.X += a;
                        farB.X += a;
                        cF += a;
                    }

                    // Middle Layer
                    if (cM == rightBound || cM > rightBound + Game1.GAME_WIDTH)
                    {
                        midB = midA;
                        midA = new Rectangle(-Game1.GAME_WIDTH + b, midB.Y, midB.Width, midB.Height);
                        cM -= 800;
                    }
                    else
                    {
                        midA.X += b;
                        midB.X += b;
                        cM += b;
                    }

                    // Close Layer
                    if (cC == rightBound || cC > rightBound + Game1.GAME_WIDTH)
                    {
                        closeB = closeA;
                        closeA = new Rectangle(-Game1.GAME_WIDTH + c, closeB.Y, closeB.Width, closeB.Height);
                        cC -= Game1.GAME_WIDTH;
                    }
                    else
                    {
                        closeA.X += c;
                        closeB.X += c;
                        cC += c;
                    }

            }
        }

        public void Draw(SpriteBatch sp)
        {

            // Debug text
            //sp.DrawString(Sky.clockFont, "  Far Counter: " + cF, new Vector2(0, 16), Color.White);
            //sp.DrawString(Sky.clockFont, "  Mid Counter: " + cM, new Vector2(0, 32), Color.White);
            //sp.DrawString(Sky.clockFont, "Close Counter: " + cC, new Vector2(0, 48), Color.White);
            //sp.DrawString(Sky.clockFont, "    Direction: " + Direction, new Vector2(0, 64), Color.White);


            // Draw Calls for Background
            sp.Draw(PlainsBackdrop, farA, spF, Color.White);
            sp.Draw(PlainsBackdrop, farB, spF, Color.White);
            sp.Draw(PlainsBackdrop, midA, spM, Color.White);
            sp.Draw(PlainsBackdrop, midB, spM, Color.White);
            sp.Draw(PlainsBackdrop, closeA, spC, Color.White);
            sp.Draw(PlainsBackdrop, closeB, spC, Color.White);
        }

        public char Direction
        {
            get { return direction; }
            private set { direction = value; }
        } 

    }

}
