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

    /// <summary>
    /// NOTE: Pixel coordinates is relative to lower left of character!
    /// </summary>
    class GameCharacter : ActiveEntity
    {
        private int _health = 0;

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

        public bool Active { get; set; }

        public int ArmAnimationFrame { get; protected set; }

        public int LegAnimationFrame { get; protected set; }

        public int SpriteBase { get; set; }

        public int CostumeBase { get; set; }

        public string Name { get; set; }

        public int BaseAttack { get; set; }

        public int BaseArmor { get; set; }

        public Inventory Inventory { get; protected set; }

        public delegate void DeathHandler(object source, EventArgs e);

        public event DeathHandler OnDeath = null;

        public delegate void InteractHandler(object source, InteractEventArgs e);

        public event InteractHandler OnInteract = null;
        public GameCharacter(string name, Point position, Point size, int health, int baseAttack, int baseArmor) : base(size, position)
        {
            this.Name = name;
            this.MaxHealth = health;
            this.Health = health;
            this.BaseArmor = baseArmor;
            this.BaseAttack = baseAttack;
            Flipped = false;
            ArmAnimationFrame = 0;
            LegAnimationFrame = 0;
            AnimationDelay = 500;
            Active = true;
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
        /// In ms
        /// </summary>
        protected double AnimationDelay { get; set; }

        private double LastAnimChange { get; set; }

        public bool Flipped { get; private set; }

        private bool noAnimLastUpdate = true;

        /// <summary>
        /// Called by game engine; tells instance to update self.
        /// </summary>
        /// <param name="gameTime">Amount of time passed since last update.</param>
        public override void Update(GameTime gameTime)
        {
            if (IsOnGround())
            {
                if (IsMovingHorizontally())
                {
                    if (noAnimLastUpdate)
                    {
                        LastAnimChange = gameTime.TotalGameTime.TotalMilliseconds;
                        noAnimLastUpdate = false;
                    }
                    else
                    {
                        double timeSinceChange = gameTime.TotalGameTime.TotalMilliseconds - LastAnimChange;
                        if (timeSinceChange >= AnimationDelay)
                        {
                            ArmAnimationFrame++;
                            LegAnimationFrame++;
                            ArmAnimationFrame %= 2;
                            LegAnimationFrame %= 4;
                            if ((LegAnimationFrame + 1) % 2 == 0)
                            {
                                Engine.AudioEngine.PlayStep();
                            }
                            LastAnimChange = gameTime.TotalGameTime.TotalMilliseconds;
                        }
                        float ratio = Math.Abs(Velocity.X) / (float)PlayerCharacter.MAX_SPEED;
                        int subtraction = (int)(ratio * 350);
                        AnimationDelay = 500 - subtraction;
                    }
                }
                else
                {
                    noAnimLastUpdate = true;
                    ArmAnimationFrame = 0;
                    LegAnimationFrame = 0;
                }
            }
            else
            {
                noAnimLastUpdate = true;
                ArmAnimationFrame = 1;
                LegAnimationFrame = 1;
            }
        }

        public virtual void StartMovingLeft()
        {
            Flipped = true;
        }

        public virtual void StartMovingRight()
        {
            Flipped = false;
        }

        public virtual void StopMovingLeft()
        {
        }

        public virtual void StopMovingRight()
        {
        }

        public virtual void Jump()
        {
        }
    }
}
