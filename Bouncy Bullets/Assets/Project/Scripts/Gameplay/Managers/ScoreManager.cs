using UnityEngine;
using Game.Core;

namespace Game.Gameplay
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("Event Channels")]
        [SerializeField] private IntEventChannelSO addScoreEventChannel;
        [SerializeField] private IntEventChannelSO scoreUpdatedEventChannel;
        [SerializeField] private IntEventChannelSO highScoreUpdatedEventChannel;
        [SerializeField] private GameEventChannelSO runStartedEventChannel;

        private int currentScore;
        private int highScore;

        private void OnEnable()
        {
            if (addScoreEventChannel != null) addScoreEventChannel.Subscribe(AddScore);
            if (runStartedEventChannel != null) runStartedEventChannel.Subscribe(HandleRunStarted);
        }

        private void OnDisable()
        {
            if (addScoreEventChannel != null) addScoreEventChannel.Unsubscribe(AddScore);
            if (runStartedEventChannel != null) runStartedEventChannel.Unsubscribe(HandleRunStarted);
        }

        private void HandleRunStarted()
        {
            currentScore = SaveManager.Instance.SavedScore;
            highScore = SaveManager.Instance.HighScore;

            if (scoreUpdatedEventChannel != null) scoreUpdatedEventChannel.RaiseEvent(currentScore);
            if (highScoreUpdatedEventChannel != null) highScoreUpdatedEventChannel.RaiseEvent(highScore);
        }

        private void AddScore(int amount)
        {
            currentScore = Mathf.Max(0, currentScore + amount);
            if (scoreUpdatedEventChannel != null) scoreUpdatedEventChannel.RaiseEvent(currentScore);

            if (currentScore > highScore)
            {
                highScore = currentScore;
                if (highScoreUpdatedEventChannel != null) highScoreUpdatedEventChannel.RaiseEvent(highScore);
            }
        }
    }
}
