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

    enum ItemID
    {
        BlockStone,
        BlockWood,
        BlockDirt,
        BlockGrass,
        AxeNormal = Item.ID_LIMIT_IDK + 1,
        ShovelNormal = Item.ID_LIMIT_AXE + 1,
        PickaxeNormal = Item.ID_LIMIT_SHOVEL + 1,
        Potion = Item.ID_LIMIT_PICKAXE + 1,
        Bomb
    }

    struct ItemData
    {
        public int MaxStack;
        public bool IsStackable;
        public int Level;
        public int Durability;
        public string Name;
        public string Texture;
        public BlockID BlockID;
        public ItemType Type;
        public ItemData(ItemType type, string name, int stack, int level, int durability, string texture)
        {
            this.Type = type;
            this.Name = name;
            this.MaxStack = stack;
            IsStackable = (stack > 1);
            this.Level = level;
            this.Durability = durability;
            this.Texture = texture;
            this.BlockID = BlockID.Dirt;
        }
        public ItemData(BlockID blockId, string name, int stack, string texture)
        {
            this.Type = ItemType.Block;
            this.Name = name;
            this.MaxStack = stack;
            IsStackable = (stack > 1);
            this.Level = 0;
            this.Durability = 0;
            this.Texture = texture;
            this.BlockID = blockId;
        }
    }

    abstract class Item
    {
        private static IDictionary<ItemID, ItemData> types = new Dictionary<ItemID, ItemData>();

        static Item()
        {
            types[ItemID.BlockDirt] = new ItemData(BlockID.Dirt, "Dirt", MAX_BLOCK_STACK, @"Items\dirt");
            types[ItemID.BlockGrass] = new ItemData(BlockID.Grass, "Grass", MAX_BLOCK_STACK, @"Items\grass");
            types[ItemID.BlockStone] = new ItemData(BlockID.Stone, "Stone", MAX_BLOCK_STACK, @"Items\stone");
            types[ItemID.BlockWood] = new ItemData(BlockID.Wood, "Wood", MAX_BLOCK_STACK, @"Items\wood");
        }

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

        public ItemID ID { get; private set; }

        public string Name { get; set; }

        public string Texture { get; protected set; }

        public BlockID BlockID { get; protected set; }

        public ItemType Type { get; protected set; }

        /// <summary>
        /// With such a wide variety of item types, factory pattern seems ideal.
        /// Especially if we can determine concrete class from the ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        protected static Item CreateTool(ItemID id, string name, int toolLevel, int toolDurability)
        {
            Item created = null;
            if ((int)id <= ID_LIMIT_BLOCK)
            {
                created = new BlockItem(id, name, BlockID.Dirt);
            }
            else if ((int)id <= ID_LIMIT_IDK)
            {
                created = null;
            }
            else if ((int)id <= ID_LIMIT_AXE)
            {
                created = new AxeItem(id, name, toolLevel, toolDurability);
            }
            else if ((int)id <= ID_LIMIT_SHOVEL)
            {
                created = new ShovelItem(id, name, toolLevel, toolDurability);
            }
            else if ((int)id <= ID_LIMIT_PICKAXE)
            {
                created = new PickaxeItem(id, name, toolLevel, toolDurability);
            }
            return created;
        }

        protected static Item CreateBlock(ItemID id, string name, BlockID bid)
        {
            Item created = null;
            if ((int)id <= ID_LIMIT_BLOCK)
            {
                created = new BlockItem(id, name, bid);
            }
            else
            {
                created = CreateTool(id, name, 0, 0);
            }
            return created;
        }

        public static Item Create(ItemID id)
        {
            Item created = null;
            ItemData data = Item.types[id];
            if ((int)id <= ID_LIMIT_BLOCK)
            {
                created = new BlockItem(id, data.Name, data.BlockID);
            }
            else
            {
                created = CreateTool(id, data.Name, data.Level, data.Durability);
            }
            return created;
        }

        protected Item(ItemID id, string name)
        {
            ID = id;
            ItemData data = Item.types[ID];
        }

        public void ChangeName(string DesiredName)
        {
            Name = DesiredName;
        }
        
    }

    #region concrete types

    class BlockItem : Item
    {
        public BlockItem(ItemID id, string name, BlockID blockId)
            : base(id, name)
        {
            IsStackable = true;
            Type = ItemType.Block;
            MaxStack = MAX_BLOCK_STACK;
            BlockID = blockId;
        }

        public override int Use(Map m, Point p, PlayerCharacter c)
        {
            // TODO: things for blocks
            return 1;
        }
    }

    abstract class ToolItem : Item
    {
        public ToolItem(ItemID id, string name, int level, int durability)
            : base(id, name)
        {
            IsStackable = true;
            MaxDurability = this.Durability = durability;
        }
    }

    class AxeItem : ToolItem
    {
        public AxeItem(ItemID id, string name, int level, int durability)
            : base(id, name, level, durability)
        {
            Type = ItemType.Axe;
            MaxStack = MAX_AXE_STACK;
        }

        public override int Use(Map m, Point p, PlayerCharacter c)
        {
            Block b = m.BlockAt(p);
            if (m.BlockAt(p).ID == BlockID.Wood)
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
        public ShovelItem(ItemID id, string name, int level, int durability)
            : base(id, name, level, durability)
        {
            Type = ItemType.Shovel;
            MaxStack = MAX_SHOVEL_STACK;
        }

        public override int Use(Map m, Point p, PlayerCharacter c)
        {
            Block b = m.BlockAt(p);
            if (m.BlockAt(p).ID == BlockID.Dirt)
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
        public PickaxeItem(ItemID id, string name, int level, int durability)
            : base(id, name, level, durability)
        {
            Type = ItemType.Pickaxe;
            MaxStack = MAX_PICKAXE_STACK;
        }

        public override int Use(Map m, Point p, PlayerCharacter c)
        {
            Block b = m.BlockAt(p);
            if (m.BlockAt(p).ID == BlockID.Stone)
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
        public ConsumableItem(ItemID id, string name)
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
        public BombItem(ItemID id, string name, int level)
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
