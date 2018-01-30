using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraxAwesomeEngine.Core
{
    enum MouseKeys
    {
        LEFT,
        RIGHT,
        MIDDLE,
        BTN_4,
        BTN_5,
        INVALID_KEY
    }

    /// <summary>
    /// This class uses (not extends) <see cref="Microsoft.Xna.Framework.Input"></see>
    /// adding some Unity like methods
    /// </summary>
    [MoonSharpUserData]
    static class InputManager
    {

        private class MouseButtonsState
        {
            public ButtonState LEFT { get; protected set; }
            public ButtonState RIGHT { get; protected set; }
            public ButtonState MIDDLE { get; protected set; }
            public ButtonState MOUSE_4 { get; protected set; }
            public ButtonState MOUSE_5 { get; protected set; }
            public float ScrollWheel { get; protected set; }
            public Vector2 MousePosition { get; protected set; }
            public Vector2 Delta { get; protected set; }

            public void SetState(MouseState state)
            {

                this.LEFT = state.LeftButton;
                this.RIGHT = state.RightButton;
                this.MIDDLE = state.MiddleButton;
                this.MOUSE_4 = state.XButton1;
                this.MOUSE_5 = state.XButton2;
                this.ScrollWheel = state.ScrollWheelValue;
                this.MousePosition = new Vector2(state.X, state.Y);
            
            }

            public ButtonState this[MouseKeys index]
            {
                get
                {
                    switch (index)
                    {
                        case MouseKeys.LEFT:    return LEFT;
                        case MouseKeys.MIDDLE:  return MIDDLE;
                        case MouseKeys.RIGHT:   return RIGHT;
                        case MouseKeys.BTN_4:   return MOUSE_4;
                        case MouseKeys.BTN_5:   return MOUSE_5;
                        default:                return LEFT;
                    }
                }
            }

        }
        
        private static KeyboardState _lastFrameKeyboardState;
        private static MouseButtonsState _lastFrameMouseState;

        private static KeyboardState _actualFrameKeyboardState;
        private static MouseButtonsState _actualFrameMouseState;
        

        internal static void InitInputManager()
        {
            _lastFrameMouseState = new MouseButtonsState();
            _actualFrameMouseState = new MouseButtonsState();
        }

        internal static void UpdateInputBegin()
        {
            _actualFrameKeyboardState = Keyboard.GetState();

            //Updating my custom MouseButtonsState
            _actualFrameMouseState.SetState(Mouse.GetState());


        }

        public static bool KeyHasBeenPressed(Keys key)
        {
            return (_actualFrameKeyboardState.IsKeyDown(key) && _lastFrameKeyboardState.IsKeyUp(key));
        }

        public static bool KeyIsHeld(Keys key)
        {
            return _actualFrameKeyboardState.IsKeyDown(key);
        }

        public static bool KeyIsUp(Keys key)
        {
            return _actualFrameKeyboardState.IsKeyUp(key);
        }

        public static bool KeyHasBeenReleased(Keys key)
        {
            return (_actualFrameKeyboardState.IsKeyUp(key) && _lastFrameKeyboardState.IsKeyDown(key));
        }

        public static bool MouseKeyHasBeenPressed(MouseKeys key)
        {
            return (_actualFrameMouseState[key] == ButtonState.Pressed && _lastFrameMouseState[key] == ButtonState.Released);
        }

        public static bool MouseKeyIsHeld(MouseKeys key)
        {
            return _actualFrameMouseState[key] == ButtonState.Pressed;
        }

        public static bool MouseKeyIsUp(MouseKeys key)
        {
            return _actualFrameMouseState[key] == ButtonState.Released;
        }

        public static bool MouseKeyHasBeenReleased(MouseKeys key)
        {
            return (_actualFrameMouseState[key] == ButtonState.Released && _lastFrameMouseState[key] == ButtonState.Pressed);
        }

        public static float ScrollWheel
        {
            get => _actualFrameMouseState.ScrollWheel;
        }
        public static Vector2 MousePosition
        {
            get => _actualFrameMouseState.MousePosition;
        }

        internal static void UpdateInputEnd()
        {

            _lastFrameKeyboardState = _actualFrameKeyboardState;
            _lastFrameMouseState = _actualFrameMouseState;
            _lastFrameMouseState.SetState(Mouse.GetState());
        }

    }
}
