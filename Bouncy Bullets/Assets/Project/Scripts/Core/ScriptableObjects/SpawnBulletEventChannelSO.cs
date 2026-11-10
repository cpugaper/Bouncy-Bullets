using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnBulletEventChannelSO", menuName = "Events/Spawn Bullet Event Channel")]
public class SpawnBulletEventChannelSO : ScriptableObject
{
    private Action<BulletType, Vector3, Quaternion> onEventRaised;

    public void Subscribe(Action<BulletType, Vector3, Quaternion> listener) => onEventRaised += listener;
    public void Unsubscribe(Action<BulletType, Vector3, Quaternion> listener) => onEventRaised -= listener;

    public void RaiseEvent(BulletType type, Vector3 position, Quaternion rotation) => onEventRaised?.Invoke(type, position, rotation);

}
