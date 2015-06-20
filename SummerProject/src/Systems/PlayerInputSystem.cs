﻿using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SummerProject
{
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
    class PlayerInputSystem : EntityComponentProcessingSystem<PlayerInfo>
    {
        KeyboardState prevKeyboard;
        MouseState prevMouse;

        bool lockCameraToPlayer;

        public override void LoadContent()
        {
            prevKeyboard = Keyboard.GetState();
            prevMouse = Mouse.GetState();
        }

        public override void Process(Entity entity, PlayerInfo playerInfo)
        {
            // Make sure that the game window is the active window.
            if (!BlackBoard.GetEntry<Game>("Game").IsActive)
                return;

            if (playerInfo.LocalPlayer)
            {
                // Get keyboard and mouse state.
                KeyboardState keyboard = Keyboard.GetState();
                MouseState mouse = Mouse.GetState();

                // Get the screen viewport.
                Viewport viewport = BlackBoard.GetEntry<Game>("Game").GraphicsDevice.Viewport;

                // Get the camera and player transform.
                Transform playerTransform = entity.GetComponent<Transform>();
                Camera camera = BlackBoard.GetEntry<Camera>("Camera");

                #region Debug networking testing

                if (Client.Active && IsKeyClicked(keyboard, Keys.F9))
                    if (Client.Instance != null)
                        Client.Instance.SendIsReadyMessage();

                #endregion

                #region Camera movement

                // Switch between free camera and locked to player.
                if (IsKeyClicked(keyboard, Keys.Space))
                    lockCameraToPlayer = !lockCameraToPlayer;

                if (lockCameraToPlayer)
                {
                    // Lock camera to player.
                    camera.Position = playerTransform.Position;
                }
                else
                {
                    // Camera movement keyboard controls.
                    if (keyboard.IsKeyDown(Keys.W))
                        camera.Position.Y -= 1 * (10 - camera.Zoom);
                    if (keyboard.IsKeyDown(Keys.S))
                        camera.Position.Y += 1 * (10 - camera.Zoom);
                    if (keyboard.IsKeyDown(Keys.A))
                        camera.Position.X -= 1 * (10 - camera.Zoom);
                    if (keyboard.IsKeyDown(Keys.D))
                        camera.Position.X += 1 * (10 - camera.Zoom);

                    // Check the mouse in within the bounds of the window...
                    if (mouse.X >= 0 && mouse.X <= viewport.Width &&
                        mouse.Y >= 0 && mouse.Y <= viewport.Height)
                    {
                        // Camera movement mouse controls.
                        const int screenEdgeBuffer = 60;
                        if (mouse.Y < screenEdgeBuffer)
                            camera.Position.Y -= 1 * (10 - camera.Zoom);
                        if (mouse.Y > viewport.Height - screenEdgeBuffer)
                            camera.Position.Y += 1 * (10 - camera.Zoom);
                        if (mouse.X < screenEdgeBuffer)
                            camera.Position.X -= 1 * (10 - camera.Zoom);
                        if (mouse.X > viewport.Width - screenEdgeBuffer)
                            camera.Position.X += 1 * (10 - camera.Zoom);
                        if (IsMouseScolledUp(mouse))
                            camera.Zoom += 0.1f;
                        if (IsMouseScrollDown(mouse))
                            camera.Zoom -= 0.1f;
                    }
                }

                // Camera zoom.
                if (keyboard.IsKeyDown(Keys.LeftShift))
                    camera.Zoom += 0.01f;
                if (keyboard.IsKeyDown(Keys.LeftControl))
                    camera.Zoom -= 0.01f;

                // Check the mouse in within the bounds of the window...
                if (mouse.X >= 0 && mouse.X <= viewport.Width &&
                    mouse.Y >= 0 && mouse.Y <= viewport.Height)
                {
                    if (IsMouseScolledUp(mouse))
                        camera.Zoom += 0.1f;
                    if (IsMouseScrollDown(mouse))
                        camera.Zoom -= 0.1f;
                }

                #endregion

                // Exit the game when the escape key is pressed (TODO: rebindable keys?).
                if (IsKeyClicked(keyboard, Keys.Escape))
                    BlackBoard.GetEntry<Game>("Game").Exit();

                // TODO: remove this from final version.
                if (keyboard.IsKeyDown(Keys.F5))
                    playerInfo.Health.Current -= 1;
                if (keyboard.IsKeyDown(Keys.F6))
                    playerInfo.Health.Current += 1;

                // Normalise the mouse coords so that (0,0) is the center instead of the top left.
                var mousePos = new Vector2(
                    mouse.X - (viewport.Width / 2f),
                    mouse.Y - (viewport.Height / 2f));

                // Figure out the destination (in pixels).
                var destination = new Vector2(
                    playerTransform.Position.X + (camera.Position.X - playerTransform.Position.X) + (mousePos.X * (1f / camera.Zoom)),
                    playerTransform.Position.Y + (camera.Position.Y - playerTransform.Position.Y) + (mousePos.Y * (1f / camera.Zoom)));

                // Make player always face the mouse pointer.
                Vector2 direction = playerTransform.Position - destination;
                playerTransform.Rotation = (float)Math.Atan2(direction.Y, direction.X) - ((float)Math.PI / 2f);

                // Move the player when the right mouse button is clicked.
                if (IsRightMouseButtonClicked(mouse))
                {
                    // Convert destination from pixel coords to block coords.
                    var destinationBlock = new Vector2 {
                        X = (int)Math.Round(destination.X / Constants.UnitSize),
                        Y = (int)Math.Round(destination.Y / Constants.UnitSize),
                    };

                    // If the player is already moving, stop and go to the new destination instead.
                    if (entity.HasComponent<MoveAction>())
                        entity.RemoveComponent<MoveAction>();

                    // The move action tells the movement system to move the player to the specified destination.
                    entity.AddComponent(new MoveAction {
                        Destination = destinationBlock,
                        Speed = 4f
                    });

                    if (Client.Instance != null)
                        Client.Instance.SendMoveMessage((int)destinationBlock.X, (int)destinationBlock.Y);
                }

                prevKeyboard = keyboard;
                prevMouse = mouse;
            }
        }

        #region Keyboard and mouse helper methods

        private bool IsKeyClicked(KeyboardState keyboard, Keys key)
        {
            return (keyboard.IsKeyUp(key) &&
                prevKeyboard.IsKeyDown(key));
        }

        private bool IsLeftMouseButtonClicked(MouseState mouse)
        {
            return (mouse.LeftButton == ButtonState.Released &&
                prevMouse.LeftButton == ButtonState.Pressed);
        }

        private bool IsMiddleMouseButtonClicked(MouseState mouse)
        {
            return (mouse.MiddleButton == ButtonState.Released &&
                prevMouse.MiddleButton == ButtonState.Pressed);
        }

        private bool IsRightMouseButtonClicked(MouseState mouse)
        {
            return (mouse.RightButton == ButtonState.Released &&
                prevMouse.RightButton == ButtonState.Pressed);
        }

        private bool IsMouseXButton1Clicked(MouseState mouse)
        {
            return (mouse.XButton1 == ButtonState.Released &&
                prevMouse.XButton1 == ButtonState.Pressed);
        }

        private bool IsMouseXButton2Clicked(MouseState mouse)
        {
            return (mouse.XButton2 == ButtonState.Released &&
                prevMouse.XButton2 == ButtonState.Pressed);
        }

        private bool IsMouseScolledUp(MouseState mouse)
        {
            return (mouse.ScrollWheelValue > prevMouse.ScrollWheelValue);
        }

        private bool IsMouseScrollDown(MouseState mouse)
        {
            return (mouse.ScrollWheelValue < prevMouse.ScrollWheelValue);
        }

        #endregion
    }
}
