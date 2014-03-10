using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    class Camera
    {

        private ILocatable _follow;

        public Vector2 Location { get; private set; }

        public Vector2 Offset { get; set; }

        public ILocatable Following {
            get
            {
                return _follow;
            }
            set
            {
                if (_follow != null)
                {
                    _follow.Moved -= HandleMovement;
                }
                _follow = value;
                _follow.Moved += HandleMovement;
            }
        }

        public Camera(ILocatable gc, Vector2 offset)
        {
            Following = gc;
            Offset = offset;
        }

        public void HandleMovement(object sender, MovedEventArgs e)
        {
            Location = e.NewLocation - Offset;
        }

    }
}
