using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics;
using FarseerPhysics.Dynamics;

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
