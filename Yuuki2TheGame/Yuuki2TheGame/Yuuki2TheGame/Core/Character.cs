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

    class GameCharacter : IUpdateable, ILocatable
    {
        private int _updateOrder = 0;

        public int UpdateOrder
        {
            get
            {
                return _updateOrder;
            }
            set
            {
                bool diff = _updateOrder != value;
                _updateOrder = value;
                if (diff && UpdateOrderChanged != null)
                {
                    UpdateOrderChanged(this, new EventArgs());
                }
            }
        }

        private bool _enabled = true;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                bool diff = _enabled != value;
                _enabled = value;
                if (diff && EnabledChanged != null)
                {
                    EnabledChanged(this, new EventArgs());
                }
            }
        }

        public event EventHandler<EventArgs> UpdateOrderChanged = null;

        public event EventHandler<EventArgs> EnabledChanged = null;
        
        private int _health = 0;

        private float _x;

        private float _y;

        // Added a CharacterSprite Object so that every character can be "drawn"        -CA
        private CharacterSprite _sprite;

        public CharacterSprite Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }
        // Added an EquippedSet object so that evey character has a "loadout" of armor,clothing,weapons,etc.    -CA
        private EquippedSet _currentEquipment;

        public EquippedSet CurrentEquipment
        {
            get { return _currentEquipment; }
            set { _currentEquipment = value; }
        }


        public Vector2 Location {
            get
            {
                return new Vector2(X, Y);
            }
            private set
            {
                if ((_x != value.X || _y != value.Y) && OnMoved != null)
                {
                    Vector2 oldV = new Vector2(_x, _y);
                    Vector2 newV = new Vector2(value.X, value.Y);
                    MovedEventArgs mea = new MovedEventArgs();
                    mea.NewLocation = newV;
                    mea.OldLocation = oldV;
                    OnMoved(this, mea);
                }
                _x = value.X;
                _y = value.Y;
            }
        }

        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                if (_x != value && OnMoved != null)
                {
                    Vector2 oldV = new Vector2(_x, _y);
                    Vector2 newV = new Vector2(value, _y);
                    MovedEventArgs mea = new MovedEventArgs();
                    mea.NewLocation = newV;
                    mea.OldLocation = oldV;
                    OnMoved(this, mea);
                }
                _x = value;
            }
        }

        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                if (_y != value && OnMoved != null)
                {
                    Vector2 oldV = new Vector2(_x, _y);
                    Vector2 newV = new Vector2(_x, value);
                    MovedEventArgs mea = new MovedEventArgs();
                    mea.NewLocation = newV;
                    mea.OldLocation = oldV;
                    OnMoved(this, mea);
                }
                _y = value;
            }
        }

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

        public event MovedEventHandler OnMoved = null;

        public GameCharacter(string name, Vector2 location, int health, int baseAttack, int baseArmor)
        {
            this.Name = name;
            this.Location = location;
            this.MaxHealth = health;
            this.Health = health;
            this.BaseArmor = baseArmor;
            this.BaseAttack = baseAttack;

            // Added simple Equipment defaulting to nothing, and Sprite defaulting to just the skin set
            this.CurrentEquipment = new EquippedSet();
            this.Sprite = new CharacterSprite(location.X, location.Y,0, 11,12,true,true,0); // Spawn with naught but ye' skin and few scraps of cloth...
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
        public void Update(GameTime ts)
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
