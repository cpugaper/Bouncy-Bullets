using Game.Core;
using UnityEngine;

namespace Game.Gameplay
{
    public class ShockwavePickup : Pickup
    {
        protected override SfxType PickupSfx => SfxType.ShockwavePickup;

        protected override bool TryApplyEffect(GameObject player)
        {
            if (player.TryGetComponent(out PlayerShockwave shockwave))
            {
                return shockwave.TryAddShockwave();
            }
            return false;
        }
    }
}
