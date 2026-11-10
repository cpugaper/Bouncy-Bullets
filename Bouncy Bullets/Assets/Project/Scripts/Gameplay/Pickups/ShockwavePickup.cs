using UnityEngine;

namespace Game.Gameplay
{
    public class ShockwavePickup : Pickup
    {
        protected override bool TryApplyEffect(GameObject player)
        {
            if (player.TryGetComponent(out PlayerShockwave shockwave))
            {
                return shockwave.TryAddShockwave();
            }
            return false;
        }

        protected override void PlayPickupSound()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayShockwavePickup();
            }
        }
    }
}
