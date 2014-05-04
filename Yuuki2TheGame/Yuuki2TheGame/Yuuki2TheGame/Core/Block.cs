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

        public void Update(GameTime gt)
        {
        }

        public int MiningHealth { get; set; }

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
                bid = BlockID.Dirt;
            }
            Initialize(bid, mapx, mapy);
		}

        private void Initialize(BlockID id, int mapx, int mapy)
        {
            if (!Block.types.ContainsKey(id))
            {
                id = BlockID.Dirt;
            }
            BlockData data = Block.types[id];
            this.ID = id;
            this.LevelRequired = data.LevelRequired;
            this.MiningHealth = data.Health;
            this.Texture = data.Texture;
            this.BlockPosition = new Vector2(mapx, mapy);
        }
    }
}
