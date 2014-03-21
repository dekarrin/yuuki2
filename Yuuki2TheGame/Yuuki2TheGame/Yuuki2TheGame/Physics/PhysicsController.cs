using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Physics
{
    class PhysicsController
    {
        private IList<IPhysical> phobs;

        public PhysicsController()
        {

        }

        public void Update(GameTime time)
        {

        }

        public void AddPhob(IPhysical obj)
        {
            phobs.Add(obj);
        }

        public void RemovePhob(IPhysical obj)
    }
}
