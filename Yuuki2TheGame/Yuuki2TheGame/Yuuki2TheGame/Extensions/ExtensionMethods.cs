using Microsoft.Xna.Framework.Input;

namespace Yuuki2TheGame.Extensions
{
    public static class ExtensionMethods
    {

        public static bool IsButtonDown(this MouseState mse, MouseButton button)
        {
            ButtonState check = ButtonState.Pressed;
            bool down = false;
            switch (button)
            {
                default:
                case MouseButton.LEFT:
                    down = (mse.LeftButton == check);
                    break;

                case MouseButton.MIDDLE:
                    down = (mse.MiddleButton == check);
                    break;

                case MouseButton.RIGHT:
                    down = (mse.RightButton == check);
                    break;

                case MouseButton.X1:
                    down = (mse.XButton1 == check);
                    break;

                case MouseButton.X2:
                    down = (mse.XButton2 == check);
                    break;
            }
            return down;
        }

        public static bool IsButtonUp(this MouseState mse, MouseButton button)
        {
            ButtonState check = ButtonState.Released;
            bool down = false;
            switch (button)
            {
                default:
                case MouseButton.LEFT:
                    down = (mse.LeftButton == check);
                    break;

                case MouseButton.MIDDLE:
                    down = (mse.MiddleButton == check);
                    break;

                case MouseButton.RIGHT:
                    down = (mse.RightButton == check);
                    break;

                case MouseButton.X1:
                    down = (mse.XButton1 == check);
                    break;

                case MouseButton.X2:
                    down = (mse.XButton2 == check);
                    break;
            }
            return down;
        }
    }
}
