using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    class SoundEngine{

        private SoundEffectInstance BackroundMusic;
        


        public SoundEngine(List<SoundEffect> GameAudio) 
        {
            BackroundMusic = GameAudio.Find(x => x.Name == "backround").CreateInstance();
            BackroundMusic.IsLooped = true;
        }

        public void PlaySound(List<SoundEffect> GameAudio)
        {
            SoundEffect randomSound;
            do
            {
                Random r = new Random();
                int random = r.Next(GameAudio.Count);
                randomSound = GameAudio[random];
            } while (randomSound.Name != "");

            randomSound.Play();
        }


        public void Update(GameTime gameTime)
        {
            if (BackroundMusic.State == SoundState.Stopped)
            {
                BackroundMusic.Play();
            }            
        }

        
    }
}
