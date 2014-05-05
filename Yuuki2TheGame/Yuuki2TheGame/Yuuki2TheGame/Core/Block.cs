using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yuuki2TheGame.Physics;
using Yuuki2TheGame;

namespace Yuuki2TheGame.Core
{

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
            types[BlockID.Dirt] = new BlockData("Dirt", 1, 100, @"Tiles\dirt");
            types[BlockID.Stone] = new BlockData("Stone", 1, 200, @"Tiles\stone");
            types[BlockID.Wood] = new BlockData("Wood", 2, 250, @"Tiles\wood");
            types[BlockID.Grass] = new BlockData("Grass", 3, 100, @"Tiles\grass");
        }

        public int LevelRequired { get; private set; }

        public int Health { get; set; }

        public int MaxHealth { get; private set; }

        public BlockID ID { get; private set; }

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
        }

        public void Update(GameTime gt)
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
