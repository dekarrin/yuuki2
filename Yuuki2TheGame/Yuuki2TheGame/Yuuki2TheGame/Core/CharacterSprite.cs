using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

//===============================================================================================================================================
//
//       Primary Author:    Corey Arneson
//         Contributors:    -NA-
//              Purpose:    Handle all the visual aspects of an NPC or Player such as Movement, Apparel, Race, Etc.
//                          An instance will be saved in each 'Character' object and only drawn if that 'Character' is in view of the camera
//              Version:    1.0
//
//===============================================================================================================================================


namespace Yuuki2TheGame.Core
{
    public class CharacterSprite
    {

        //=========================
        //  Texture Declarations
        //=========================

        static public Texture2D CharacterSpriteSheet;   // Texture for the actual Tile Sheet Asset: located in ~root~/Sprites/CharacterSprites.png

        //=========================
        //  Variable Declarations
        //=========================

        private int charStance;             // 0: idle, 1:walking, 2:swimming, 3:ascending/descending, 4:sleeping, 5:sitting...
        private Vector2 origin;             // Character origin (XXXX.aa, YYYY.bb) position ( width/2, height )

        //===   Character's Sprite Bounds   ===
        private Rectangle charFrame;        // Entire Character: 16 x 48px
        private Rectangle upperFrame;       // Head Slot: 16 x 16px
        private Rectangle middleFrame;      // Torso Slot: 16 x 16px
        private Rectangle lowerFrame;       // Legs Slot: 16 x 16px



        //===   Sprite IDs   ===
        private static int nullSpriteID = 32;        // last tile position and has no image data at all

        private int eyeId,              // Eye color
                    armSetId,           // Arm Set  ( mage robe, armor, etc...)
                    legSetId,           // Leg Set  ( above ^ )
                    footSetId,          // Foot Set ( above ^ )
                    handSetId,          // Hand Set ( above ^ )
                    torsoId,            // Torso    ( above ^ )
                    pelvisId,           // Pelvis   ( above ^ )
                    hairId,             // Hair     ( Must be seperate from head so as to preserve a characters hairstyle when removing a headgear item )
                    headId,             // Headgear ( helmets, crowns, hoods, etc...)
                    backId,             // Apparel Based Back Accesory ( Capes, backpack, etc...) !!! Quivers and scabards will be handled by the ItemSprite associated with the Selected Object !!!
                    skinId;             // ID of the Head associated with characters Race ( Serves as an index to retrieve other skin assets )

      

        private int blinkId;            // Temporary eyeId when blinking
        



        //===   Flags   ===
        private bool isVisible,         // IF in view, draw otherwise do nothing
                     isPlayer,          // IF player, use curosr mechanics
                     isBlinking,        // True: Reset Eyecolor, False: Wait for next blink
                     isLookingRight,    // True: Draw facing right, False: Draw facing left
                     isMale,            // True: Draw male sprites, False: Draw female sprites                                          !!!! NOT CURRENTLY IMPLEMENTED !!!
                     isClimbing,        // True: Ignore isLookingRight and draw associated special sprites, False: nothing              !!!! NOT CURRENTLY IMPLEMENTED !!!
                     isSwimming,        // True: Agknowledge isLookingRight and draw associated special sprites, False: nothing         !!!! NOT CURRENTLY IMPLEMENTED !!!
                     isWearingHeadgear; // True: Draw headgear and not hair, False: Draw hair and not headgear



        //=========================
        //      Constructors
        //=========================

        // Constructor taking all possibe input
        public CharacterSprite(bool player, float x, float y, int skin, int eye, int hair, int torso, int arm, int hand, int pelvis, int leg, int foot, int head, int back, bool male, bool lookright, bool headgear, int stance)
        {
            skinId = skin;
            eyeId = eye;
            hairId = hair;
            torsoId = torso;
            armSetId = arm;
            handSetId = hand;
            pelvisId = pelvis;
            legSetId = leg;
            footSetId = foot;
            headId = head;
            backId = back;

            charStance = stance;
            blinkId = eye;

            origin = new Vector2(x, y);

            isPlayer = player;

            if (isPlayer)
                isVisible = true;

            isMale = male;
            isLookingRight = lookright;
            isWearingHeadgear = headgear;
            isBlinking = false;

            //Create the draw frames for each sprite 'zone'
            charFrame = new Rectangle(Camera.PixPosFromVector2(origin).X - 8, Camera.PixPosFromVector2(origin).Y - 48, 16, 48);
            upperFrame  = new Rectangle(charFrame.X, charFrame.Y,      charFrame.Width, charFrame.Width);
            middleFrame = new Rectangle(charFrame.X, charFrame.Y + 16, charFrame.Width, charFrame.Width);
            lowerFrame  = new Rectangle(charFrame.X, charFrame.Y + 32, charFrame.Width, charFrame.Width);
        }

        public CharacterSprite(bool player,float x, float y, int skin, int eye, int hair, bool male, bool lookright, int stance)    // Near-Nude Basic Creation...
        {
            skinId = skin;
            eyeId = eye;
            hairId = hair;
            torsoId = skinId;
            armSetId = skinId;
            handSetId = skinId;
            pelvisId = skinId;
            legSetId = skinId;
            footSetId = skinId;
            headId = skinId;
            backId = skinId;

            charStance = stance;
            blinkId = eye;

            origin = new Vector2(x, y);

            isPlayer = player;

            if (isPlayer)
                isVisible = true;           // Player is always visible

            else if (   Camera.PixPosFromVector2(origin).X > -32 && Camera.PixPosFromVector2(origin).X < 832    // if NPC is within 2 blocks of camera, they are visible
                     && Camera.PixPosFromVector2(origin).Y > -32 && Camera.PixPosFromVector2(origin).Y < 632)
            {
                isVisible = true;
            }
            else isVisible = false;        // otherwise they are not visible and do not need to be drawn

            isMale = male;
            isLookingRight = lookright;
            isWearingHeadgear = false;
            isBlinking = false;

            //Create the draw frames for each sprite 'zone'
            charFrame = new Rectangle(Camera.PixPosFromVector2(origin).X - 8, Camera.PixPosFromVector2(origin).Y - 48, 16, 48);
            upperFrame = new Rectangle(charFrame.X, charFrame.Y, charFrame.Width, charFrame.Width);
            middleFrame = new Rectangle(charFrame.X, charFrame.Y + 16, charFrame.Width, charFrame.Width);
            lowerFrame = new Rectangle(charFrame.X, charFrame.Y + 32, charFrame.Width, charFrame.Width);
        }


        //=========================
        //  Core Functionality
        //=========================
        

        // Main Update for this CharacterSprite
        void Update( GameTime gametime)
        {
            //Cursor Mechanics
            MouseState ms = new MouseState();

            if (isPlayer)                           // if I am a player...
            {
                if (ms.X > origin.X && ms.X > 0 && ms.X < 800 && ms.Y > 0 && ms.Y < 600)                // if cursor is to the right of 'me" look right
                {
                    isLookingRight = true;
                }
                else if (ms.X < origin.X && ms.X > 0 && ms.X < 800 && ms.Y > 0 && ms.Y < 600)           // if cursor is to the left of 'me' look left
                {
                    isLookingRight = false;
                }
            }

            //Animation Mechanics
            UpdateAnimation(gametime,5);    // Update all sprite animations at 5 Ms/Frame (~Ticks/Frame)

        }

        void RefreshSprite(GameTime current,EquippedSet equiped)
        {

        }

        // Main Draw for this CharacterSprite
        void Draw( GameTime gametime, SpriteBatch GlobalSpriteBatchObject)
        {
            if (isVisible)
            {
                switch (charStance)
                {
                    case 0: // Idle
                        PaintIDLE(GlobalSpriteBatchObject);
                        break;

                    case 1: // Walking
                        PaintWALK(GlobalSpriteBatchObject);
                        break;

                    case 2: // Swimming
                        PaintSWIM(GlobalSpriteBatchObject);
                        break;

                    case 3: // Climbing
                        PaintCLIMB(GlobalSpriteBatchObject);
                        break;
                }
            }

        }

        // Saves all data into a single line record
        public String SaveCharacter()
        {
            // Generate a record
            string saveOutput = "#" + SkinId + EyeId + HairId + TorsoId + ArmSetId +  HandSetId +  PelvisId +  LegSetId + FootSetId + HeadId + BackId 
                                    + saveBool(isMale) + saveBool(isLookingRight) + saveBool(isWearingHeadgear) + origin.X + origin.Y + charStance +  saveBool(isPlayer) + "\n";
            return saveOutput;                                
        }

        // Parses a singleline record and creates a CharacterSprite
        public CharacterSprite LoadCharacter(string record)
        {
            //Temp Vars for parsing
            float x, y;
            int a, b, c, d, e, f, g, h, i, j, k, o;
            bool l, m, n, p;
           
            // Parse the record
            a = Convert.ToInt16(record.Substring(1, 2));
            b = Convert.ToInt16(record.Substring(3, 2));
            c = Convert.ToInt16(record.Substring(5, 2));
            d = Convert.ToInt16(record.Substring(7, 2));
            e = Convert.ToInt16(record.Substring(9, 2));
            f = Convert.ToInt16(record.Substring(11, 2));
            g = Convert.ToInt16(record.Substring(13, 2));
            h = Convert.ToInt16(record.Substring(15, 2));
            i = Convert.ToInt16(record.Substring(17, 2));
            j = Convert.ToInt16(record.Substring(19, 2));
            k = Convert.ToInt16(record.Substring(21, 2));
            l = loadBool(record.Substring(23, 1));
            m = loadBool(record.Substring(24, 1));
            n = loadBool(record.Substring(25, 1));
            x = (float)(Convert.ToInt16(record.Substring(26, 4) + Convert.ToInt16(record.Substring(31,2))/10));   // 'XXXX.aa' where 'XXXX' is grid position, 'aa' the partial pixel position of '00' to '15'
            y = (float)(Convert.ToInt16(record.Substring(33, 4) + Convert.ToInt16(record.Substring(38,2))/10));   // 'YYYY.bb' where 'YYYY' is grid position, 'bb' the partial pixel position of '00' to '15'
            o = Convert.ToInt16(record.Substring(40,1));
            p = loadBool(record.Substring(41, 1));

            return new CharacterSprite(p, x, y, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o);
        }


        //=========================
        //  Useful Sub-Methods
        //=========================

        // Retrieve Sprite based off of ID
        private Rectangle getSpriteFromID(int id)
        {
            Rectangle rect;

            if (isLookingRight) // forward read sprite
            {
                rect = new Rectangle(id % 16, id / 16, 16, 16);
            }

            else                // reverse read sprite
            {
                rect = new Rectangle((id + 1) % 16, id / 16, -16, 16);
            }

            return rect;
        }

        //Convert  t or f to bool
        private bool loadBool(string input)
        {
            if (input.Equals("t"))
                return true;
            else
                return false;
        }

        // Convert bool to t or f
        private string saveBool(bool input)
        {
            if (input)
                return "t";
            else
                return "f";
        }

        // Draw calls for idle stance
        private void PaintIDLE(SpriteBatch Painter)
        {
            Painter.Begin();        // Start this 'Batch'

            Painter.Draw(CharacterSpriteSheet, middleFrame, getSpriteFromID(HandSetId + 1), Color.White);   //BackHand
            Painter.Draw(CharacterSpriteSheet, middleFrame, getSpriteFromID(ArmSetId + 1), Color.White);    //BackArm
            Painter.Draw(CharacterSpriteSheet, lowerFrame,  getSpriteFromID(FootSetId + 1), Color.White);   //BackFoot
            Painter.Draw(CharacterSpriteSheet, lowerFrame,  getSpriteFromID(LegSetId + 1), Color.White);    //BackLeg

            if (PelvisId == SkinId)
                Painter.Draw(CharacterSpriteSheet, lowerFrame, getSpriteFromID(SkinId + 32), Color.White);  //Pelvis without apparel;
            else
                Painter.Draw(CharacterSpriteSheet, lowerFrame, getSpriteFromID(PelvisId), Color.White);     //Pelvis

            if (TorsoId == SkinId)
                Painter.Draw(CharacterSpriteSheet, middleFrame, getSpriteFromID(SkinId + 16), Color.White); //Torso without apparel;
            else
                Painter.Draw(CharacterSpriteSheet, middleFrame, getSpriteFromID(TorsoId), Color.White);     //Torso

            Painter.Draw(CharacterSpriteSheet, upperFrame, getSpriteFromID(SkinId), Color.White);           //Skin of face
            Painter.Draw(CharacterSpriteSheet, upperFrame, getSpriteFromID(EyeId), Color.White);            //Eye color
                
            if (isWearingHeadgear)
                Painter.Draw(CharacterSpriteSheet, upperFrame, getSpriteFromID(HeadId), Color.White);       //Headgear
            else
                Painter.Draw(CharacterSpriteSheet, upperFrame, getSpriteFromID(HairId), Color.White);       //Hair

            Painter.Draw(CharacterSpriteSheet, middleFrame, getSpriteFromID(HandSetId), Color.White);       //FrontHand
            Painter.Draw(CharacterSpriteSheet, middleFrame, getSpriteFromID(ArmSetId), Color.White);        //FrontArm
            Painter.Draw(CharacterSpriteSheet, lowerFrame,  getSpriteFromID(FootSetId), Color.White);       //FrontFoot
            Painter.Draw(CharacterSpriteSheet, lowerFrame,  getSpriteFromID(LegSetId), Color.White);        //FrontLeg

            Painter.End();      // Finish this 'Batch'
        }

        // Draw calls for moving stance
        private void PaintWALK(SpriteBatch Painter)
        {

        }       //  !!! PLACEHOLDER !!!

        // Draw calls for swimming stance
        private void PaintSWIM(SpriteBatch Painter)
        {

        }       //  !!! PLACEHOLDER !!!

        // Draw calls for climbing stance
        private void PaintCLIMB(SpriteBatch Painter)
        {

        }       //  !!! PLACEHOLDER !!!




        ///<summary>         !!!!   Replaced by Camera.PixPosFromVector2()   !!!!
       //      
       //  !!! THIS IS A PLACEHOLDER FOR A METHOD THAT WILL CONVERT A 'CCCC.aa' COORDINATE TO PIXEL POSITIONS OF THE SCREEN !!!
       // Calculate the current pixel coordinates of a given double of format 'CCCC.aa' representing a partial grid location
       // The given bool "horizontal" denotes whether to calculate a width pixel position or a height pixel position
       // private Point PixelFromPartialGrid(Vector2 coord)
       // {
       //     Point pixelPosition;
       //     return Point.Zero;
       // }
       ///</summary>
       ///

        // Main Animation Sub-Method for Update 
        private void UpdateAnimation(GameTime currentTime, int MsPerFr)
        {
            int MPF = MsPerFr;            //Milliseconds per Frame
            if (currentTime.ElapsedGameTime.Milliseconds % MPF == 0)
            {
                switch (charStance)
                {
                    case 0:
                            
                        break;

                    case 1:

                        break;

                    case 2:

                        break;


                }

                // Blinking
                if (isBlinking && currentTime.ElapsedGameTime.Milliseconds % MPF * 3 == 0)  // If my eyes are closed and they have been for 3 frames then:
                {
                    EyeId = blinkId;            // return it to eyecolor id
                    isBlinking = false;   // stop current 'blink'
                }
                else                                                                        // If my eyes are open...
                {
                    int ranDur = new Random().Next(5,30);   
                    if (currentTime.ElapsedGameTime.Milliseconds % MPF * ranDur == 0)       // ...and they have been so for 5 - 30 frames then: 
                    {
                        EyeId = nullSpriteID;   // temporarily set 'eyeId' to nothing and then switch back
                        isBlinking = true;
                    }
                }
                

                
            }
        }


        //============================
        //  Accessors and Modifiers
        //============================

        public int SkinId
        {
            get { return skinId; }
            set { skinId = value; }
        }

        public int BackId
        {
            get { return backId; }
            set { backId = value; }
        }

        public int HeadId
        {
            get { return headId; }
            set { headId = value; }
        }

        public int HairId
        {
            get { return hairId; }
            set { hairId = value; }
        }

        public int PelvisId
        {
            get { return pelvisId; }
            set { pelvisId = value; }
        }

        public int TorsoId
        {
            get { return torsoId; }
            set { torsoId = value; }
        }

        public int HandSetId
        {
            get { return handSetId; }
            set { handSetId = value; }
        }

        public int FootSetId
        {
            get { return footSetId; }
            set { footSetId = value; }
        }

        public int LegSetId
        {
            get { return legSetId; }
            set { legSetId = value; }
        }

        public int ArmSetId
        {
            get { return armSetId; }
            set { armSetId = value; }
        }

        public int EyeId
        {
            get { return eyeId; }
            set { eyeId = value; }
        }

    }
}
