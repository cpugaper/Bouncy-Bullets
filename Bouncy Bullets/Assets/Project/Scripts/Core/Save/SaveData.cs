using System;

namespace Game.Core
{
    [Serializable]
    public class SaveData
    {
        public int highScore;
        public bool hasSavedGame;
        public int savedScore;
        public int savedPhaseIndex;
    }
}
