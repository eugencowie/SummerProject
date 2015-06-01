using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using System;

namespace SummerProject
{
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = 0)]
    class PickupSystem : EntityComponentProcessingSystem<PlayerInfo, Inventory, Transform>
    {
        public override void Process(Entity entity, PlayerInfo playerInfo, Inventory inventory, Transform transform)
        {
            // Get the tilemap and block size.
            Tilemap tilemap = entityWorld.TagManager.GetEntity("level").GetComponent<Tilemap>();
            int blockSize = tilemap.BlockSize;

            // Convert player position from pixel coords to block coords.
            Point position = new Point() {
                X = (int)Math.Round(transform.Position.X / blockSize),
                Y = (int)Math.Round(transform.Position.Y / blockSize),
            };

            // If the player is standing on a key...
            if (tilemap.Tiles[position.X, position.Y].Object == ObjectBlock.Key)
            {
                // Remove the key and add it to the player's inventory.
                tilemap.Tiles[position.X, position.Y].Object = ObjectBlock.None;
                inventory.HasKey = true;

                // Remove all locked doors.
                for (int y = 0; y < tilemap.Tiles.GetLength(1); y++) {
                    for (int x = 0; x < tilemap.Tiles.GetLength(0); x++) {
                        if (tilemap.Tiles[x, y].Object == ObjectBlock.LockedDoor) {
                            tilemap.Tiles[x, y].Object = ObjectBlock.None;
                            tilemap.RecalculateCollisionBlocks();
                        }
                    }
                }
            }
        }
    }
}
