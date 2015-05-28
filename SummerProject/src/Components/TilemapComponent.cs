using Artemis.Interface;

namespace SummerProject
{
    enum VisualBlock {
        None,
        Ground,
        UnpassableGround,
        LockedGround,
        LockedDoor,
        Wall,
    }

    enum SymbolicBlock {
        None,
        PlayerStart,
        General,
        Boss,
        Mob,
        MobSpawn,
        Chest,
        Trap,
        HealthPack,
        Button,
    }

    class TilemapComponent : IComponent
    {
        public VisualBlock[,] VisualBlocks;
        public SymbolicBlock[,] SymbolicBlocks;
        public AStar.TileInfo[,] CollisionBlocks;
        public int BlockSize;
    }
}
