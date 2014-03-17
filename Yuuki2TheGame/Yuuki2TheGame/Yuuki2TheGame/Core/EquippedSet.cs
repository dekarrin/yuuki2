using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuuki2TheGame.Core
{
    //===============================================================================================================================================
    //
    //       Primary Author:    Corey Arneson
    //         Contributors:    -NA-
    //              Purpose:    Handle the "currently equipped" slots of inventory
    //                          An instance will be saved in each 'Character' object to allow access to these "Slots"
    //              Version:    1.0
    //
    //===============================================================================================================================================

    class EquippedSet
    {
        //=========================
        //  Variable Declarations
        //=========================

        private Item    headItem,           // Headgear
                        torsoItem,          // Torso apparel
                        legsItem,           // Leg apparel
                        handsItem,          // Hand apparel
                        feetItem,           // Foot apparel
                        backItem,           // Back apparel ( capes, cloaks, backpacks, etc.)

                        accessory1,         // Accessories that exhibit "properties" but are not drawn in sprite ( rings, necklaces, tokens, etc.)
                        accessory2,         // see above ^
                        accessory3,         // see above ^
                        accessory4,         // see above ^

                        inhandLeft,         //Weapon/item in Left hand
                        inhandRight,        //Weapon/item in Right hand
                        inhandBoth;         //Weapon/item in Both hands

        //=========================
        //      Constructors
        //=========================

        public EquippedSet()            // Have nothing equipped
        {
            headItem = null;
            torsoItem = null;
            legsItem = null;
            handsItem = null;
            feetItem = null;
            backItem = null;

            accessory1 = null;
            accessory2 = null;
            accessory3 = null;
            accessory4 = null;

            InhandBoth = null;
            InhandLeft = null;
            InhandRight = null;
        }

        public EquippedSet(Item head, Item torso, Item legs, Item hands, Item feet, Item back, Item acc1, Item acc2, Item acc3, Item acc4)  // Have all but "In hands"
        {
            HeadItem = head;
            TorsoItem = torso;
            LegsItem = legs;
            HandsItem = hands;
            FeetItem = feet;
            BackItem = back;

            Accessory1 = acc1;
            Accessory2 = acc2;
            Accessory3 = acc3;
            Accessory4 = acc4;

            InhandBoth = null;
            InhandLeft = null;
            InhandRight = null;

        }

        public EquippedSet(Item head, Item torso, Item legs, Item hands, Item feet, Item back, Item acc1, Item acc2, Item acc3, Item acc4,Item Lhand, Item Rhand)   // Have all and both hands distinct
        {
            HeadItem = head;
            TorsoItem = torso;
            LegsItem = legs;
            HandsItem = hands;
            FeetItem = feet;
            BackItem = back;

            Accessory1 = acc1;
            Accessory2 = acc2;
            Accessory3 = acc3;
            Accessory4 = acc4;

            InhandBoth = null;
            InhandLeft = Lhand;
            InhandRight = Rhand;

        }

        public EquippedSet(Item head, Item torso, Item legs, Item hands, Item feet, Item back, Item acc1, Item acc2, Item acc3, Item acc4, Item Both)   // Have all and both hands as one
        {
            HeadItem = head;
            TorsoItem = torso;
            LegsItem = legs;
            HandsItem = hands;
            FeetItem = feet;
            BackItem = back;

            Accessory1 = acc1;
            Accessory2 = acc2;
            Accessory3 = acc3;
            Accessory4 = acc4;

            InhandBoth = Both;
            InhandLeft = null;
            InhandRight = null;

        }


        ///================================================
        ///         Properties and Other Modifiers
        ///================================================

        public Item InhandBoth
        {
            get { return inhandBoth; }
            set 
            {
                inhandBoth = value;
                if(value != null)
                {   
                    InhandLeft = null;
                    InhandRight = null;
                }
            }
        }

        public Item InhandRight
        {
            get { return inhandRight; }
            set 
            {
                inhandRight = value;
                if (value != null)
                {
                    InhandBoth = null;
                }
            }
        }

        public Item InhandLeft
        {
            get { return inhandLeft; }
            set
            {
                inhandLeft = value;
                if (value != null)
                {
                    InhandBoth = null;
                }
            }
        }

        public Item Accessory4
        {
            get { return accessory4; }
            set { accessory4 = value; }
        }

        public Item Accessory3
        {
            get { return accessory3; }
            set { accessory3 = value; }
        }

        public Item Accessory2
        {
            get { return accessory2; }
            set { accessory2 = value; }
        }

        public Item Accessory1
        {
            get { return accessory1; }
            set { accessory1 = value; }
        }

        public Item BackItem
        {
            get { return backItem; }
            set { backItem = value; }
        }

        public Item FeetItem
        {
            get { return feetItem; }
            set { feetItem = value; }
        }

        public Item HandsItem
        {
            get { return handsItem; }
            set { handsItem = value; }
        }

        public Item LegsItem
        {
            get { return legsItem; }
            set { legsItem = value; }
        }

        public Item TorsoItem
        {
            get { return torsoItem; }
            set { torsoItem = value; }
        }

        public Item HeadItem
        {
            get { return headItem; }
            set { headItem = value; }
        }

                    
    }
}
