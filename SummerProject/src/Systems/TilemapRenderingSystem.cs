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
    class TilemapRenderingSystem : EntityComponentProcessingSystem<Tilemap>
    {
        ContentManager contentManager;
        SpriteBatch spriteBatch;
        Texture2D groundTexture;
        Texture2D blockTexture;
        Texture2D unpassableGroundTexture;
        Texture2D lockedDoorTexture;
        Texture2D chestTexture;
        Texture2D keyTexture;

        public override void LoadContent()
        {
            contentManager = BlackBoard.GetEntry<Game>("Game").Content;
            spriteBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            groundTexture = contentManager.Load<Texture2D>("textures/ground");
            blockTexture = contentManager.Load<Texture2D>("textures/block");
            unpassableGroundTexture = contentManager.Load<Texture2D>("textures/unpassable_ground");
            lockedDoorTexture = contentManager.Load<Texture2D>("textures/locked_door");
            chestTexture = contentManager.Load<Texture2D>("textures/chest");
            keyTexture = contentManager.Load<Texture2D>("textures/key");
        }

        public override void Process(Entity entity, Tilemap tilemap)
        {
            for (int y = 0; y < tilemap.VisualBlocks.GetLength(1); y++)
            {
                for (int x = 0; x < tilemap.VisualBlocks.GetLength(0); x++)
                {
                    // Use the correct texture for each block.
                    Texture2D texture = null;
                    switch (tilemap.VisualBlocks[x, y]) {
                        case VisualBlock.Ground: texture = groundTexture; break;
                        case VisualBlock.Wall: texture = blockTexture; break;
                        case VisualBlock.UnpassableGround: texture = unpassableGroundTexture; break;
                    }

                    if (texture != null)
                    {
                        Vector2 textureOrigin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
                        Rectangle destinationRect = new Rectangle() {
                            X = x * tilemap.BlockSize,
                            Y = y * tilemap.BlockSize,
                            Width = tilemap.BlockSize,
                            Height = tilemap.BlockSize,
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
                            MathHelper.ToRadians(tilemap.Rotations[x, y]),
                            textureOrigin,
                            SpriteEffects.None,
                            1.0f);
                        //}
                    }
                }
            }

            for (int y = 0; y < tilemap.SymbolicBlocks.GetLength(1); y++)
            {
                for (int x = 0; x < tilemap.SymbolicBlocks.GetLength(0); x++)
                {
                    // Use the correct texture for each block.
                    Texture2D texture = null;
                    switch (tilemap.SymbolicBlocks[x, y]) {
                        case SymbolicBlock.LockedDoor: texture = lockedDoorTexture; break;
                        case SymbolicBlock.Chest: texture = chestTexture; break;
                        case SymbolicBlock.Key: texture = keyTexture; break;
                    }

                    if (texture != null)
                    {
                        Vector2 textureOrigin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
                        Rectangle destinationRect = new Rectangle() {
                            X = x * tilemap.BlockSize,
                            Y = y * tilemap.BlockSize,
                            Width = tilemap.BlockSize,
                            Height = tilemap.BlockSize,
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
                            MathHelper.ToRadians(tilemap.Rotations[x, y]),
                            textureOrigin,
                            SpriteEffects.None,
                            0.9f);
                        //}
                    }
                }
            }
        }
    }
}
