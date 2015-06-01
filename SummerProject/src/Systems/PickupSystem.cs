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
            Vector2 position = transform.Position;

            Entity level = entityWorld.TagManager.GetEntity("level");
            Tilemap tilemap = level.GetComponent<Tilemap>();
            int blockSize = tilemap.BlockSize;

            // Convert position and destination from pixel coords to block coords.
            Point positionBlock = new Point() {
                X = (int)Math.Round(position.X / blockSize),
                Y = (int)Math.Round(position.Y / blockSize),
            };

            if (tilemap.Tiles[positionBlock.X, positionBlock.Y].Symbolic == SymbolicBlock.Key)
            {
                inventory.HasKey = true;

                for (int y = 0; y < tilemap.Tiles.GetLength(1); y++)
                {
                    for (int x = 0; x < tilemap.Tiles.GetLength(0); x++)
                    {
                        if (tilemap.Tiles[x, y].Symbolic == SymbolicBlock.LockedDoor) {
                            tilemap.Tiles[x, y].Symbolic = SymbolicBlock.None;
                            tilemap.RecalculateCollisionBlocks();
                        }
                    }
                }
            }
        }
    }
}
