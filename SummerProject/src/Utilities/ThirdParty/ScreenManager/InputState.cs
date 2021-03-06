// ReSharper disable All

#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SummerProject
{
    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        public const int MaxInputs = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;
        public MouseState CurrentMouseState;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;
        public MouseState LastMouseState;

        public readonly bool[] GamePadWasConnected;
        

        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];
            CurrentMouseState = new MouseState();

            LastKeyboardStates = new KeyboardState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];
            LastMouseState = new MouseState();

            GamePadWasConnected = new bool[MaxInputs];
        }


        /// <summary>
        /// Reads the latest state user input.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastGamePadStates[i] = CurrentGamePadStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (CurrentGamePadStates[i].IsConnected)
                    GamePadWasConnected[i] = true;
            }

            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }


        /// <summary>
        /// Helper for checking if a key was pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsKeyDown(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return CurrentKeyboardStates[i].IsKeyDown(key);
            }
            else
            {
                // Accept input from any player.
                return (IsKeyDown(key, PlayerIndex.One,   out playerIndex) ||
                        IsKeyDown(key, PlayerIndex.Two,   out playerIndex) ||
                        IsKeyDown(key, PlayerIndex.Three, out playerIndex) ||
                        IsKeyDown(key, PlayerIndex.Four,  out playerIndex));
            }
        }


        /// <summary>
        /// Helper for checking if a button was pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsButtonDown(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return CurrentGamePadStates[i].IsButtonDown(button);
            }
            else
            {
                // Accept input from any player.
                return (IsButtonDown(button, PlayerIndex.One,   out playerIndex) ||
                        IsButtonDown(button, PlayerIndex.Two,   out playerIndex) ||
                        IsButtonDown(button, PlayerIndex.Three, out playerIndex) ||
                        IsButtonDown(button, PlayerIndex.Four,  out playerIndex));
            }
        }


        /// <summary>
        /// Helper for checking if a key was newly pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsKeyClicked(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key) &&
                        LastKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (IsKeyClicked(key, PlayerIndex.One,   out playerIndex) ||
                        IsKeyClicked(key, PlayerIndex.Two,   out playerIndex) ||
                        IsKeyClicked(key, PlayerIndex.Three, out playerIndex) ||
                        IsKeyClicked(key, PlayerIndex.Four,  out playerIndex));
            }
        }


        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsButtonClicked(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonDown(button) &&
                        LastGamePadStates[i].IsButtonUp(button));
            }
            else
            {
                // Accept input from any player.
                return (IsButtonClicked(button, PlayerIndex.One,   out playerIndex) ||
                        IsButtonClicked(button, PlayerIndex.Two,   out playerIndex) ||
                        IsButtonClicked(button, PlayerIndex.Three, out playerIndex) ||
                        IsButtonClicked(button, PlayerIndex.Four,  out playerIndex));
            }
        }


        public bool IsLeftMouseButtonClicked()
        {
            return (CurrentMouseState.LeftButton == ButtonState.Pressed &&
                    LastMouseState.LeftButton == ButtonState.Released);
        }


        public bool IsMiddleMouseButtonClicked()
        {
            return (CurrentMouseState.MiddleButton == ButtonState.Pressed &&
                    LastMouseState.MiddleButton == ButtonState.Released);
        }


        public bool IsRightMouseButtonClicked()
        {
            return (CurrentMouseState.RightButton == ButtonState.Pressed &&
                    LastMouseState.RightButton == ButtonState.Released);
        }


        public bool IsMouseWheelScolledUp()
        {
            return (CurrentMouseState.ScrollWheelValue > LastMouseState.ScrollWheelValue);
        }


        public bool IsMouseWheelScrolledDown()
        {
            return (CurrentMouseState.ScrollWheelValue < LastMouseState.ScrollWheelValue);
        }
    }
}
