using UnityEngine;

namespace Game.Core
{
    [CreateAssetMenu(fileName = "PositionEventChannelSO", menuName = "Events/Position Event Channel")]
    public class PositionEventChannelSO : EventChannelSO<Vector3> { }
}