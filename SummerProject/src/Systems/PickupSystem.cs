using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SummerProject
{
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = 0)]
    class PickupSystem : EntityComponentProcessingSystem<Player, Inventory, Transform>
    {
        public override void Process(Entity entity, Player player, Inventory inventory, Transform transform)
        {
            Vector2 position = transform.Position;

            Entity level = entityWorld.TagManager.GetEntity("level");
            Tilemap tilemap = level.GetComponent<Tilemap>();
            SymbolicBlock[,] blocks = tilemap.SymbolicBlocks;
            int blockSize = tilemap.BlockSize;

            // Convert position and destination from pixel coords to block coords.
            Point positionBlock = new Point() {
                X = (int)Math.Round(position.X / blockSize),
                Y = (int)Math.Round(position.Y / blockSize),
            };

            if (blocks[positionBlock.X, positionBlock.Y] == SymbolicBlock.Key)
            {
                inventory.HasKey = true;

                for (int y = 0; y < tilemap.SymbolicBlocks.GetLength(1); y++)
                {
                    for (int x = 0; x < tilemap.SymbolicBlocks.GetLength(0); x++)
                    {
                        if (tilemap.SymbolicBlocks[x, y] == SymbolicBlock.LockedDoor)
                        {
                            tilemap.SymbolicBlocks[x, y] = SymbolicBlock.None;
                            tilemap.RecalculateCollisionBlocks();
                        }
                    }
                }
            }
        }
    }
}
