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
        AxeNormal,
        ShovelNormal,
        PickaxeNormal,
        Potion,
        Bomb
    }
    
    /// <summary>
    /// A method for an item to perform its action when used.
    /// </summary>
    /// <param name="caller">The Item instance that is calling the action.</param>
    /// <param name="position">The absolute pixel coordinates of where the item was used.</param>
    /// <param name="coordinates">The absolute physical coordinates of where the item was used.</param>
    /// <param name="map">The map with the current data.</param>
    /// <param name="user">The character that used the item.</param>
    /// <returns>How many of that inventory item was used up during the process of using it.</returns>
    delegate int ItemAction(Item caller, Point position, Point coordinates, Map map, GameCharacter user);

    struct ItemData
    {
        public readonly int StackSize;
        public readonly int Level;
        public readonly int Durability;
        public readonly string Name;
        public readonly string Texture;
        public readonly BlockID BlockID;
        public readonly ItemType Type;
        public readonly ItemAction Action;
        public ItemData(ItemType type, string name, int stack, string texture, int level, int durability)
        {
            this.Type = type;
            this.Name = name;
            this.StackSize = ((stack >= 1) ? stack : 1);
            this.Level = level;
            this.Durability = durability;
            this.Texture = texture;
            this.BlockID = BlockID.Dirt;
            this.Action = Item.GetAction(type);
        }
        public ItemData(BlockID blockId, string name, int stack, string texture)
        {
            this.Type = ItemType.Block;
            this.Name = name;
            this.StackSize = ((stack >= 1) ? stack : 1);
            this.Level = 0;
            this.Durability = 0;
            this.Texture = texture;
            this.BlockID = blockId;
            this.Action = Item.GetAction(ItemType.Block);
        }
    }

    class Item
    {

        public const int MAX_BLOCK_STACK = 200;

        public const int MAX_AXE_STACK = 10;

        public const int MAX_PICKAXE_STACK = 30;

        public const int MAX_SHOVEL_STACK = 2000; // ?!

        public const int MAX_CONSUMABLE_STACK = 25;

        public const int MAX_BOMB_STACK = 25;

        private static IDictionary<ItemID, ItemData> types = new Dictionary<ItemID, ItemData>();

        private static IDictionary<ItemType, ItemAction> actions = new Dictionary<ItemType, ItemAction>();

        internal static ItemAction GetAction(ItemType type)
        {
            if (Item.actions.ContainsKey(type))
            {
                return Item.actions[type];
            }
            else
            {
                return Item.DropItemAction;
            }
        }

        static Item()
        {
            Item.actions[ItemType.Block] = delegate(Item caller, Point pos, Point coords, Map map, GameCharacter c)
            {
                if (true)
                {

                }
                return 1;
            };
            Item.types[ItemID.BlockDirt] = new ItemData(BlockID.Dirt, "Dirt", MAX_BLOCK_STACK, @"Items\dirt");
            Item.types[ItemID.BlockGrass] = new ItemData(BlockID.Grass, "Grass", MAX_BLOCK_STACK, @"Items\grass");
            Item.types[ItemID.BlockStone] = new ItemData(BlockID.Stone, "Stone", MAX_BLOCK_STACK, @"Items\stone");
            Item.types[ItemID.BlockWood] = new ItemData(BlockID.Wood, "Wood", MAX_BLOCK_STACK, @"Items\wood");
        }

        private static int DropItemAction(Item caller, Point pos, Point coords, Map map, GameCharacter c)
        {
            return 1;
        }

        public int StackSize { get; private set; }

        public bool IsStackable { get; private set; }

        public int Level { get; private set; }

        public int Durability { get; set; }

        public int MaxDurability { get; private set; }

        public ItemID ID { get; private set; }

        public string Name { get; set; }

        public string Texture { get; private set; }

        public BlockID BlockID { get; private set; }

        public ItemType Type { get; private set; }

        public Item(ItemID id)
        {
            ItemData data = Item.types[id];
            this.ID = id;
            this.BlockID = data.BlockID;
            this.Durability = this.MaxDurability = data.Durability;
            this.IsStackable = (data.StackSize > 1);
            this.Level = data.Level;
            this.Name = data.Name;
            this.StackSize = data.StackSize;
            this.Texture = data.Texture;
            this.Type = data.Type;
        }

        /// <summary>
        /// Invokes the action associated with this Item.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="coordinates"></param>
        /// <param name="map"></param>
        /// <param name="user"></param>
        /// <returns>Returns how many of the item was used up.</returns>
        public int Use(Point position, Point coordinates, Map map, GameCharacter user)
        {
            return UseItem(this, position, coordinates, map, user);
        }

        public void Update(GameTime gameTime) {

        }

        private readonly ItemAction UseItem;
        
    }

}
