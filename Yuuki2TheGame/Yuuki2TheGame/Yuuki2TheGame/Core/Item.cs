using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    enum ItemType
    {
        Block,
        Weapon,
        Armor,
        Axe,
        PickAxe,
        Gun,
        Shovel,
        Decoration,
    }
    class Item : IUpdateable
    {
        private int id;
        private string name;
        private string type;
        private int level;
        private bool stack;
        private int _maxstack;
        private bool equipable;
        private int count;
        private int blockdamage;


        private int _updateOrder = 0;

        public int UpdateOrder
        {
            get
            {
                return _updateOrder;
            }
            set
            {
                bool diff = _updateOrder != value;
                _updateOrder = value;
                if (diff && UpdateOrderChanged != null)
                {
                    UpdateOrderChanged(this, new EventArgs());
                }
            }
        }

        private bool _enabled = true;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                bool diff = _enabled != value;
                _enabled = value;
                if (diff && EnabledChanged != null)
                {
                    EnabledChanged(this, new EventArgs());
                }
            }
        }

        public event EventHandler<EventArgs> UpdateOrderChanged = null;

        public event EventHandler<EventArgs> EnabledChanged = null;

        public delegate void OnUse(Map m);

        public void Update(GameTime gameTime) {

        }
        public int BlockDamage
        {
            get { return blockdamage; }
            set { blockdamage = value; }
        }
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        public bool Equipable
        {
            get { return equipable; }
            set { equipable = value; }
        }

        public int MaxStack
        {
            get { return _maxstack; }
            set { _maxstack = value; }
        }
        public bool Stackable
        {
            get { return stack; }
            set { stack = value; }
        }
        
        public int Level
        {
            get { return level; }
            set { level = value; }
        }
        

        public bool Stack
        {
            get { return stack; }
            set { stack = value; }
        }
        
        public int ID
        {
            get {return id;}
            set
            {
                if (value > 0)
                {
                    id = value;
                }
                else
                { 
                    id = 1; 
                }
            }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        

        public Item(int number, string ItemName, string ItemType, int ToolLevel, bool isStacakable, bool isEquipable, OnUse ItemAction)
        {
            ID = number;
            Name = ItemName;
            Type = ItemType;
            Level = ToolLevel;
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
            Equipable = isEquipable;
            
        }

        public void Use(Map m, Point p)
        {
            int health = m.BlockAt(p).MiningHealth;
       
            if (m.BlockAt(p) != null)
            {
                health =- this.BlockDamage;
                m.BlockAt(p).MiningHealth = health;
            }
        }

        public void ChangeName(string DesiredName)
        {
            Name = DesiredName;
        }
        

    }
}
