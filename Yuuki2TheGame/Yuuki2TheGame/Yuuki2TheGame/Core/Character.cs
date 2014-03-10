using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame.Core
{
    class InteractEventArgs : EventArgs
    {
        public GameCharacter Actor { get; set; }
    }

    class GameCharacter : IUpdateable
    {
        private int _health = 0;

        public Point Position { get; private set; }

        public int MaxHealth { get; private set; }

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

        public event DeathHandler OnDeath = null;

        public delegate void InteractHandler(object source, InteractEventArgs e);

        public event InteractHandler OnInteract = null;

        public GameCharacter(string name, Point position, int health, int baseAttack, int baseArmor)
        {
            this.Name = name;
            this.Position = position;
            this.MaxHealth = health;
            this.Health = health;
            this.BaseArmor = baseArmor;
            this.BaseAttack = baseAttack;
        }

        /// <summary>
        /// Interacts with this character.
        /// </summary>
        /// <param name="interactor"></param>
        public void Interact(GameCharacter interactor)
        {
            if (OnInteract != null)
            {
                InteractEventArgs e = new InteractEventArgs();
                e.Actor = interactor;
                OnInteract(this, e);
            }
        }

        /// <summary>
        /// Called by game engine; tells instance to update self.
        /// </summary>
        /// <param name="ts">Amount of time passed since last update.</param>
        public override virtual void Update(GameTime ts)
        {

        }

        public void MoveLeft()
        {
            // can't fill out until we have engine details
        }

        public void MoveRight()
        {
            // can't fill out until we have engine details
        }

        public void Jump()
        {
            // can't fill out until we have engine details
        }


    }
}
