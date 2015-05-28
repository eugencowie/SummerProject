using Artemis;
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
    class PlayerInputSystem : EntityComponentProcessingSystem<PlayerComponent>
    {
        KeyboardState prevKeyboard;
        MouseState prevMouse;

        public override void LoadContent()
        {
            prevKeyboard = Keyboard.GetState();
            prevMouse = Mouse.GetState();
        }

        public override void Process(Entity entity, PlayerComponent playerComponent)
        {
            if (playerComponent.LocalPlayer)
            {
                // Get keyboard and mouse state.
                KeyboardState keyboard = Keyboard.GetState();
                MouseState mouse = Mouse.GetState();

                // Get the screen viewport.
                Viewport viewport = EntitySystem.BlackBoard.GetEntry<Game>("Game").GraphicsDevice.Viewport;

                // Get the camera and player transform.
                TransformComponent playerTransform = entity.GetComponent<TransformComponent>();
                Camera camera = EntitySystem.BlackBoard.GetEntry<Camera>("Camera");

                #region Camera movement

                // Reset camera position to player position when spacebar is pressed.
                if (IsKeyClicked(keyboard, Keys.Space))
                    camera.Position = playerTransform.Position;

                // Camera movement keyboard controls.
                if (keyboard.IsKeyDown(Keys.W))
                    camera.Position.Y -= 1 * (10 - camera.Zoom);
                if (keyboard.IsKeyDown(Keys.S))
                    camera.Position.Y += 1 * (10 - camera.Zoom);
                if (keyboard.IsKeyDown(Keys.A))
                    camera.Position.X -= 1 * (10 - camera.Zoom);
                if (keyboard.IsKeyDown(Keys.D))
                    camera.Position.X += 1 * (10 - camera.Zoom);
                if (keyboard.IsKeyDown(Keys.LeftShift))
                    camera.Zoom += 0.01f;
                if (keyboard.IsKeyDown(Keys.LeftControl))
                    camera.Zoom -= 0.01f;

                // Check the mouse in within the bounds of the window...
                if (mouse.X >= 0 && mouse.X <= viewport.Width &&
                    mouse.Y >= 0 && mouse.Y <= viewport.Height)
                {
                    // Camera movement mouse controls.
                    int screenEdgeBuffer = 60;
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

                #endregion

                // Exit the game when the escape key is pressed (TODO: rebindable keys?).
                if (IsKeyClicked(keyboard, Keys.Escape))
                    EntitySystem.BlackBoard.GetEntry<Game>("Game").Exit();

                // Move the player when the right mouse button is clicked.
                if (IsRightMouseButtonClicked(mouse))
                {
                    // Normalise the mouse coords so that (0,0) is the center instead of the top left.
                    Vector2 mousePos = new Vector2(
                        mouse.X - (viewport.Width / 2.0f),
                        mouse.Y - (viewport.Height / 2.0f));

                    // Figure out the destination (in pixels).
                    Vector2 destination = new Vector2(
                        playerTransform.Position.X + (camera.Position.X - playerTransform.Position.X) + (mousePos.X * (1.0f / camera.Zoom)),
                        playerTransform.Position.Y + (camera.Position.Y - playerTransform.Position.Y) + (mousePos.Y * (1.0f / camera.Zoom)));

                    // Convert destination from pixel coords to block coords.
                    Entity level = entityWorld.TagManager.GetEntity("level");
                    TilemapComponent tilemapComponent = level.GetComponent<TilemapComponent>();
                    int blockSize = tilemapComponent.BlockSize;
                    Vector2 destinationBlock = new Vector2() {
                        X = (int)Math.Round(destination.X / blockSize),
                        Y = (int)Math.Round(destination.Y / blockSize),
                    };

                    // If the player is already moving, stop and go to the new destination instead.
                    if (entity.HasComponent<PlayerMoveAction>())
                        entity.RemoveComponent<PlayerMoveAction>();

                    // The move action tells the movement system to move the player to the specified destination.
                    entity.AddComponent(new PlayerMoveAction() {
                        Destination = destinationBlock,
                        Speed = 4.0f
                    });
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
