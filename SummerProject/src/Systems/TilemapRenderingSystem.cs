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
                    Texture2D[] textures = new Texture2D[2] { null, null };

                    // Set base texture.
                    switch (tilemap.Tiles[x, y].Base) {
                        case BaseBlock.Ground: textures[0] = groundTexture; break;
                        case BaseBlock.Wall: textures[0] = blockTexture; break;
                        case BaseBlock.UnpassableGround: textures[0] = unpassableGroundTexture; break;
                    }

                    // Set object texture.
                    switch (tilemap.Tiles[x, y].Object) {
                        case ObjectBlock.LockedDoor: textures[1] = lockedDoorTexture; break;
                        case ObjectBlock.Chest: textures[1] = chestTexture; break;
                        case ObjectBlock.Key: textures[1] = keyTexture; break;
                    }

                    // TODO: refactor this
                    for (int i = 0; i < textures.Length; i++)
                    {
                        if (textures[i] != null)
                        {
                            Vector2 textureOrigin = new Vector2(textures[i].Width / 2.0f, textures[i].Height / 2.0f);
                            Rectangle destinationRect = new Rectangle() {
                                X = x * tilemap.BlockSize,
                                Y = y * tilemap.BlockSize,
                                Width = tilemap.BlockSize,
                                Height = tilemap.BlockSize,
                            };

                            float layerDepth = 1.0f;
                            float rotation = 0.0f;
                            SpriteEffects effect = SpriteEffects.None;
                            switch (i)
                            {
                                case 0:
                                    layerDepth = 1.0f;
                                    rotation = tilemap.Tiles[x, y].BaseRotation;
                                    effect = tilemap.Tiles[x, y].BaseEffect;
                                    break;

                                case 1:
                                    layerDepth = 0.9f;
                                    rotation = tilemap.Tiles[x, y].ObjectRotation;
                                    effect = tilemap.Tiles[x, y].ObjectEffect;
                                    break;
                            }

                            // TODO: Fog of war stuff
                            //Entity player = entityWorld.TagManager.GetEntity("player");
                            //Vector2 playerPos = player.GetComponent<Transform>().Position;
                            //Vector2 destPos = new Vector2(destinationRect.X, destinationRect.Y);
                            //if ((playerPos - destPos).Length() < 500)
                            //{
                            spriteBatch.Draw(
                                textures[i],
                                destinationRect,
                                null,
                                Color.White,
                                rotation,
                                textureOrigin,
                                effect,
                                layerDepth);
                            //}
                        }
                    }
                }
            }
        }
    }
}
