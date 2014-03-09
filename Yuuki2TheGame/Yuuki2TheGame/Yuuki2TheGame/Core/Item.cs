using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    
    class Item
    {
        private int id;
        private string name;
        private string type;
        private int level;
        private int health;
        private bool stack;
        private int _maxstack;
        private bool equipable;
        private int count;
        private string description;

        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
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
        

        public Item(int number, string ItemName, string ItemType, int ToolLevel, bool isStacakable, bool isEquipable)
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

        public abstract Item()
        {
            // TODO: Complete member initialization
        }

        public void Use(){
            //implemented with engine details
        }

        public void ChangeName(string DesiredName)
        {
            Name = DesiredName;
        }

        public void PerformAction()
        {

        }

        public string InspectInfo()
        {
            return Description;
        }

        public void OnDestroy()
        {
            this.Health = 0;
        }


        

    }
}
