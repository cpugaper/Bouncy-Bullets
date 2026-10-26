using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Event Channels")]
    [SerializeField] private IntEventChannelSO scoreUpdatedEventChannel;
    [SerializeField] private IntEventChannelSO highScoreUpdatedEventChannel;

    private const string ScoreFormat = "D4";

    private void OnEnable()
    {
        if (scoreUpdatedEventChannel != null) scoreUpdatedEventChannel.Subscribe(UpdateScoreText);
        if (highScoreUpdatedEventChannel != null) highScoreUpdatedEventChannel.Subscribe(UpdateHighScoreText);

    }

    private void OnDisable()
    {
        if (scoreUpdatedEventChannel != null) scoreUpdatedEventChannel.Unsubscribe(UpdateScoreText);
        if (highScoreUpdatedEventChannel != null) highScoreUpdatedEventChannel.Unsubscribe(UpdateHighScoreText);

    }

    private void UpdateScoreText(int newScore)
    {
        if (scoreText != null) scoreText.text = newScore.ToString(ScoreFormat);
    }

    private void UpdateHighScoreText(int newScore)
    {
        if (highScoreText != null) highScoreText.text = newScore.ToString(ScoreFormat);
    }
}
