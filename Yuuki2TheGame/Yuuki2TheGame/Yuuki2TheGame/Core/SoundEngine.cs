using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    class SoundEngine
    {
        SoundEffectInstance bgm;
        SoundEffectInstance action;
        public SoundEngine(List<SoundEffect> GameAudio)
        {
            bgm = GameAudio[0].CreateInstance();
            bgm.IsLooped = true;
            bgm.Play();

            action = GameAudio[1].CreateInstance();
            
        }

        public void PlaySound()
        {
            if (action.State != SoundState.Playing)
            {
                action.Play();
            }
        }
    }
}
