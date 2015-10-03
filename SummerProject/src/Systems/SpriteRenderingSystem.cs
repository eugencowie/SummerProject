using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
            if (entity == null || sprite == null || transform == null)
                return;

            if (sprite.Texture != null)
            {
                Vector2 size = new Vector2(sprite.Texture.Width, sprite.Texture.Height);
                Vector2 origin = size / 2f;

                if (transform.Size != Vector2.Zero)
                    size = transform.Size;
                else
                    size = new Vector2(1f, 1f);

                Rectangle destinationRect = new Rectangle {
                    X = (int)(transform.Position.X * Constants.UnitSize),
                    Y = (int)(transform.Position.Y * Constants.UnitSize),
                    Width = (int)(size.X * Constants.UnitSize),
                    Height = (int)(size.Y * Constants.UnitSize)
                };

                // Calculate the sprite layer depth. Enums elements in C# by default are integers
                // starting at zero for the first element and increasing by one until the last
                // element. We calculate the sprite layer depth by dividing the integer value of
                // the specified LayerDepth with the number of elements in the enum, minus one.
                float layerDepth = (int)sprite.LayerDepth / (float)(Enum.GetNames(typeof(LayerDepth)).Length - 1);

                spriteBatch.Draw(
                    sprite.Texture,
                    destinationRect,
                    null,
                    Color.White,
                    transform.Rotation,
                    origin,
                    sprite.Effects,
                    layerDepth);
            }
        }
    }
}
