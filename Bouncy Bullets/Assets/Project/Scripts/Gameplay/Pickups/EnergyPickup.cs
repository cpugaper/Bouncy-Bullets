using UnityEngine;

namespace Game.Gameplay
{
    public class EnergyPickup : Pickup
    {
        [SerializeField] private float energyAmount = 25f;

        protected override bool TryApplyEffect(GameObject player)
        {
            if (player.TryGetComponent(out PlayerTurbo turbo))
            {
                return turbo.TryAddEnergy(energyAmount);
            }
            return false;
        }

        protected override void PlayPickupSound()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayEnergyPickup();
            }
        }
    }
}
