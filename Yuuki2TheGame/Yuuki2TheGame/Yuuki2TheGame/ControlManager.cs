using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Yuuki2TheGame.Extensions;

namespace Yuuki2TheGame
{
    public enum MouseButton
    {
        LEFT,
        RIGHT,
        MIDDLE,
        X1,
        X2
    }

    class KeyEventArgs : EventArgs
    {
        public readonly Keys Key;

        public readonly TimeSpan TimeDown;

        /// <summary>
        /// Amount of time after the time limit threshold that the key was held down for.
        /// </summary>
        public readonly TimeSpan TimeActive;

        /// <summary>
        /// The number of times since last key down that this key event has fired. The first firing will have
        /// this set to 1, the second to 2, etc.
        /// </summary>
        public readonly long FireCount;

        public KeyEventArgs(Keys key, long downTicks, long minTimeTicks, long fireCount)
        {
            int down = (int)(((double)downTicks) / 10000.0);
            int minTime = (int)(((double)minTimeTicks) / 10000.0);
            this.Key = key;
            this.TimeActive = new TimeSpan(0, 0, 0, 0, down - minTime);
            this.TimeDown = new TimeSpan(0, 0, 0, 0, down);
            this.FireCount = fireCount;
        }
    }

    class MouseEventArgs : EventArgs
    {
        public readonly int X;

        public readonly int Y;

        public MouseEventArgs(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    class MouseMoveEventArgs : MouseEventArgs
    {
        public readonly Point Start;

        public readonly Point Delta;

        /// <summary>
        /// measured in pixels per second.
        /// </summary>
        public readonly double Speed;

        /// <summary>
        /// Time it took to move the cursor that far.
        /// </summary>
        public readonly TimeSpan Time;

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
            Start = new Point(startX, startY);
            Delta = new Point(dx, dy);
            Time = new TimeSpan(0, 0, 0, 0, ms);
            Speed = dist / (((double)ms) / 1000.0);
        }
    }

    class MouseButtonEventArgs : MouseEventArgs
    {
        public readonly MouseButton Button;

        /// <summary>
        /// Time that the button was held down for.
        /// </summary>
        public readonly TimeSpan TimeDown;

        /// <summary>
        /// Amount of time after the time limit threshold that the button was held down for.
        /// </summary>
        public readonly TimeSpan TimeActive;

        /// <summary>
        /// The number of times since last mouse down that this key event has fired. The first firing will have
        /// this set to 1, the second to 2, etc.
        /// </summary>
        public readonly long FireCount;

        public MouseButtonEventArgs(int x, int y, MouseButton button, long downTicks, long minTimeTicks, long fireCount)
            : base(x, y)
        {
            int down = (int)(((double)downTicks) / 10000.0);
            int minTime = (int)(((double)minTimeTicks) / 10000.0);
            this.Button = button;
            this.TimeActive = new TimeSpan(0, 0, 0, 0, down - minTime);
            this.TimeDown = new TimeSpan(0, 0, 0, 0, down);
            this.FireCount = fireCount;
        }
    }

    class MouseScrollEventArgs : MouseEventArgs
    {
        public readonly int Value;

        public readonly int Delta;

        /// <summary>
        /// Measured in values per second.
        /// </summary>
        public readonly double Speed;

        /// <summary>
        /// Time it took to get this measurement. Measured in milliseconds.
        /// </summary>
        public readonly int Time;

        public MouseScrollEventArgs(int x, int y, int value, int oldValue, long ticks)
            : base(x, y)
        {
            int ms = (int)(((double)ticks) / 10000.0);
            this.Value = value;
            Delta = oldValue - value;
            Time = ms;
            Speed = Math.Abs(Delta) / (((double)ms) / 1000.0);
        }
    }

    class MouseDragEventArgs : MouseMoveEventArgs
    {
        public Rectangle DragArea;

        public MouseDragEventArgs(int x, int y, int startX, int startY, long ticks)
            : base(x, y, startX, startY, ticks)
        {
            DragArea = new Rectangle(Start.X, Start.Y, Delta.X + 1, Delta.Y + 1);
        }
    }

    delegate void KeyAction(KeyEventArgs args);

    delegate void MouseButtonAction(MouseButtonEventArgs args);

    delegate void MouseDragAction(MouseDragEventArgs args);

    delegate void MouseMoveAction(MouseMoveEventArgs args);

    delegate void MouseScrollAction(MouseScrollEventArgs args);

    delegate void MouseAction(MouseEventArgs args);

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
        public const string DEFAULT_NAME = "__DEFAULT__";

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

            public bool setStart = false;

            public bool startLocked = false;

            public bool inDrag = false;

            public bool firedClickDown = false;

            public long clickTime = 0;

            public int startX = 0;

            public int startY = 0;

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

            public long dragStartTime = 0;

            public MouseButtonAction OnDown = null;

            public MouseButtonAction OnUp = null;

            /// <summary>
            /// Fired when the button is pressed and the mouse is moved.
            /// </summary>
            public MouseAction OnDragStart = null;

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

            public int minDistX = 0;

            public int minDistY = 0;

            public double minDist = 0;

            public long checkInterval = 0;

            public long lastCheck = 0;

            public long lastFire = 0;

            public MouseMoveAction OnMove = null;
        }

        private class MouseScrollHandler
        {
            public int minDist = 0;

            public int startValue = 0;

            public long checkInterval = 0;

            public long lastCheck = 0;

            public long lastFire = 0;

            public MouseScrollAction OnScrollUp = null;

            public MouseScrollAction OnScrollDown = null;

        }

        #endregion

        #region handler attributes

        private IDictionary<string, KeyHandler> keyHandlers = new Dictionary<string, KeyHandler>();

        private IDictionary<string, MouseButtonHandler> buttonHandlers = new Dictionary<string, MouseButtonHandler>();

        private IDictionary<string, MouseMoveHandler> moveHandlers = new Dictionary<string, MouseMoveHandler>();

        private IDictionary<string, MouseScrollHandler> scrollHandlers = new Dictionary<string, MouseScrollHandler>();

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

        public void AddMouseScrollListener(string name, int minDistance, int checkInterval, MouseScrollAction scrollUpAction, MouseScrollAction scrollDownAction)
        {
            MouseState mse = Mouse.GetState();
            MouseScrollHandler msh = new MouseScrollHandler();
            msh.minDist = minDistance;
            msh.checkInterval = ((long)checkInterval) * 10000L;
            msh.OnScrollUp = scrollUpAction;
            msh.OnScrollDown = scrollDownAction;
            msh.startValue = mse.ScrollWheelValue;
            scrollHandlers[name] = msh;
        }

        public void RemoveMouseScrollListener(string name)
        {
            scrollHandlers.Remove(name);
        }

        public void BindMouseScrollUp(string listenerName, MouseScrollAction action)
        {
            if (!scrollHandlers.ContainsKey(listenerName))
            {
                AddMouseScrollListener(listenerName, 0, 0, null, null);
            }
            scrollHandlers[listenerName].OnScrollUp = action;
        }

        public void BindMouseScrollDown(string listenerName, MouseScrollAction action)
        {
            if (!scrollHandlers.ContainsKey(listenerName))
            {
                AddMouseScrollListener(listenerName, 0, 0, null, null);
            }
            scrollHandlers[listenerName].OnScrollDown = action;
        }

        public void BindMouseScrollUp(MouseScrollAction action) {
            BindMouseScrollUp(DEFAULT_NAME, action);
        }

        public void BindMouseScrollDown(MouseScrollAction action) {
            BindMouseScrollDown(DEFAULT_NAME, action);
        }

        public void AddMouseMoveListener(string name, int minDistX, int minDistY, double minDistTotal, int checkInterval, MouseMoveAction moveAction)
        {
            MouseState mse = Mouse.GetState();
            MouseMoveHandler mmh = new MouseMoveHandler();
            mmh.minDistX = minDistX;
            mmh.minDistY = minDistY;
            mmh.minDist = minDistTotal;
            mmh.checkInterval = ((long)checkInterval) * 10000L;
            mmh.startX = mse.X;
            mmh.startY = mse.Y;
            mmh.OnMove = moveAction;
            moveHandlers[name] = mmh;
        }

        public void RemoveMouseMoveListener(string name)
        {
            moveHandlers.Remove(name);
        }

        public void BindMouseMove(string listenerName, MouseMoveAction action)
        {
            if (!moveHandlers.ContainsKey(listenerName))
            {
                AddMouseMoveListener(listenerName, 0, 0, 0, 0, null);
            }
            moveHandlers[listenerName].OnMove = action;
        }

        public void BindMouseMove(MouseMoveAction action)
        {
            BindMouseMove(DEFAULT_NAME, action);
        }

        public void AddKeyListener(string name, Keys key, long downFireLimit, int minHoldTime, KeyAction downAction, KeyAction upAction)
        {
            KeyHandler kh = new KeyHandler();
            kh.key = key;
            kh.fireLimit = downFireLimit;
            kh.minTime = ((long) minHoldTime) * 10000L;
            kh.OnDown = downAction;
            kh.OnUp = upAction;
            keyHandlers[name] = kh;
        }

        public void AddKeyListener(Keys key, long downFireLimit, int minHoldTime, KeyAction downAction, KeyAction upAction) {
            AddKeyListener(DEFAULT_NAME + KeyName(key), key, downFireLimit, minHoldTime, downAction, upAction);
        }

        public void RemoveKeyListener(string name)
        {
            keyHandlers.Remove(name);
        }

        public void RemoveKeyListener(Keys key) {
            RemoveKeyListener(DEFAULT_NAME + KeyName(key));
        }

        public void BindKeyUp(string listenerName, Keys key, KeyAction action)
        {
            if (!keyHandlers.ContainsKey(listenerName))
            {
                AddKeyListener(listenerName, key, 0, 0, null, null);
            }
            keyHandlers[listenerName].OnUp = action;
        }

        public void BindKeyDown(string listenerName, Keys key, KeyAction action, bool fireContinuously)
        {
            if (!keyHandlers.ContainsKey(listenerName))
            {
                AddKeyListener(listenerName, key, ((fireContinuously) ? 0 : 1), 0, null, null);
            }
            keyHandlers[listenerName].OnDown = action;
        }

        public void BindKeyUp(Keys key, KeyAction action)
        {
            BindKeyUp(DEFAULT_NAME + KeyName(key), key, action);
        }

        public void BindKeyDown(Keys key, KeyAction action, bool fireContinuously)
        {
            BindKeyDown(DEFAULT_NAME + KeyName(key), key, action, fireContinuously);
        }

        public void AddMouseButtonListener(string name, MouseButton button, long downFireLimit, int minHoldTime, MouseButtonAction downAction, MouseButtonAction upAction, MouseAction dragStartAction, MouseDragAction dragFinishAction, MouseButtonAction clickAction, MouseButtonAction clickAndDownAction, MouseButtonAction doubleClickAction)
        {
            MouseButtonHandler mbh = new MouseButtonHandler();
            mbh.button = button;
            mbh.fireLimit = downFireLimit;
            mbh.minTime = ((long)minHoldTime) * 10000L;
            mbh.OnDown = downAction;
            mbh.OnUp = upAction;
            mbh.OnClick = clickAction;
            mbh.OnClickAndDown = clickAndDownAction;
            mbh.OnDoubleClick = doubleClickAction;
            mbh.OnDragStart = dragStartAction;
            mbh.OnDragFinish = dragFinishAction;
            buttonHandlers[name] = mbh;
        }

        public void AddMouseButtonListener(MouseButton button, long downFireLimit, int minHoldTime, MouseButtonAction downAction, MouseButtonAction upAction, MouseAction dragStartAction, MouseDragAction dragFinishAction, MouseButtonAction clickAction, MouseButtonAction clickAndDownAction, MouseButtonAction doubleClickAction)
        {
            AddMouseButtonListener(DEFAULT_NAME + ButtonName(button), button, downFireLimit, minHoldTime, downAction, upAction, dragStartAction, dragFinishAction, clickAction, clickAndDownAction, doubleClickAction);
        }

        public void RemoveMouseButtonListener(string name)
        {
            buttonHandlers.Remove(name);
        }

        public void RemoveMouseButtonListener(MouseButton button)
        {
            RemoveMouseButtonListener(DEFAULT_NAME + ButtonName(button));
        }

        public void BindMouseButtonDown(string name, MouseButton button, MouseButtonAction action, bool fireContinuously)
        {
            if (!buttonHandlers.ContainsKey(name))
            {
                AddMouseButtonListener(name, button, ((fireContinuously) ? 0 : 1), 0, null, null, null, null, null, null, null);
            }
            buttonHandlers[name].OnDown = action;
        }

        public void BindMouseButtonDown(MouseButton button, MouseButtonAction action, bool fireContinuously)
        {
            BindMouseButtonDown(DEFAULT_NAME + ButtonName(button), button, action, fireContinuously);
        }

        public void BindMouseButtonUp(string name, MouseButton button, MouseButtonAction action)
        {
            if (!buttonHandlers.ContainsKey(name))
            {
                AddMouseButtonListener(name, button, 0, 0, null, null, null, null, null, null, null);
            }
            buttonHandlers[name].OnUp = action;
        }

        public void BindMouseButtonUp(MouseButton button, MouseButtonAction action)
        {
            BindMouseButtonUp(DEFAULT_NAME + ButtonName(button), button, action);
        }

        public void BindMouseButtonClick(string name, MouseButton button, MouseButtonAction action)
        {
            if (!buttonHandlers.ContainsKey(name))
            {
                AddMouseButtonListener(name, button, 0, 0, null, null, null, null, null, null, null);
            }
            buttonHandlers[name].OnClick = action;
        }

        public void BindMouseButtonClick(MouseButton button, MouseButtonAction action)
        {
            BindMouseButtonClick(DEFAULT_NAME + ButtonName(button), button, action);
        }

        public void BindMouseButtonClickAndDown(string name, MouseButton button, MouseButtonAction action)
        {
            if (!buttonHandlers.ContainsKey(name))
            {
                AddMouseButtonListener(name, button, 0, 0, null, null, null, null, null, null, null);
            }
            buttonHandlers[name].OnClickAndDown = action;
        }

        public void BindMouseButtonClickAndDown(MouseButton button, MouseButtonAction action)
        {
            BindMouseButtonClickAndDown(DEFAULT_NAME + ButtonName(button), button, action);
        }

        public void BindMouseButtonDoubleClick(string name, MouseButton button, MouseButtonAction action)
        {
            if (!buttonHandlers.ContainsKey(name))
            {
                AddMouseButtonListener(name, button, 0, 0, null, null, null, null, null, null, null);
            }
            buttonHandlers[name].OnDoubleClick = action;
        }

        public void BindMouseButtonDoubleClick(MouseButton button, MouseButtonAction action)
        {
            BindMouseButtonDoubleClick(DEFAULT_NAME + ButtonName(button), button, action);
        }

        public void BindMouseButtonDragStart(string name, MouseButton button, MouseAction action)
        {
            if (!buttonHandlers.ContainsKey(name))
            {
                AddMouseButtonListener(name, button, 0, 0, null, null, null, null, null, null, null);
            }
            buttonHandlers[name].OnDragStart = action;
        }

        public void BindMouseButtonDragStart(MouseButton button, MouseAction action)
        {
            BindMouseButtonDragStart(DEFAULT_NAME + ButtonName(button), button, action);
        }

        public void BindMouseButtonDragFinish(string name, MouseButton button, MouseDragAction action)
        {
            if (!buttonHandlers.ContainsKey(name))
            {
                AddMouseButtonListener(name, button, 0, 0, null, null, null, null, null, null, null);
            }
            buttonHandlers[name].OnDragFinish = action;
        }

        public void BindMouseButtonDragFinish(MouseButton button, MouseDragAction action)
        {
            BindMouseButtonDragFinish(DEFAULT_NAME + ButtonName(button), button, action);
        }

        private string KeyName(Keys k)
        {
            return Enum.GetName(typeof(Keys), k);
        }

        private string ButtonName(MouseButton b)
        {
            return Enum.GetName(typeof(MouseButton), b);
        }

        private void UpdateMouseButtons(GameTime gt, MouseState mse)
        {
            long time = gt.TotalGameTime.Ticks;
            foreach (MouseButtonHandler h in buttonHandlers.Values)
            {
                bool justPushed = false;
                long timeDown = time - h.startTime;
                long timeLastClick = time - h.clickTime;
                int dx = mse.X - h.startX;
                int dy = mse.Y - h.startY;
                int tol = ClickMoveTolerance;
                bool inClickBounds = (dx <= tol && dy <= tol);
                if (!h.pushed && mse.IsButtonDown(h.button))
                {
                    h.pushed = true;
                    h.fireCount = 0;
                    h.startTime = time;
                    h.inDrag = false;
                    if ((timeLastClick / 10000.0) > DoubleClickTimeTolerance)
                    {
                        h.startX = mse.X;
                        h.startY = mse.Y;
                        h.firedClickDown = false;
                    }
                    justPushed = true;
                }
                else if (h.pushed && mse.IsButtonUp(h.button))
                {
                    MouseButtonEventArgs eargs = new MouseButtonEventArgs(mse.X, mse.Y, h.button, timeDown, h.minTime, 1);
                    h.pushed = false;
                    h.fireCount = 0;
                    h.startTime = 0;
                    if (timeDown >= h.minTime)
                    {
                        if (h.OnUp != null)
                        {
                            h.OnUp(eargs);
                        }
                        if (!h.inDrag && inClickBounds)
                        {
                            if (h.OnClick != null)
                            {
                                h.OnClick(eargs);
                            }
                            h.clickTime = time;
                            if ((timeLastClick / 10000.0) <= DoubleClickTimeTolerance)
                            {
                                if (h.OnDoubleClick != null)
                                {
                                    h.OnDoubleClick(eargs);
                                }
                                h.clickTime = 0;
                            }
                        }
                        else {
                            if (h.inDrag && h.OnDragFinish != null)
                            {
                                long dtime = time - h.dragStartTime;
                                h.OnDragFinish(new MouseDragEventArgs(mse.X, mse.Y, h.dragStartX, h.dragStartY, dtime));
                            }
                        }
                    }
                }
                if (h.pushed)
                {
                    if (!justPushed && !inClickBounds && !h.inDrag)
                    {
                        h.dragStartX = h.startX;
                        h.dragStartY = h.startY;
                        if (!h.inDrag && h.OnDragStart != null)
                        {
                            h.OnDragStart(new MouseEventArgs(h.dragStartX, h.dragStartY));
                        }
                        h.inDrag = true;
                        h.dragStartTime = time;
                    }
                    if (timeDown >= h.minTime)
                    {
                        if (h.OnDown != null && (h.fireCount < h.fireLimit || h.fireLimit == 0))
                        {
                            h.fireCount++;
                            h.OnDown(new MouseButtonEventArgs(mse.X, mse.Y, h.button, timeDown, h.minTime, h.fireCount));
                        }
                        if ((timeLastClick / 10000.0) <= DoubleClickTimeTolerance)
                        {
                            if (!h.inDrag)
                            {
                                if (h.OnClickAndDown != null && !h.firedClickDown)
                                {
                                    h.OnClickAndDown(new MouseButtonEventArgs(mse.X, mse.Y, h.button, timeDown, h.minTime, 1));
                                    h.firedClickDown = true;
                                }
                            }
                        }
                        else
                        {
                            h.firedClickDown = false;
                        }
                    }
                }
            }
        }

        private void UpdateMouseScroll(GameTime gt, MouseState mse)
        {
            long time = gt.TotalGameTime.Ticks;
            foreach (MouseScrollHandler hlr in scrollHandlers.Values)
            {
                CheckScrollMovement(hlr, mse, time);
            }
        }

        private void UpdateMouseMovement(GameTime gt, MouseState mse)
        {
            long time = gt.TotalGameTime.Ticks;
            foreach (MouseMoveHandler hlr in moveHandlers.Values)
            {
                CheckMouseMovement(hlr, mse, time);
            }
        }

        private void UpdateKeys(GameTime gt, KeyboardState kb)
        {
            long time = gt.TotalGameTime.Ticks;
            foreach (KeyHandler hlr in keyHandlers.Values)
            {
                long down = time - hlr.startTime;
                CheckKeyState(hlr, kb, down, time);
                CheckKeyDownFire(hlr, down);
            }
        }

        private void CheckScrollMovement(MouseScrollHandler hlr, MouseState mse, long time)
        {
            long timeSinceCheck = time - hlr.lastCheck;
            if (timeSinceCheck >= hlr.checkInterval)
            {
                hlr.lastCheck = time;
                int delta = mse.ScrollWheelValue - hlr.startValue;
                int dist = Math.Abs(delta);
                if (dist >= hlr.minDist && dist != 0)
                {
                    if (delta < 0 && hlr.OnScrollDown != null)
                    {
                        long timeSinceFire = time - hlr.lastFire;
                        hlr.OnScrollDown(new MouseScrollEventArgs(mse.X, mse.Y, mse.ScrollWheelValue, hlr.startValue, timeSinceFire));
                        hlr.startValue = mse.ScrollWheelValue;
                        hlr.lastFire = time;
                    }
                    else if (delta > 0 && hlr.OnScrollUp != null)
                    {
                        long timeSinceFire = time - hlr.lastFire;
                        hlr.OnScrollUp(new MouseScrollEventArgs(mse.X, mse.Y, mse.ScrollWheelValue, hlr.startValue, timeSinceFire));
                        hlr.startValue = mse.ScrollWheelValue;
                        hlr.lastFire = time;
                    }
                }
            }
        }

        private void CheckMouseMovement(MouseMoveHandler handler, MouseState mse, long time)
        {
            long timeSinceCheck = time - handler.lastCheck;
            if (timeSinceCheck >= handler.checkInterval)
            {
                handler.lastCheck = time;
                int distx = Math.Abs(mse.X - handler.startX);
                int disty = Math.Abs(mse.Y - handler.startY);
                double dist = Math.Sqrt((distx * distx) + (disty * disty));
                if (handler.OnMove != null && distx >= handler.minDistX && disty >= handler.minDistY && dist >= handler.minDist && dist > 0)
                {
                    long timeSinceFire = time - handler.lastFire;
                    handler.OnMove(new MouseMoveEventArgs(mse.X, mse.Y, handler.startX, handler.startY, timeSinceFire));
                    handler.startX = mse.X;
                    handler.startY = mse.Y;
                    handler.lastFire = time;
                }
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
