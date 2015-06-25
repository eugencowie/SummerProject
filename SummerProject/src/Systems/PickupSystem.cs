using System;
using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;

namespace SummerProject
{
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = 0)]
    class PickupSystem : EntityComponentProcessingSystem<PlayerInfo, Inventory, Transform>
    {
        public override void Process(Entity entity, PlayerInfo playerInfo, Inventory inventory, Transform transform)
        {
            // Get the tilemap and block size.
            Tilemap tilemap = entityWorld.TagManager.GetEntity("level").GetComponent<Tilemap>();

            // Convert player position from unit coords (float) to block coords (int).
            var pos = new Point {
                X = (int)Math.Round(transform.Position.X),
                Y = (int)Math.Round(transform.Position.Y)
            };

            // If the player is standing on a key...
            if (tilemap.Tiles[pos.X, pos.Y].Object == ObjectBlock.Key)
            {
                // Remove the key and add it to the player's inventory.
                tilemap.Tiles[pos.X, pos.Y].Object = ObjectBlock.None;
                entityWorld.DeleteEntity(tilemap.Tiles[pos.X, pos.Y].ObjectEntity);
                tilemap.Tiles[pos.X, pos.Y].ObjectEntity = null;
                inventory.HasKey = true;

                // Remove all locked doors.
                for (int y = 0; y < tilemap.Tiles.GetLength(1); y++) {
                    for (int x = 0; x < tilemap.Tiles.GetLength(0); x++) {
                        if (tilemap.Tiles[x, y].Object == ObjectBlock.LockedDoor) {
                            tilemap.Tiles[x, y].Object = ObjectBlock.None;
                            entityWorld.DeleteEntity(tilemap.Tiles[x, y].ObjectEntity);
                            tilemap.Tiles[x, y].ObjectEntity = null;
                            tilemap.RecalculateCollisionBlocks();
                        }
                    }
                }
            }
        }
    }
}
