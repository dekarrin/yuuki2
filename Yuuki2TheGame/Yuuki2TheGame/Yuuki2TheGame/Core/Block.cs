using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuuki2TheGame.Physics;
using Yuuki2TheGame;
using Yuuki2TheGame.Data;

namespace Yuuki2TheGame.Core
{
    delegate void DestroyHandler(Block source);

    struct BlockData
    {
        public int LevelRequired;
        public int Health;
        public string Texture;
        public string Name;
        public BlockData(string name, int levelRequired, int health, string texture)
        {
            this.Name = name;
            this.LevelRequired = levelRequired;
            this.Health = health;
            this.Texture = texture;
        }
    }

    enum BlockID
    {
        Dirt,
        Stone,
        Wood,
        Grass
    }

    class Block : ScreenEntity
    {
        private static IDictionary<BlockID, BlockData> types = new Dictionary<BlockID, BlockData>();
        
        static Block()
        {
            List<GameDataObject> temp = new List<GameDataObject>();
            foreach (GameDataObject ob in Game1.ObjectData)
            {
                temp.Add(ob);
            }

            types[BlockID.Dirt] = new BlockData(temp[0].Name, temp[0].LevelRequired, temp[0].Health, temp[0].FilePath);
            types[BlockID.Stone] = new BlockData(temp[1].Name, temp[1].LevelRequired, temp[1].Health, temp[1].FilePath);
            types[BlockID.Wood] = new BlockData(temp[2].Name, temp[2].LevelRequired, temp[2].Health, temp[2].FilePath);
            types[BlockID.Grass] = new BlockData(temp[3].Name, temp[3].LevelRequired, temp[3].Health, temp[3].FilePath);
        }

        public Item Item
        {
            get
            {
                if (ID == BlockID.Grass)
                {
                    return new Item(BlockID.Dirt);
                }
                else
                {
                    return new Item(ID);
                }
            }
        }

        public int LevelRequired { get; private set; }

        public int Health { get; set; }

        public int MaxHealth { get; private set; }

        public BlockID ID { get; private set; }

        public event DestroyHandler OnDestroy;

        public Block(BlockID id, int mapx, int mapy)
            : base(new Point(Game1.METER_LENGTH, Game1.METER_LENGTH))
        {
            Initialize(id, mapx, mapy);
        }

        public Block(int id, int mapx, int mapy)
            : base(new Point(Game1.METER_LENGTH, Game1.METER_LENGTH))
        {
            BlockID bid;
            if (Enum.IsDefined(typeof(BlockID), id))
            {
                bid = (BlockID)id;
            }
            else
            {
                bid = Block.types.First(x => true).Key;
            }
            Initialize(bid, mapx, mapy);
		}

        /// <summary>
        /// Does damage to this block.
        /// </summary>
        /// <param name="basePower">Amount of damage if this block is not weak to the attack.</param>
        /// <param name="highPower">Amount of damage if this block is weak to the attack.</param>
        /// <param name="weakBlocks">A collection if IDs that the attacker specializes in destroying.
        /// If this Block's ID is included, the attack does additional damage.</param>
        /// <returns>Whether this block was completely destroyed.</returns>
        public void Damage(int basePower, int highPower, ICollection<BlockID> weakBlocks)
        {
            if (weakBlocks.Contains(this.ID))
            {
                Health -= highPower;
            }
            else
            {
                Health -= basePower;
            }
            if (Health <= 0 && OnDestroy != null)
            {
                OnDestroy(this);
            }
        }

        public void Restore()
        {
            Health = MaxHealth;
        }

        public override void Update(GameTime gt)
        {
        }

        private void Initialize(BlockID id, int mapx, int mapy)
        {
            if (!Block.types.ContainsKey(id))
            {
                id = Block.types.First(x => true).Key;
            }
            BlockData data = Block.types[id];
            this.ID = id;
            this.LevelRequired = data.LevelRequired;
            this.Health = this.MaxHealth = data.Health;
            this.Texture = data.Texture;
            this.BlockPosition = new Vector2(mapx, mapy);
        }
    }
}
