using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    class PlayerCharacter : GameCharacter
    {
        public const int WIDTH = Game1.BLOCK_WIDTH * 1;
        public const int HEIGHT = Game1.BLOCK_HEIGHT * 2;

        public static Inventory inventory = new Inventory();
        
        //when no item is equipped it is represent by null
        private Item SelectedItem = null;

        //Assumes that you can only select an item in your inventory
        public void EquipItem(Item i)
        {
            if (SelectedItem == null)
            {
                inventory.Remove(i);
                SelectedItem = i;
            }
            else
            {
                inventory.Add(SelectedItem);
                SelectedItem = null;
            }
        }

        public PlayerCharacter(string name, Point pixelLocation, int health, int baseAttack, int baseArmor) : base(name, pixelLocation, new Point(WIDTH, HEIGHT), health, baseAttack, baseArmor)
        {
        }
    }
}
