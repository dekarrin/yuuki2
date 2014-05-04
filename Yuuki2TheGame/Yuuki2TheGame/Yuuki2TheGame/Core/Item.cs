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

    struct ItemTypeData
    {
        public readonly ItemAction Action;
        public readonly ICollection<BlockID> Effectives;

        public ItemTypeData(BlockID[] effectives, ItemAction action)
        {
            this.Action = action;
            this.Effectives = new HashSet<BlockID>(effectives);
        }

        public ItemTypeData(BlockID effective, ItemAction action)
            : this(new[] { effective }, action)
        { }

        public ItemTypeData(ItemAction action)
        {
            this.Action = action;
            this.Effectives = new HashSet<BlockID>();
        }
    }

    struct ItemData
    {
        public readonly int StackSize;
        public readonly int Level;
        public readonly int Durability;
        public readonly string Name;
        public readonly string Texture;
        public readonly BlockID BlockID;
        public readonly ItemType Type;

        public ItemData(ItemType type, string name, int stack, string texture, int level, int durability)
        {
            this.Type = type;
            this.Name = name;
            this.StackSize = ((stack >= 1) ? stack : 1);
            this.Level = level;
            this.Durability = durability;
            this.Texture = texture;
            this.BlockID = BlockID.Dirt;
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

        public const int TOOL_DURABILITY = 100;

        public const int TOOL_BASE_POWER = 10;

        public const int TOOL_MAX_POWER = 50;

        private static IDictionary<ItemID, ItemData> types = new Dictionary<ItemID, ItemData>();

        private static IDictionary<ItemType, ItemTypeData> typeData = new Dictionary<ItemType, ItemTypeData>();

        #region data initialization

        static Item()
        {
            Item.InitializeTypeData();
            Item.InitializeItemData();
        }

        private static void InitializeItemData()
        {
            Item.types[ItemID.BlockDirt] = new ItemData(BlockID.Dirt, "Dirt", MAX_BLOCK_STACK, @"Items\dirt");
            Item.types[ItemID.BlockGrass] = new ItemData(BlockID.Grass, "Grass", MAX_BLOCK_STACK, @"Items\grass");
            Item.types[ItemID.BlockStone] = new ItemData(BlockID.Stone, "Stone", MAX_BLOCK_STACK, @"Items\stone");
            Item.types[ItemID.BlockWood] = new ItemData(BlockID.Wood, "Wood", MAX_BLOCK_STACK, @"Items\wood");
            Item.types[ItemID.AxeNormal] = new ItemData(ItemType.Axe, "Basic Axe", MAX_AXE_STACK, null, 1, TOOL_DURABILITY);
            Item.types[ItemID.PickaxeNormal] = new ItemData(ItemType.Pickaxe, "Basic Pickaxe", MAX_PICKAXE_STACK, null, 1, TOOL_DURABILITY);
            Item.types[ItemID.ShovelNormal] = new ItemData(ItemType.Shovel, "Basic Shovel", MAX_PICKAXE_STACK, null, 1, TOOL_DURABILITY);
        }

        private static void InitializeTypeData()
        {
            Item.typeData[ItemType.Block] = new ItemTypeData(delegate(Item caller, Point pos, Point coords, Map map, GameCharacter user)
            {
                if (map.AddBlock(caller.BlockID, coords))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });
            ItemAction useTool = delegate(Item caller, Point pos, Point coords, Map map, GameCharacter user)
            {
                Block b = map.BlockAt(coords);
                b.Damage(TOOL_BASE_POWER, TOOL_MAX_POWER, caller.EffectiveAgainst);
                if (b.Health <= 0)
                {
                    map.DestroyBlock(coords);
                }
                return 0;
            };
            Item.typeData[ItemType.Axe] = new ItemTypeData(BlockID.Wood, useTool);
            Item.typeData[ItemType.Shovel] = new ItemTypeData(BlockID.Dirt, useTool);
            Item.typeData[ItemType.Pickaxe] = new ItemTypeData(BlockID.Stone, useTool);
            Item.typeData[ItemType.Consumable] = new ItemTypeData(delegate(Item caller, Point pos, Point coords, Map map, GameCharacter user)
            {
                int currentHealth = user.Health;
                if (currentHealth + 50 > 100)
                {
                    user.Health = 100;
                }
                return 1;
            });
            Item.typeData[ItemType.Bomb] = new ItemTypeData(delegate(Item caller, Point pos, Point coords, Map map, GameCharacter user)
            {
                //explodes and destroys a radius of blocks around it.
                return 1;
            });
        }

        #endregion

        private static ItemAction DropItemAction = delegate(Item caller, Point pos, Point coords, Map map, GameCharacter c)
        {
            map.AddItem(caller, pos);
            return 1;
        };

        private readonly ItemAction UseItem;

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

        public ICollection<BlockID> EffectiveAgainst { get; private set; }

        public Item(ItemID id)
        {
            Initialize(id, ref UseItem);
        }

        public Item(int id)
        {
            ItemID itemid;
            if (Enum.IsDefined(typeof(ItemID), id))
            {
                itemid = (ItemID)id;
            }
            else
            {
                itemid = Item.types.First(x => true).Key;
            }
            Initialize(itemid, ref UseItem);
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

        private void Initialize(ItemID id, ref ItemAction action)
        {
            if (!Item.types.ContainsKey(id))
            {
                id = Item.types.First(x => true).Key;
            }
            ItemData data = Item.types[id];
            this.ID = id;
            this.BlockID = data.BlockID;
            this.Durability = this.MaxDurability = data.Durability;
            this.IsStackable = (data.StackSize > 1);
            this.Level = data.Level;
            this.Name = data.Name;
            this.StackSize = data.StackSize;
            this.Texture = data.Texture;
            ItemType type = data.Type;
            if (!Item.typeData.ContainsKey(type))
            {
                type = Item.typeData.First(x => true).Key;
            }
            ItemTypeData typeData = Item.typeData[type];
            this.Type = type;
            action = typeData.Action;
            this.EffectiveAgainst = typeData.Effectives;
        }
        
    }

}
