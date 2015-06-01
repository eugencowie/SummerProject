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
            lockedDoorTexture = contentManager.Load<Texture2D>("textures/objects/locked_door");
            chestTexture = contentManager.Load<Texture2D>("textures/objects/chest");
            keyTexture = contentManager.Load<Texture2D>("textures/objects/key");
        }

        public override void Process(Entity entity, Tilemap tilemap)
        {
            for (int y = 0; y < tilemap.Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < tilemap.Tiles.GetLength(0); x++)
                {
                    // Use the correct texture for each block.
                    Texture2D texture = null;
                    switch (tilemap.Tiles[x, y].Base) {
                        case BaseBlock.Ground: texture = groundTexture; break;
                        case BaseBlock.Wall: texture = blockTexture; break;
                        case BaseBlock.UnpassableGround: texture = unpassableGroundTexture; break;
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
                            tilemap.Tiles[x, y].BaseRotation,
                            textureOrigin,
                            tilemap.Tiles[x, y].BaseEffect,
                            1.0f);
                        //}
                    }

                    // Use the correct texture for each block.
                    texture = null;
                    switch (tilemap.Tiles[x, y].Object) {
                        case ObjectBlock.LockedDoor: texture = lockedDoorTexture; break;
                        case ObjectBlock.Chest: texture = chestTexture; break;
                        case ObjectBlock.Key: texture = keyTexture; break;
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
                            tilemap.Tiles[x, y].ObjectRotation,
                            textureOrigin,
                            tilemap.Tiles[x, y].ObjectEffect,
                            0.9f);
                        //}
                    }
                }
            }
        }
    }
}
