using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Yuuki2TheGame.Physics;

namespace Yuuki2TheGame.Core
{
    class PlayerCharacter : GameCharacter
    {
        public const int WIDTH = Game1.METER_LENGTH * 1;
        public const int HEIGHT = Game1.METER_LENGTH * 2;
        public const float WALK_FORCE = 500.0f;
        public const float JUMP_FORCE = 45000.0f;
        public const float MAX_SPEED = 5.0f;


        private bool movingLeft = false;
        private bool movingRight = false;

        public PlayerCharacter(string name, Point pixelLocation, int health, int baseAttack, int baseArmor) : base(name, pixelLocation, new Point(WIDTH, HEIGHT), health, baseAttack, baseArmor)
        {
            this.Inventory = new Inventory(Game1.INVENTORY_ITEMS, Game1.QUICK_SLOTS);
        }

        protected override void ContactMaskChanged(int oldValue)
        {
            if (IsOnLeftWall())
            {
                RemoveForce("move_left");
            }
            else if (movingLeft && ((oldValue & (int)ContactType.LEFT) != 0))
            {
                ApplyLeftForce();
            }
            if (IsOnRightWall())
            {
                RemoveForce("move_right");
            }
            else if (movingRight && ((oldValue & (int)ContactType.RIGHT) != 0))
            {
                ApplyRightForce();
            }
            base.ContactMaskChanged(oldValue);
        }

        public override void Jump()
        {
            if (IsOnGround())
            {
                ApplyImpulse(new Vector2(0, -JUMP_FORCE));
            }
        }

        public override void StartMovingLeft()
        {
            if (!movingLeft)
            {
                movingLeft = true;
                ApplyLeftForce();
            }
        }

        public override void StartMovingRight()
        {
            if (!movingRight)
            {
                movingRight = true;
                ApplyRightForce();
            }
        }

        public override void StopMovingLeft()
        {
            if (movingLeft)
            {
                movingLeft = false;
                RemoveForce("move_left");
            }
        }

        public override void StopMovingRight()
        {
            if (movingRight)
            {
                movingRight = false;
                RemoveForce("move_right");
            }
        }

        private void ApplyLeftForce()
        {
            if (!IsOnLeftWall())
            {
                AddForce(new Vector2(-WALK_FORCE, 0), "move_left", new Vector2(-MAX_SPEED, 0));
            }
        }

        private void ApplyRightForce()
        {
            if (!IsOnRightWall())
            {
                AddForce(new Vector2(WALK_FORCE, 0), "move_right", new Vector2(MAX_SPEED, 0));
            }
        }
    }
}
