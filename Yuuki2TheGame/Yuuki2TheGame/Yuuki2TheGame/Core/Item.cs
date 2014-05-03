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
        Axe,
        Pickaxe,
        Shovel,
        Consumable,
        Bomb
    }

    abstract class Item
    {
        public const int MAX_BLOCK_STACK = 200;

        public const int MAX_AXE_STACK = 10;

        public const int MAX_PICKAXE_STACK = 30;

        public const int MAX_SHOVEL_STACK = 2000; // ?!

        public const int MAX_CONSUMABLE_STACK = 25;

        public const int MAX_BOMB_STACK = 25;

        public const int ID_LIMIT_BLOCK = 8;

        public const int ID_LIMIT_IDK = 16;

        public const int ID_LIMIT_AXE = 24;

        public const int ID_LIMIT_SHOVEL = 32;

        public const int ID_LIMIT_PICKAXE = 2048;

        private int _id;

        /// <summary>
        /// Returns how many of the item was used up.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="p"></param>
        public abstract int Use(Map m, Point p, PlayerCharacter c);

        public void Update(GameTime gameTime) {

        }

        public int MaxStack { get; protected set; }

        public bool IsStackable { get; protected set; }

        public int Level { get; set; }

        public int Durability { get; set; }

        public int MaxDurability { get; protected set; }
        
        public int ID
        {
            get {return _id;}
            set
            {
                if (value > 0)
                {
                    _id = value;
                }
                else
                { 
                    _id = 1; 
                }
            }
        }
        public string Name { get; set; }

        public string TextureID { get; protected set; }

        public ItemType Type { get; protected set; }

        /// <summary>
        /// With such a wide variety of item types, factory pattern seems ideal.
        /// Especially if we can determine concrete class from the ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public static Item Create(int id, string name, int toolLevel, int toolDurability)
        {
            Item created = null;
            if (id <= ID_LIMIT_BLOCK)
            {
                created = new BlockItem(id, name);
            }
            else if (id <= ID_LIMIT_IDK)
            {
                created = null;
            }
            else if (id <= ID_LIMIT_AXE)
            {
                created = new AxeItem(id, name, toolLevel, toolDurability);
            }
            else if (id <= ID_LIMIT_SHOVEL)
            {
                created = new ShovelItem(id, name, toolLevel, toolDurability);
            }
            else if (id <= ID_LIMIT_PICKAXE)
            {
                created = new PickaxeItem(id, name, toolLevel, toolDurability);
            }
            return created;
        }

        public static Item Create(int id, string name)
        {
            return Create(id, name, 0, 0);
        }

        protected Item(int id, string name)
        {
            ID = id;
            Name = name;
            MaxStack = 0;
        }

        public void ChangeName(string DesiredName)
        {
            Name = DesiredName;
        }
        
    }

    #region concrete types

    class BlockItem : Item
    {
        public BlockItem(int id, string name)
            : base(id, name)
        {
            IsStackable = true;
            Type = ItemType.Block;
            MaxStack = MAX_BLOCK_STACK;
        }

        public override int Use(Map m, Point p, PlayerCharacter c)
        {
            // TODO: things for blocks
            return 1;
        }
    }

    abstract class ToolItem : Item
    {
        public ToolItem(int id, string name, int level, int durability)
            : base(id, name)
        {
            IsStackable = true;
            MaxDurability = this.Durability = durability;
        }
    }

    class AxeItem : ToolItem
    {
        public AxeItem(int id, string name, int level, int durability)
            : base(id, name, level, durability)
        {
            Type = ItemType.Axe;
            MaxStack = MAX_AXE_STACK;
        }

        public override int Use(Map m, Point p, PlayerCharacter c)
        {
            Block b = m.BlockAt(p);
            if (m.BlockAt(p).Type == (int)WoodType.Type)
            {
                int health;
                int CurrentHealth = m.BlockAt(p).MiningHealth;
                health = CurrentHealth - 50;
                if (health <= 0)
                {
                    //make the block at the point null;
                }
                else
                {
                    m.BlockAt(p).MiningHealth = health;
                }
            }
            else
            {
                int health;
                int CurrentHealth = m.BlockAt(p).MiningHealth;
                health = CurrentHealth - 10;
                m.BlockAt(p).MiningHealth = health;
            }
            return 0;
        }
    }

    class ShovelItem : ToolItem
    {
        public ShovelItem(int id, string name, int level, int durability)
            : base(id, name, level, durability)
        {
            Type = ItemType.Shovel;
            MaxStack = MAX_SHOVEL_STACK;
        }

        public override int Use(Map m, Point p, PlayerCharacter c)
        {
            Block b = m.BlockAt(p);
            if (m.BlockAt(p).Type == (int)DirtType.Type)
            {
                int health;
                int CurrentHealth = m.BlockAt(p).MiningHealth;
                health = CurrentHealth - 50;
                if (health <= 0)
                {
                    //make the block at the point null;
                }
                else
                {
                    m.BlockAt(p).MiningHealth = health;
                }
            }
            else
            {
                int health;
                int CurrentHealth = m.BlockAt(p).MiningHealth;
                health = CurrentHealth - 10;
                m.BlockAt(p).MiningHealth = health;
            }
            return 0;
        }
    }

    class PickaxeItem : ToolItem
    {
        public PickaxeItem(int id, string name, int level, int durability)
            : base(id, name, level, durability)
        {
            Type = ItemType.Pickaxe;
            MaxStack = MAX_PICKAXE_STACK;
        }

        public override int Use(Map m, Point p, PlayerCharacter c)
        {
            Block b = m.BlockAt(p);
            if (m.BlockAt(p).Type == (int)GroundType.Type)
            {
                int health;
                int CurrentHealth = m.BlockAt(p).MiningHealth;
                health = CurrentHealth - 50;
                if (health <= 0)
                {
                    //make the block at the point null;
                }
                else
                {
                    m.BlockAt(p).MiningHealth = health;
                }
            }
            else
            {
                int health;
                int CurrentHealth = m.BlockAt(p).MiningHealth;
                health = CurrentHealth - 10;
                m.BlockAt(p).MiningHealth = health;
            }
            return 0;
        }
    }

    class ConsumableItem : Item
    {
        public ConsumableItem(int id, string name)
            : base(id, name)
        {
            Type = ItemType.Consumable;
            MaxStack = MAX_CONSUMABLE_STACK;
        }

        public override int Use(Map m, Point p, PlayerCharacter c)
        {
            int currentHealth = c.Health;
            if (currentHealth + 50 > 100)
            {
                c.Health = 100;
            }
            return 1;
        }
    }

    class BombItem : Item
    {
        public BombItem(int id, string name, int level)
            : base(id, name)
        {
            Level = level;
            Type = ItemType.Bomb;
            MaxStack = MAX_BOMB_STACK;
        }

        public override int Use(Map m, Point p, PlayerCharacter c)
        {
            //explodes and destroys a radius of blocks around it.
            return 1;
        }

    }

    #endregion
}
