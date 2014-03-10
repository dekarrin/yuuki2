using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    class Camera
    {

        public Vector2 Location { get; private set; }

        public ILocatable Following { get; set; }

        public Camera(ILocatable gc)
        {
            Following = gc;
        }



    }
}
