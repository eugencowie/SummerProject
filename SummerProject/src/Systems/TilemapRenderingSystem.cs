using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SummerProject
{
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = 0)]
    class TilemapRenderingSystem : EntityComponentProcessingSystem<RenderableComponent, TilemapComponent>
    {
        ContentManager contentManager;
        SpriteBatch spriteBatch;
        Texture2D ground;
        Texture2D block;

        public override void LoadContent()
        {
            contentManager = BlackBoard.GetEntry<Game>("Game").Content;
            spriteBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            ground = contentManager.Load<Texture2D>("textures/ground");
            block = contentManager.Load<Texture2D>("textures/block");
        }

        public override void Process(Entity entity, RenderableComponent renderableComponent, TilemapComponent tilemapComponent)
        {
            if (!renderableComponent.Hidden)
            {
                for (int y = 0; y < tilemapComponent.Blocks.GetLength(1); y++)
                {
                    for (int x = 0; x < tilemapComponent.Blocks.GetLength(0); x++)
                    {
                        // Use the correct texture for each block.
                        Texture2D texture = null;
                        switch (tilemapComponent.Blocks[x, y]) {
                            case 0: texture = ground; break;
                            case 1: texture = block; break;
                        }

                        if (texture != null)
                        {
                            Vector2 textureOrigin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
                            Rectangle destinationRect = new Rectangle() {
                                X = x * tilemapComponent.BlockSize,
                                Y = y * tilemapComponent.BlockSize,
                                Width = tilemapComponent.BlockSize,
                                Height = tilemapComponent.BlockSize,
                            };

                            // TODO: Fog of war stuff
                            //Entity player = entityWorld.TagManager.GetEntity("player");
                            //Vector2 playerPos = player.GetComponent<TransformComponent>().Position;
                            //Vector2 destPos = new Vector2(destinationRect.X, destinationRect.Y);
                            //if ((playerPos - destPos).Length() < 500)
                            //{
                                spriteBatch.Draw(
                                    texture,
                                    destinationRect,
                                    null,
                                    Color.White,
                                    0.0f,
                                    textureOrigin,
                                    SpriteEffects.None,
                                    1.0f);
                            //}
                        }
                    }
                }
            }
        }
    }
}
