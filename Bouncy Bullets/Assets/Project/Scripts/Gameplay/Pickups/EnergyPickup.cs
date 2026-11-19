using Game.Core;
using UnityEngine;

namespace Game.Gameplay
{
    public class EnergyPickup : Pickup
    {
        [SerializeField] private float energyAmount = 25f;
        protected override SfxType PickupSfx => SfxType.EnergyPickup;

        protected override bool TryApplyEffect(GameObject player)
        {
            if (player.TryGetComponent(out PlayerTurbo turbo))
            {
                return turbo.TryAddEnergy(energyAmount);
            }
            return false;
        }
    }
}
