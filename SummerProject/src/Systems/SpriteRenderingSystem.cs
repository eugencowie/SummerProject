using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SummerProject
{
    /// <summary>
    /// System responsible for rendering any entity which has both a Sprite component and Transform component.
    /// </summary>
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = 0)]
    class SpriteRenderingSystem : EntityComponentProcessingSystem<Sprite, Transform>
    {
        SpriteBatch spriteBatch;

        public override void LoadContent()
        {
            spriteBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
        }

        public override void Process(Entity entity, Sprite sprite, Transform transform)
        {
            if (sprite.Texture != null)
            {
                Vector2 size = new Vector2(sprite.Texture.Width, sprite.Texture.Height);
                Vector2 origin = size / 2f;

                if (transform.Size != Vector2.Zero)
                    size = transform.Size;

                Rectangle destinationRect = new Rectangle {
                    X = (int)transform.Position.X,
                    Y = (int)transform.Position.Y,
                    Width = (int)size.X,
                    Height = (int)size.Y
                };

                spriteBatch.Draw(
                    sprite.Texture,
                    destinationRect,
                    null,
                    Color.White,
                    transform.Rotation,
                    origin,
                    sprite.Effects,
                    sprite.LayerDepth);
            }
        }
    }
}
