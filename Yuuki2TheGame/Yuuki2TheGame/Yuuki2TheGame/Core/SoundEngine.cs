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
        public SoundEngine(List<SoundEffect> GameAudio)
        {
            bgm = GameAudio[0].CreateInstance();
            bgm.IsLooped = true;
            bgm.Play();
            
        }

        public void PlaySound(List<SoundEffect> GameAudio)
        {
            GameAudio[1].Play();
        }
    }
}
