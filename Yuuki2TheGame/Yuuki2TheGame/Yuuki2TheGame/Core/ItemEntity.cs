using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Yuuki2TheGame.Physics;

namespace Yuuki2TheGame.Core
{

    /// <summary>
    /// Holds items in the map.
    /// </summary>
    class ItemEntity : ActiveEntity
    {

        public const int ITEM_SIZE = 10;

        public delegate void PickedHandler(ItemEntity sender);

        public event PickedHandler OnPicked;

        public Item Item { get; private set; }

        public ItemEntity(Item item, Point pos)
            : base(new Point(ITEM_SIZE, ITEM_SIZE), pos)
        {
            this.Item = item;
            this.Texture = item.Texture;
        }

        protected override void ContactMaskChanged(int oldValue)
        {
            if (IsOnGround() && ((oldValue & (int)ContactType.DOWN) == 0))
            {
                Engine.AudioEngine.PlayItemContact();
            }
            base.ContactMaskChanged(oldValue);
        }

        public Item PickUp()
        {
            if (OnPicked != null)
            {
                Engine.AudioEngine.PlayItemPickup();
                OnPicked(this);
            }
            return Item;
        }

    }
}
