using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    class Character
    {
        private int _health = 0;

        public int MaxHealth { get; }

        public int Health {
            get {
                return _health;
            }
            set {
                // range check value
                _health = Math.Max(Math.Min(MaxHealth, value), 0);
                if (_health == 0)
                {
                    if (OnDeath != null)
                    {
                        OnDeath(this, null);
                    }
                }
            }
        }

        public string Name { get; set; }

        public int BaseAttack { get; set; }

        public int BaseArmor { get; set; }

        public object Inventory { get; set; }

        public delegate void DeathHandler(object source, EventArgs e);

        public DeathHandler OnDeath = null;

        public delegate void OnInteract(Character source);

        protected abstract void Interact;

        public void MoveLeft()
        {

        }

        public void MoveRight()
        {

        }

        public void Jump()
        {

        }


    }
}
