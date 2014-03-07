using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    class Inventory
    {
        private List<Item> inventory;

        public Inventory(){
            inventory = new List<Item>();
        }

        private Item GetItem(Item item){
            Item FoundItem = inventory.Find(x => item.ID == x.ID);
            return FoundItem;
        }

        public void Add(Item n){
            if (n.Stackable)
            {
                //probably broken code RIP
                Item foundItem = GetItem(n);
                foundItem.Count++;
            }
            else
            {
                inventory.Add(n);
            }
        }

        public void Remove(Item n)
        {
            Item r = GetItem(n);
            inventory.Remove(r);

        }
        
    }
}
