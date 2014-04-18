using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Yuuki2TheGame.Data;

namespace Yuuki2TheGame.Core
{
    enum ItemType
    {
        Block = 1,
        Axe,
        PickAxe,
        Shovel,
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
        //OnUse action;


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

        public delegate void OnUse(Map m, Point p);

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
        

        public Item(int id)
        {
            GameObjectData temp = Engine.items[id - 1];
            this.ID = temp.ID;
            this.Level = temp.Level;
            this.Name = temp.Name;
            this.Type = temp.Type;
            this.Stack = temp.Stack;
            this.MaxStack = temp.MaxStack;
        }


        public void ChangeName(string DesiredName)
        {
            Name = DesiredName;
        }
        

    }
}
