using Artemis.Interface;

namespace SummerProject
{
    class PlayerInfo : IComponent
    {
        public int PlayerId;
        public bool LocalPlayer;

        public int Level = 1;
        public int Experience = 0;

        public int BaseHealth = 100;
        public int BaseMana = 100;
    }
}
