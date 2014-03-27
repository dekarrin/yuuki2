using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Yuuki2TheGame.Core;

namespace Yuuki2TheGame.Physics
{
    class PhysicsController
    {
        public const float GRAVITY = 9.806f;

        private IList<IPhysical> phobs = new List<IPhysical>();

        private World model = new World(new Vector2(0, GRAVITY));

        public PhysicsController()
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio((float)Game1.BLOCK_WIDTH);
        }

        public void AddMap(Yuuki2TheGame.Core.Map map)
        {
            Point coords = new Point();
            Block block = null;
            for (coords.X = 0; coords.X < map.Width; coords.X++)
            {
                for (coords.Y = 0; coords.Y < map.Height; coords.Y++)
                {
                    block = map.BlockAt(coords);
                    if (block != null)
                    {
                        AddPhob(map.BlockAt(coords));
                    }
                }
            }
        }

        public void Update(GameTime time)
        {
            model.Step((float)time.ElapsedGameTime.Milliseconds);
        }

        public void AddPhob(IPhysical obj)
        {
            obj.SetWorld(model);
            phobs.Add(obj);
        }

        public void RemovePhob(IPhysical obj)
        {
            obj.SetWorld(null);
            phobs.Remove(obj);
        }
    }
}
