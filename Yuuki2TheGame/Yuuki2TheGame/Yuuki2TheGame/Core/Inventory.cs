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
        public List<InventorySlot> Slots { get; private set; }

        private int _activeSlot = 0;

        public int QuickSlotsCount { get; private set;  }

        public int ActiveSlot
        {
            get
            {
                return _activeSlot;
            }
            set
            {
                Slots[_activeSlot].IsActive = false;
                if (value >= 0)
                {
                    _activeSlot = value % QuickSlotsCount;
                }
                else
                {
                    _activeSlot = QuickSlotsCount - 1;
                }
                Slots[_activeSlot].IsActive = true;
            }
        }

        public IList<InventorySlot> QuickSlots
        {
            get
            {
                return Slots.Take(QuickSlotsCount).ToList();
            }
        }

        public Inventory(int size, int quicks)
        {
            Slots = new List<InventorySlot>();
            for (int i = 0; i < size; i++)
            {
                Slots.Add(new InventorySlot(null));
            }
            QuickSlotsCount = quicks;
            ActiveSlot = 0;
        }

        public bool Contains(Item i)
        {
            return Slots.Exists(x => x.Item != null && x.Item.ID == i.ID);
        }

        public void Add(Item item)
        {
            if (item.IsStackable && this.Contains(item))
            {
                Slots.Find(x => x.Item != null && x.Item.ID == item.ID).Count++;
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
            return Slots.Find(x => x.Item == null);
        }

        //removes one instance of the item.
        public void Remove(Item n)
        {
            if (Contains(n))
            {
                if (n.IsStackable)
                {
                    InventorySlot sl = Slots.Find(x => x.Item != null && x.Item.ID == n.ID);
                    sl.Count++;
                }
                else
                {
                    InventorySlot sl = Slots.Find(x => x.Item != null && x.Item.ID == n.ID);
                    sl.Item = null;
                    sl.Count = 0;
                    sl.IsActive = false;
                }
            }

        }

        public void Empty()
        {
            Slots.Clear();
        }
        
    }
}
