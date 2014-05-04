using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    class InventorySlot
    {
        private int _count = 0;

        public Item Item { get; set; }

        public int Count {
            get
            {
                return _count;
            }
            set
            {
                _count = Math.Max(0, value);
                if (_count == 0)
                {
                    Item = null;
                }
            }
        }

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

        public InventorySlot ActiveSlot
        {
            get
            {
                return Slots[_activeSlot];
            }
        }

        public int ActiveSlotNumber
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
            ActiveSlotNumber = 0;
        }

        public bool Contains(Item i)
        {
            return Slots.Exists(x => x.Item != null && x.Item.ID == i.ID);
        }

        public bool Add(Item item)
        {
            bool added = false;
            if (this.Contains(item))
            {
                InventorySlot slot = Slots.Find(x => x.Item != null && x.Item.ID == item.ID && x.Count < x.Item.StackSize);
                if (slot != null)
                {
                    slot.Count++;
                    added = true;
                }
            }
            if (!added)
            {
                InventorySlot empty = GetNextEmptySlot();
                if (empty != null)
                {
                    empty.Item = item;
                    empty.Count = 1;
                    added = true;
                }
            }
            return added;
        }

        public InventorySlot GetNextEmptySlot()
        {
            if (IsFull())
            {
                return null;
            }
            else
            {
                return Slots.Find(x => x.Item == null);
            }
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

        /// <summary>
        /// Returns whether all slots have something in them.
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            bool full = true;
            foreach (InventorySlot slot in Slots)
            {
                if (slot.Item == null)
                {
                    full = false;
                    break;
                }
            }
            return full;
        }

        public void Empty()
        {
            Slots.Clear();
        }
        
    }
}
