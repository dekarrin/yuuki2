using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    class Sky
    {
        public static Texture2D sky;
        public static SpriteFont clockFont;

        private int rate;
        private int tick;

        int currentTime;
        int currentPixel;

        bool isNight;
        bool isMidday;
        bool isMorning;
        bool isEvening;

        public Sky()
        {
            rate = 3;                     // Set to a different tick count to speed up of slow down time ( small-fast, large-slow)
            currentTime = 100;            // start at 1am
            currentPixel = 0;             // start at top of sheet;

            isNight = true;
            isMidday = false;
            isMorning = false;
            isEvening = false;
        }

        private bool Minute()
        {

            if (tick != rate)
            {
                tick++;
                return false;
            }
            else
            {
                tick = 0;
                return true;
            }

        }


        public void Update(GameTime gt)
        {
            if (currentTime < 600 || currentTime > 1800)          // night 
            {
                isNight = true;
                isMidday = false;
                isMorning = false;
                isEvening = false;
            }
            else if (currentTime >= 600 && currentTime <= 1100)    // morning
            {
                isMidday = false;
                isNight = false;
                isMorning = true;
                isEvening = false;
            }
            else if (currentTime > 1100 && currentTime < 1300)    // midday
            {
                isMidday = true;
                isNight = false;
                isMorning = false;
                isEvening = false;
            }
            else
            {
                isNight = false;
                isMidday = false;
                isMorning = false;
                isEvening = true;
            }


            if (Minute())
            {
                if (currentTime % 100 < 59)    // not a new hour...
                {

                    currentTime += 1;                                // ... add a minute

                    if (isMorning)
                    {
                        currentPixel += 1;
                    }
                    else if (isEvening)
                    {
                        currentPixel -= 1;
                    }

                }
                else                                                // is a new hour...
                {
                    if (currentTime == 2359)                            // and is midnight...
                    {
                        currentTime = 100;                                 // set to 1am
                        currentPixel = 0;                                   // reset image loop
                    }

                    else if (currentTime == 1200)
                    {
                        currentTime += 41;                               // ... make it a new hour
                        currentPixel = 300;

                    }

                    else                                               // and is not midnight...
                    {
                        currentTime += 41;                               // ... make it a new hour

                        if (isMorning)
                        {
                            currentPixel += 1;
                        }
                        else if (isEvening)
                        {
                            currentPixel -= 1;
                        }
                    }
                }



            }

        }

        public string ClockTime()
        {
            string time = "";

            time = " Time: " + (currentTime - (currentTime % 100)) / 100 + ":" + (currentTime % 100);

            return time;
        }

        public void DrawSky(SpriteBatch sp)
        {


            if (isNight)        // night time
            {
                sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, 400), new Rectangle(0, currentPixel, Game1.GAME_WIDTH, 60), Color.White);
                //sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, Game1.GAME_HEIGHT), new Rectangle(0, 360 + currentPixel, Game1.GAME_WIDTH, 60), Color.White);
            }
            else if (isMidday)   // high sun
            {
                sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, 400), new Rectangle(800, currentPixel, Game1.GAME_WIDTH, 60), Color.White);
                //sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, Game1.GAME_HEIGHT), new Rectangle(800, 360 + currentPixel, Game1.GAME_WIDTH, 60), Color.White);
            }
            else if (isMorning)  //sunrise
            {
                sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, 400), new Rectangle(0, currentPixel, Game1.GAME_WIDTH, 60), Color.White);
                //sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, Game1.GAME_HEIGHT), new Rectangle(0, 360 + currentPixel, Game1.GAME_WIDTH, 60), Color.White);
            }
            else                // sunset
            {
                sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, 400), new Rectangle(800, currentPixel, Game1.GAME_WIDTH, 60), Color.White);
                //sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, Game1.GAME_HEIGHT), new Rectangle(800, 360 + currentPixel, Game1.GAME_WIDTH, 60), Color.White);
            }
        }

            public void DrawLight( SpriteBatch sp)
            {
                if (isNight)        // night time
                {
                    //sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, 400), new Rectangle(0, currentPixel, Game1.GAME_WIDTH, 60), Color.White);
                    sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, Game1.GAME_HEIGHT), new Rectangle(0, 360 + currentPixel, Game1.GAME_WIDTH, 60), Color.White);
                }
                else if (isMidday)   // high sun
                {
                    //sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, 400), new Rectangle(800, currentPixel, Game1.GAME_WIDTH, 60), Color.White);
                    sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, Game1.GAME_HEIGHT), new Rectangle(800, 360 + currentPixel, Game1.GAME_WIDTH, 60), Color.White);
                }
                else if (isMorning)  //sunrise
                {
                    //sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, 400), new Rectangle(0, currentPixel, Game1.GAME_WIDTH, 60), Color.White);
                    sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, Game1.GAME_HEIGHT), new Rectangle(0, 360 + currentPixel, Game1.GAME_WIDTH, 60), Color.White);
                }
                else                // sunset
                {
                    //sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, 400), new Rectangle(800, currentPixel, Game1.GAME_WIDTH, 60), Color.White);
                    sp.Draw(sky, new Rectangle(0, 0, Game1.GAME_WIDTH, Game1.GAME_HEIGHT), new Rectangle(800, 360 + currentPixel, Game1.GAME_WIDTH, 60), Color.White);
                }

                // Debug in-game clock
                sp.DrawString(clockFont, ClockTime(), new Vector2(0, 0), Color.White);
            }

    }
}
