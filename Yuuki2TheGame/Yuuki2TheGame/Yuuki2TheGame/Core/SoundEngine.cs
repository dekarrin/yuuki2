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
        SoundEffectInstance squish;
        SoundEffectInstance step;
        SoundEffectInstance tele;
        SoundEffectInstance blockBreak;
        SoundEffectInstance blockPlace;
        SoundEffectInstance itemContact;
        SoundEffectInstance itemPickup;
        SoundEffectInstance jump;
        SoundEffectInstance land;
        SoundEffectInstance onHover;
        SoundEffectInstance onSelect;
        public SoundEngine(List<SoundEffect> GameAudio)
        {
            bgm = GameAudio[0].CreateInstance();
            bgm.IsLooped = true;
            bgm.Play();

            action = GameAudio[1].CreateInstance();
            squish = GameAudio[2].CreateInstance();
            step = GameAudio[3].CreateInstance();
            tele = GameAudio[4].CreateInstance();
            blockBreak = GameAudio[5].CreateInstance();
            blockPlace = GameAudio[6].CreateInstance();
            itemContact = GameAudio[7].CreateInstance();
            itemPickup = GameAudio[8].CreateInstance();
            jump = GameAudio[9].CreateInstance();
            land = GameAudio[10].CreateInstance();
            onHover = GameAudio[11].CreateInstance();
            onSelect = GameAudio[12].CreateInstance();
        }

        public void PlaySound()
        {
            if (action.State != SoundState.Playing)
            {
                action.Play();
            }
        }

        public void PlaySquish()
        {
            if (squish.State != SoundState.Playing)
            {
                squish.Play();
            }
        }


        public void PlayStep()
        {
            if (step.State != SoundState.Playing)
            {
                step.Play();
            }
        }

        public void PlayTele()
        {
            if (tele.State != SoundState.Playing)
            {
                tele.Play();
            }
        }

        public void PlayBlockPlace()
        {
            if (blockPlace.State != SoundState.Playing)
            {
                blockPlace.Play();
            }
        }

        public void PlayBlockBreak()
        {
            if (blockBreak.State != SoundState.Playing)
            {
                blockBreak.Play();
            }
        }
        public void PlayItemContact()
        {
            if (itemContact.State != SoundState.Playing)
            {
                itemContact.Play();
            }
        }
        public void PlayItemPickup()
        {
            if (itemPickup.State != SoundState.Playing)
            {
                itemPickup.Play();
            }
        }
        public void PlayJump()
        {
            if (jump.State != SoundState.Playing)
            {
                jump.Play();
            }
        }
        public void PlayLand()
        {
            if (land.State != SoundState.Playing)
            {
                land.Play();
            }
        }

        public void PlayOnHover()
        {
            if (onHover.State != SoundState.Playing)
            {
                onHover.Play();
            }
        }

        public void PlayOnSelect()
        {
            if (onSelect.State != SoundState.Playing)
            {
                onSelect.Play();
            }
        }
    }
}
