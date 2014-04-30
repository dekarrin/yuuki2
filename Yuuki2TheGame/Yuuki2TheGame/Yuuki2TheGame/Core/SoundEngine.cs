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


        public void Update(GameTime gameTime)
        {
            if (BackroundMusic.State == SoundState.Stopped)
            {
                BackroundMusic.Play();
            }            
        }

        
    }
}
