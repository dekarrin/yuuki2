using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    class ItemAction
    {
        
        int correctDamage = 50; //damage done if the item is of the right type.
        static void AxeAction(Map m, Point p)
        {

            Block b = m.BlockAt(p);
            if (m.BlockAt(p).Type == "wood")
            {
                int health;
                int CurrentHealth = m.BlockAt(p).MiningHealth;
                health = CurrentHealth - Damage;
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

        static void PickAxeAction(Map m, Point p)
        {

            Block b = m.BlockAt(p);
            if (m.BlockAt(p).Type == "earth")
            {
                int health;
                int CurrentHealth = m.BlockAt(p).MiningHealth;
                health = CurrentHealth - Damage;
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




        static void BombAction(Map m, Point p)
        {
            for(p.)
        }
        
    }
}
