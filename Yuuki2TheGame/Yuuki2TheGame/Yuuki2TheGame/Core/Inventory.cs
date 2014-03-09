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

        public bool FindItem(Item i){
            return inventory.Exists(x => i.ID == x.ID);
        }

        public void Add(Item n){
            if (n.Stackable && this.FindItem(n))
            {
                //not broken anymore B]
                inventory.Find(x => n.ID == x.ID).Count++;
            }
            else
            {
                inventory.Add(n);
            }
        }

        //removes one instance of the item.
        public void Remove(Item n)
        {
            
            if(n.Stackable && this.FindItem(n))
            {
                inventory.Find(x => n.ID == x.ID).Count--;
            }
            else
            {
                inventory.RemoveAll(x => n.ID == x.ID);
            }

        }

        public void Empty()
        {
            inventory.Clear();
        }
        
    }
}
