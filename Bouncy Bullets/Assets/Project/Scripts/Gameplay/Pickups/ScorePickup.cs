using UnityEngine;
using Game.Core;

namespace Game.Gameplay
{
    public class ScorePickup : Pickup
    {
        [SerializeField] private int bonusScore = 5;

        [Header("Event Channels")]
        [SerializeField] protected IntEventChannelSO addScoreEventChannel;

        protected override bool TryApplyEffect(GameObject player)
        {
            if (addScoreEventChannel != null)
            {
                addScoreEventChannel.RaiseEvent(bonusScore);
            }
            return true;
        }

        protected override void PlayPickupSound()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayScorePickup();
            }
        }
    }
}
