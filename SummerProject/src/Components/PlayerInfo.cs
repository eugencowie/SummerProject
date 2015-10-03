using Artemis.Interface;

namespace SummerProject
{
    class Trait
    {
        #region Fields

        public int Base;
        public int Modifier;

        private int _current;

        public int Current {
            get { return _current; }
            set { if (value >= 0 && value <= Max) _current = value; }
        }

        public int Max {
            get { return Base + Modifier; }
        }

        public float Percentage {
            get { return (float)_current / Max; }
        }

        #endregion

        public Trait(int _base, int modifier)
        {
            Base = _base;
            Modifier = modifier;
            _current = Max;
        }
    }

    class PlayerInfo : IComponent
    {
        public int PlayerId;
        public bool LocalPlayer;

        public int PlayerType;

        public int Level = 1;
        public int Experience = 0;

        public Trait Health = new Trait(100, 0);
        public Trait Mana = new Trait(100, 0);
    }
}
