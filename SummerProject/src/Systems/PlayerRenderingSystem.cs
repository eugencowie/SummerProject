using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SummerProject
{
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = 0)]
    class PlayerRenderingSystem : EntityComponentProcessingSystem<PlayerComponent, TransformComponent>
    {
        ContentManager contentManager;
        SpriteBatch spriteBatch;
        Texture2D playerTexture;

        public override void LoadContent()
        {
            contentManager = BlackBoard.GetEntry<Game>("Game").Content;
            spriteBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            playerTexture = contentManager.Load<Texture2D>("textures/player");
        }

        public override void Process(Entity entity, PlayerComponent playerComponent, TransformComponent transformComponent)
        {
            Vector2 textureOrigin = new Vector2(playerTexture.Width / 2.0f, playerTexture.Height / 2.0f);

            Vector2 size = new Vector2(playerTexture.Width, playerTexture.Height);
            if (transformComponent.Size != Vector2.Zero)
                size = transformComponent.Size;
            Rectangle destinationRect = new Rectangle() {
                X = (int)transformComponent.Position.X,
                Y = (int)transformComponent.Position.Y,
                Width = (int)size.X,
                Height = (int)size.Y,
            };

            spriteBatch.Draw(
                playerTexture,
                destinationRect,
                null,
                Color.White,
                transformComponent.Rotation,
                textureOrigin,
                SpriteEffects.None,
                0.0f);
        }
    }
}
