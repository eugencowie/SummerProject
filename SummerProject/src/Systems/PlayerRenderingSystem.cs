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
    class PlayerRenderingSystem : EntityComponentProcessingSystem<RenderableComponent, PlayerComponent, TransformComponent>
    {
        ContentManager contentManager;
        SpriteBatch spriteBatch;
        Texture2D player;

        public override void LoadContent()
        {
            contentManager = BlackBoard.GetEntry<Game>("Game").Content;
            spriteBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            player = contentManager.Load<Texture2D>("textures/player");
        }

        public override void Process(Entity entity, RenderableComponent renderableComponent, PlayerComponent playerComponent, TransformComponent transformComponent)
        {
            if (!renderableComponent.Hidden)
            {
                Vector2 textureOrigin = new Vector2(player.Width / 2.0f, player.Height / 2.0f);
                Rectangle destinationRect = new Rectangle() {
                    X = (int)transformComponent.Position.X,
                    Y = (int)transformComponent.Position.Y,
                    Width = (int)transformComponent.Size.X,
                    Height = (int)transformComponent.Size.Y,
                };

                spriteBatch.Draw(
                    player,
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
}
