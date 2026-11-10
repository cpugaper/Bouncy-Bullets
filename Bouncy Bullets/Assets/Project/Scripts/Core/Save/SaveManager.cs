using UnityEngine;

namespace Game.Core
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        [Header("Event Channels")]
        [SerializeField] private IntEventChannelSO scoreUpdatedEventChannel;
        [SerializeField] private IntEventChannelSO highScoreUpdatedEventChannel;
        [SerializeField] private IntEventChannelSO phaseChangedEventChannel;
        [SerializeField] private PositionEventChannelSO playerDeathEventChannel;

        private SaveData data;
        private int runScore;
        private bool isHighScoreDirty;

        public bool HasSavedGame => data.hasSavedGame;
        public int HighScore => data.highScore;
        public int SavedScore => data.savedScore;
        public int SavedPhaseIndex => data.savedPhaseIndex;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            data = SaveSystem.Load();
        }

        private void OnEnable()
        {
            if (scoreUpdatedEventChannel != null) scoreUpdatedEventChannel.Subscribe(HandleScoreUpdated);
            if (highScoreUpdatedEventChannel != null) highScoreUpdatedEventChannel.Subscribe(HandleHighScoreUpdated);
            if (phaseChangedEventChannel != null) phaseChangedEventChannel.Subscribe(HandlePhaseChanged);
            if (playerDeathEventChannel != null) playerDeathEventChannel.Subscribe(HandlePlayerDeath);
        }

        private void OnDisable()
        {
            if (scoreUpdatedEventChannel != null) scoreUpdatedEventChannel.Unsubscribe(HandleScoreUpdated);
            if (highScoreUpdatedEventChannel != null) highScoreUpdatedEventChannel.Unsubscribe(HandleHighScoreUpdated);
            if (phaseChangedEventChannel != null) phaseChangedEventChannel.Unsubscribe(HandlePhaseChanged);
            if (playerDeathEventChannel != null) playerDeathEventChannel.Unsubscribe(HandlePlayerDeath);
        }

        public void BeginRun(bool continueSavedGame)
        {
            if (continueSavedGame && data.hasSavedGame)
            {
                runScore = data.savedScore;
                return;
            }

            data.hasSavedGame = false;
            data.savedScore = 0;
            data.savedPhaseIndex = 0;
            runScore = 0;

            WriteToDisk();
        }

        private void HandleScoreUpdated(int score) => runScore = score;

        private void HandlePlayerDeath(Vector3 position)
        {
            data.hasSavedGame = false;
            data.savedScore = 0;
            data.savedPhaseIndex = 0;
            WriteToDisk();
        }

        private void HandleHighScoreUpdated(int highScore)
        {
            if (highScore <= data.highScore) return;
            data.highScore = highScore;
            isHighScoreDirty = true;
        }

        private void HandlePhaseChanged(int phaseNumber)
        {
            // phaseNumber is 1-based; convert it to a 0-based array index.
            data.savedPhaseIndex = phaseNumber - 1;
            data.savedScore = runScore;
            data.hasSavedGame = true;
            WriteToDisk();
        }

        private void SaveHighScoreIfDirty()
        {
            if (isHighScoreDirty) WriteToDisk();
        }

        private void WriteToDisk()
        {
            SaveSystem.Save(data);
            isHighScoreDirty = false;
        }

        private void OnApplicationPause(bool paused) { if (paused) SaveHighScoreIfDirty(); }

        private void OnDestroy()
        {
            SaveHighScoreIfDirty();
            if (Instance == this) Instance = null;
        }
    }
}
