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
    class EnemyRenderingSystem : EntityComponentProcessingSystem<EnemyInfo, Transform>
    {
        ContentManager contentManager;
        SpriteBatch spriteBatch;
        Texture2D enemyTexture;

        public override void LoadContent()
        {
            contentManager = BlackBoard.GetEntry<Game>("Game").Content;
            spriteBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            enemyTexture = contentManager.Load<Texture2D>("textures/objects/enemy");
        }

        public override void Process(Entity entity, EnemyInfo enemyInfo, Transform transform)
        {
            Vector2 textureOrigin = new Vector2(enemyTexture.Width / 2.0f, enemyTexture.Height / 2.0f);

            Vector2 size = new Vector2(enemyTexture.Width, enemyTexture.Height);
            if (transform.Size != Vector2.Zero)
                size = transform.Size;
            Rectangle destinationRect = new Rectangle() {
                X = (int)transform.Position.X,
                Y = (int)transform.Position.Y,
                Width = (int)size.X,
                Height = (int)size.Y,
            };

            spriteBatch.Draw(
                enemyTexture,
                destinationRect,
                null,
                Color.White,
                transform.Rotation,
                textureOrigin,
                SpriteEffects.None,
                0.0f);
        }
    }
}
