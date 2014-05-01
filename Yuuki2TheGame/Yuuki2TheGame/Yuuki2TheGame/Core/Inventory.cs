using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    class InventorySlot
    {
        public Item Item { get; set; }

        public int Count { get; set; }

        public bool IsActive { get; set; }

        public InventorySlot(Item item)
        {
            this.Item = item;
            Count = 0;
            IsActive = false;
        }
    }

    class Inventory
    {
        private List<InventorySlot> slots;

        public int QuickSlotsCount { get; set; }

        public IList<InventorySlot> QuickSlots
        {
            get
            {
                return slots.Take(QuickSlotsCount).ToList();
            }
        }

        public Inventory(int size)
        {
            slots = new List<InventorySlot>();
            for (int i = 0; i < size; i++)
            {
                slots.Add(new InventorySlot(null));
            }
        }

        public bool Contains(Item i)
        {
            return slots.Exists(x => x.Item.ID == i.ID);
        }

        public void Add(Item item)
        {
            if (item.Stackable && this.Contains(item))
            {
                slots.Find(x => item.ID == x.Item.ID).Count++;
            }
            else
            {
                InventorySlot empty = GetNextEmptySlot();
                empty.Item = item;
                empty.Count = 1;
            }
        }

        public InventorySlot GetNextEmptySlot()
        {
            return slots.Find(x => x.Item == null);
        }

        //removes one instance of the item.
        public void Remove(Item n)
        {
            if (Contains(n))
            {
                if (n.Stackable)
                {
                    InventorySlot sl = slots.Find(x => n.ID == x.Item.ID);
                    sl.Count++;
                }
                else
                {
                    InventorySlot sl = slots.Find(x => n.ID == x.Item.ID);
                    sl.Item = null;
                    sl.Count = 0;
                    sl.IsActive = false;
                }
            }

        }

        public void Empty()
        {
            slots.Clear();
        }
        
    }
}
