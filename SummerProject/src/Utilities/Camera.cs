// ReSharper disable All

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SummerProject
{
    class Camera
    {
        public Vector2 Position;
        public float Rotation;

        Matrix transform;
        float zoom;

        #region Getters/setters

        public float Zoom
        {
            get { return zoom; }
            set { zoom = value; if (zoom < 0.1f) zoom = 0.1f; } // Negative zoom will flip image
        }

        #endregion
 
        public Camera()
        {
            Position = Vector2.Zero;
            Rotation = 0.0f;
            zoom = 1.0f;
        }

        public void Move(Vector2 amount)
        {
            Position += amount;
        }

        public Matrix GetTransformationMatrix(GraphicsDevice graphicsDevice)
        {
            transform =
                Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));

            return transform;
        }
    }
}
