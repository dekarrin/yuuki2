using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Yuuki2TheGame
{
    enum MouseButton
    {
        LEFT,
        RIGHT,
        MIDDLE,
        X1,
        X2
    }

    class KeyEventArgs : EventArgs
    {
        public Keys key;

        public TimeSpan timeDown;

        /// <summary>
        /// Amount of time after the time limit threshold that the key was held down for.
        /// </summary>
        public TimeSpan timeActive;

        /// <summary>
        /// The number of times since last key down that this key event has fired. The first firing will have
        /// this set to 1, the second to 2, etc.
        /// </summary>
        public long fireCount;

        public KeyEventArgs(Keys key, long downTicks, long minTimeTicks, long fireCount)
        {
            int down = (int)(((double)downTicks) / 10000.0);
            int minTime = (int)(((double)minTimeTicks) / 10000.0);
            this.key = key;
            this.timeActive = new TimeSpan(0, 0, 0, 0, down - minTime);
            this.timeDown = new TimeSpan(0, 0, 0, 0, down);
            this.fireCount = fireCount;
        }
    }

    class MouseEventArgs : EventArgs
    {
        public int x;

        public int y;

        public MouseEventArgs(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    class MouseMoveEventArgs : MouseEventArgs
    {
        public Point start;

        public Point delta;

        /// <summary>
        /// measured in pixels per second.
        /// </summary>
        public double speed;

        /// <summary>
        /// Time it took to move the cursor that far.
        /// </summary>
        public TimeSpan time;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">The current x-coordinate of the cursor</param>
        /// <param name="y">The current y-coordinate of the cursor</param>
        /// <param name="startX">Where the cursor ended</param>
        /// <param name="startY">Where the cursor started</param>
        /// <param name="ms">Time it took to move this far, in millliseconds</param>
        public MouseMoveEventArgs(int x, int y, int startX, int startY, long ticks)
            : base(x, y)
        {
            int ms = (int)(((double)ticks) / 10000.0);
            int dx = x - startX;
            int dy = y - startY;
            double dist = Math.Sqrt((dx * dx) + (dy * dy));
            start = new Point(startX, startY);
            delta = new Point(dx, dy);
            time = new TimeSpan(0, 0, 0, 0, ms);
            speed = dist / (((double)ms) * 1000.0);
        }
    }

    class MouseButtonEventArgs : MouseEventArgs
    {
        MouseButton button;

        /// <summary>
        /// Time that the button was held down for.
        /// </summary>
        public TimeSpan timeDown;

        /// <summary>
        /// Amount of time after the time limit threshold that the button was held down for.
        /// </summary>
        public TimeSpan timeActive;

        /// <summary>
        /// The number of times since last mouse down that this key event has fired. The first firing will have
        /// this set to 1, the second to 2, etc.
        /// </summary>
        public long fireCount;

        public MouseButtonEventArgs(int x, int y, MouseButton button, long downTicks, long minTimeTicks, long fireCount)
            : base(x, y)
        {
            int down = (int)(((double)downTicks) / 10000.0);
            int minTime = (int)(((double)minTimeTicks) / 10000.0);
            this.button = button;
            this.timeActive = new TimeSpan(0, 0, 0, 0, down - minTime);
            this.timeDown = new TimeSpan(0, 0, 0, 0, down);
            this.fireCount = fireCount;
        }
    }

    class MouseScrollEventArgs : MouseEventArgs
    {
        public int value;

        public int delta;

        /// <summary>
        /// Measured in values per second.
        /// </summary>
        public double speed;

        /// <summary>
        /// Time it took to get this measurement. Measured in milliseconds.
        /// </summary>
        public int time;

        public MouseScrollEventArgs(int x, int y, int value, int oldValue, long ticks)
            : base(x, y)
        {
            int ms = (int)(((double)ticks) / 10000.0);
            this.value = value;
            delta = oldValue - value;
            time = ms;
            speed = Math.Abs(delta) / (((double)ms) * 1000.0);
        }
    }

    class MouseDragEventArgs : MouseMoveEventArgs
    {
        public Rectangle dragRect;

        public MouseDragEventArgs(int x, int y, int startX, int startY, int ms)
            : base(x, y, startX, startY, ms)
        {
            dragRect = new Rectangle(this.start.X, this.start.Y, this.delta.X + 1, this.delta.Y + 1);
        }
    }

    delegate void KeyAction(KeyEventArgs args);

    delegate void MouseButtonAction(MouseButtonEventArgs args);

    delegate void MouseDragAction(MouseDragEventArgs args);

    delegate void MouseMoveAction(MouseMoveEventArgs args);

    delegate void MouseScrollAction(MouseScrollEventArgs args);

    /// <summary>
    /// Events for mouse buttons will fire in a specific order.
    /// 
    /// If the mouse is kept in the same place, the events fire in the following order, though
    /// whether it goes through all events depends on how many times the user clicks:
    /// OnDown -> OnUp -> OnClick -> OnDown -> OnClickAndDown -> OnUp -> OnDoubleClick
    /// 
    /// If the mouse is dragged over an area, the events fire in the following order:
    /// OnDown -> OnDragStart -> OnUp -> OnDragFinish
    /// 
    /// Of course, events only have an effect if they have delegates assigned to them.
    /// </summary>
    class ControlManager
    {
        #region handler definitions

        private class KeyHandler
        {
            public Keys key;

            public bool pushed = false;

            /// <summary>
            /// Number of times the down event fired.
            /// </summary>
            public long fireCount = 0;

            /// <summary>
            /// Number of times the down event is allowed to fire. 0 for unlimited.
            /// </summary>
            public long fireLimit = 0;

            /// <summary>
            /// Minimum ticks for the key to be held down before an event is fired.
            /// </summary>
            public long minTime = 0;

            public long startTime = 0;

            public KeyAction OnDown = null;

            public KeyAction OnUp = null;
        }

        private class MouseButtonHandler
        {
            public MouseButton button;

            public bool pushed = false;

            public long minTime = 0;

            public long startTime = 0;

            /// <summary>
            /// Number of times the down event fired.
            /// </summary>
            public long fireCount = 0;

            /// <summary>
            /// Number of times the down event is allowed to fire. 0 for unlimited.
            /// </summary>
            public long fireLimit = 0;

            public int dragStartX = -1;

            public int dragStartY = -1;

            public MouseButtonAction OnDown = null;

            public MouseButtonAction OnUp = null;

            /// <summary>
            /// Fired when the button is pressed and the mouse is moved.
            /// </summary>
            public MouseDragAction OnDragStart = null;

            /// <summary>
            /// Fired when the button is pressed, the mouse is moved, and then the button is released.
            /// </summary>
            public MouseDragAction OnDragFinish = null;

            /// <summary>
            /// Fired when the button is pressed and released without moving the mouse.
            /// </summary>
            public MouseButtonAction OnClick = null;

            /// <summary>
            /// Fired when the button is clicked and then pressed down quickly without moving the mouse.
            /// This is the 'click-and-a-half'.
            /// </summary>
            public MouseButtonAction OnClickAndDown = null;

            /// <summary>
            /// Fired when the button is clicked twice in quick succession.
            /// </summary>
            public MouseButtonAction OnDoubleClick = null;
        }

        private class MouseMoveHandler
        {
            public int startX = -1;

            public int startY = -1;

            public int minDeltaX = 0;

            public int minDeltaY = 0;

            public int minDelta = 0;

            public long fireInterval = 0;

            public long lastFire = 0;

            public MouseMoveAction OnMove = null;
        }

        private class MouseScrollHandler
        {
            public int minDelta = 0;

            public int startValue = 0;

            public long fireInterval;

            public long lastFire = 0;

            public MouseScrollAction OnScrollUp = null;

            public MouseScrollAction OnScrollDown = null;

        }

        #endregion

        #region handler attributes

        private IDictionary<Keys, KeyHandler> keyHandlers = new Dictionary<Keys, KeyHandler>();

        private IDictionary<MouseButton, MouseButtonHandler> buttonHandlers = new Dictionary<MouseButton, MouseButtonHandler>();

        private IList<MouseMoveHandler> moveHandlers = new List<MouseMoveHandler>();

        private IList<MouseScrollHandler> scrollHandlers = new List<MouseScrollHandler>();

        #endregion
        
        /// <summary>
        /// Number of pixels mouse can move between button down and button up and not be considered a drag.
        /// This should not be set very high, lest you greatly confuse your users! Recommend 0-2, certainly
        /// not anything above 8 unless you REALLY know what you're doing!
        /// </summary>
        public int ClickMoveTolerance { get; set; }

        /// <summary>
        /// Amount of time allowed between two mouse button presses to be considered a
        /// double-click / click-and-a-half. Measured in milliseconds.
        /// </summary>
        public int DoubleClickTimeTolerance { get; set; }
        
        public ControlManager()
        {
            ClickMoveTolerance = 1;
            DoubleClickTimeTolerance = 500;
        }

        public void Update(GameTime gt)
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState mse = Mouse.GetState();
            UpdateKeys(gt, kb);
            UpdateMouseMovement(gt, mse);
            UpdateMouseScroll(gt, mse);
            UpdateMouseButtons(gt, mse);
        }

        private void UpdateMouseButtons(GameTime gt, MouseState mse)
        {

        }

        private void UpdateMouseScroll(GameTime gt, MouseState mse)
        {

        }

        private void UpdateMouseMovement(GameTime gt, MouseState mse)
        {
            foreach (MouseMoveHandler hlr in moveHandlers)
            {
                //new TimeSpan()
            }
        }

        private void UpdateKeys(GameTime gt, KeyboardState kb)
        {
            foreach (KeyHandler hlr in keyHandlers.Values)
            {
                long down = gt.TotalGameTime.Ticks - hlr.startTime;
                CheckKeyState(hlr, kb, down, gt.TotalGameTime.Ticks);
                CheckKeyDownFire(hlr, down);
            }
        }

        private void CheckKeyState(KeyHandler handler, KeyboardState kb, long timeDown, long curTime)
        {
            if (!handler.pushed && kb.IsKeyDown(handler.key))
            {
                handler.pushed = true;
                handler.fireCount = 0;
                handler.startTime = curTime;
            }
            else if (handler.pushed && kb.IsKeyUp(handler.key))
            {
                handler.pushed = false;
                handler.fireCount = 0;
                handler.startTime = 0;
                if (handler.OnUp != null && timeDown >= handler.minTime)
                {
                    handler.OnUp(new KeyEventArgs(handler.key, timeDown, handler.minTime, 1));
                }
            }
        }

        private void CheckKeyDownFire(KeyHandler handler, long timeDown)
        {
            if (handler.pushed && handler.OnDown != null && timeDown >= handler.minTime)
            {
                if (handler.fireCount < handler.fireLimit || handler.fireLimit == 0)
                {
                    handler.fireCount++;
                    handler.OnDown(new KeyEventArgs(handler.key, timeDown, handler.minTime, handler.fireCount));
                }
            }
        }
    }
}
