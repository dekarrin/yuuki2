using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    class ItemAction
    {
        public void AxeAction(Map m, Point p)
        {
            Block b = m.BlockAt(p);
            if (m.BlockAt(p).Type == "wood")
            {
                int health;
                int CurrentHealth = m.BlockAt(p).MiningHealth;
                health = CurrentHealth - 50;
                if (health <= 0)
                {
                    //make the block at the point null;
                }
                else
                {
                    m.BlockAt(p).MiningHealth = health;
                }
            }
            else
            {
                int health;
                int CurrentHealth = m.BlockAt(p).MiningHealth;
                health = CurrentHealth - 10;
                m.BlockAt(p).MiningHealth = health;
            }
        }

        public void PickAction(Map m, Point p)
        {

            Block b = m.BlockAt(p);
            if (m.BlockAt(p).Type == "ground")
            {
                int health;
                int CurrentHealth = m.BlockAt(p).MiningHealth;
                health = CurrentHealth - 50;
                if (health <= 0)
                {
                    //make the block at the point null;
                }
                else
                {
                    m.BlockAt(p).MiningHealth = health;
                }
            }
            else
            {
                int health;
                int CurrentHealth = m.BlockAt(p).MiningHealth;
                health = CurrentHealth - 10;
                m.BlockAt(p).MiningHealth = health;
            }
        }

        public void BombAction(Map m, Point p)
        {
            //explodes and destroys a radius of blocks around it.
        }

        public void ShovelAction(Map m, Point p)
        {

            Block b = m.BlockAt(p);
            if (m.BlockAt(p).Type == "Dirt")
            {
                int health;
                int CurrentHealth = m.BlockAt(p).MiningHealth;
                health = CurrentHealth - 50;
                if (health <= 0)
                {
                    //make the block at the point null;
                }
                else
                {
                    m.BlockAt(p).MiningHealth = health;
                }
            }
            else
            {
                int health;
                int CurrentHealth = m.BlockAt(p).MiningHealth;
                health = CurrentHealth - 10;
                m.BlockAt(p).MiningHealth = health;
            }
        }

        public void PotionAction(GameCharacter c)
        {
            int currentHealth = c.Health;
            if (currentHealth + 50 > 100)
            {
                c.Health = 100;
            }
        }


        
    }
}
