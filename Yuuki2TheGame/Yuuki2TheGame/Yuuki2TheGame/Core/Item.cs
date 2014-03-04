using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    class Item
    {
        int ItemNumber;
        string Name;
        string Type;
        int Level;
        bool Stackable;
        int MaxStack;

        public Item(int number, string ItemName, string ItemType, int ItemLevel, bool isStacakable)
        {
            ItemNumber = number;
            Name = ItemName;
            Type = ItemType;
            Level = ItemLevel;
            Stackable = isStacakable;
            if (Stackable){
               switch(Type){
                   case "block":
                       MaxStack = 200;
                       break;
                   case "tool":
                       MaxStack = 10;
                       break;
                   case "powerup":
                       MaxStack = 2;
                       break;
               }
            }
            else{ MaxStack = 0;}
        }

        public void Use(){
            //implemented with engine details
        }

        public void ChangeName(string DesiredName)
        {
            Name = DesiredName;
        }
        

    }
}
