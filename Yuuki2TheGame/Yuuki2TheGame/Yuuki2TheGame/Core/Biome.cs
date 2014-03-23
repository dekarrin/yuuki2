using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    class Biome
    {
        private string biomeName;
        private SkyType sky;

        public SkyType Sky
        {
            get { return sky; }
            set { sky = value; }
        }

        public string BiomeName
        {
            get { return biomeName; }
            set { biomeName = value; }
        }

        public Biome(string name, Texture2D sk, Texture2D bd)
        {

            biomeName = name;
            sky = new SkyType(sk, bd);
        }

        
        
    }

    class Biome_Plains: Biome
    {
        public static Texture2D PlainsBackdrop;
        public static Texture2D PlainsSky;

        public Biome_Plains()
        : base("Plains", PlainsSky, PlainsBackdrop)
        {

        }
    }

    class SkyType
    {
        private Texture2D backdrop;
        private Texture2D sky;
        private Rectangle skyframe;
        private float currentTime;
        private int currentSkyPixel;
        //private bool draw;

        public SkyType(Texture2D sk, Texture2D bd)
        {
            currentTime = 12.00f;
            skyframe = new Rectangle(0, 0, 1024, 24);
            currentSkyPixel = 0;
            //draw = true;
            sky = sk;
            backdrop = bd;


        }

        public void Update(GameTime gametime)
        {

            int dayMins = 3;                        // realworld length of a 24-hour period
            int dayFrames = dayMins * 60 * 10;      // above ^ in Ms
            int hour = dayFrames/24;                // a relative ingame "hour" based on above^
            int min = hour / 60;                    // a relative ingame "minute" based on above ^

            if (gametime.ElapsedGameTime.Milliseconds % min == 0)  // every minute...
            {
                if (currentTime == 23.59f)                                   // if midnight, reset
                {
                    currentTime = 1.00f;
                    currentSkyPixel += 1;
                    skyframe = new Rectangle(currentSkyPixel, 0, 1024, 24);

                }
                else if (currentTime == 11.59f)                             //if noon, loop image
                {
                    currentTime = 12.00f;
                    currentSkyPixel = 0;
                    skyframe = new Rectangle(currentSkyPixel, 0, 1024, 24);
                }
                else if (currentTime - Math.Truncate(currentTime) < 0.59f)  // if not, add a minute
                {
                    currentTime += 0.01f;
                    currentSkyPixel += 1;
                    skyframe = new Rectangle(currentSkyPixel, 0, 1024, 24);
                }
                else                                                        // if an hour, add one
                {
                    currentTime += 1;
                    currentSkyPixel += 1;
                    skyframe = new Rectangle(currentSkyPixel, 0, 1024, 24);
                }
                //draw = true;
                Console.WriteLine("time:/t" + currentTime + "/t pixelPos:\t" + currentSkyPixel);
            }
            //else
                //draw = false;
          
        }

        public void Draw(GameTime gametime, SpriteBatch sb)
        {
            //if(draw)
                sb.Draw(Biome_Plains.PlainsSky,new Rectangle(0,0,Game1.GAME_WIDTH, Game1.GAME_HEIGHT), skyframe,Color.White); 
        }


    }
}
