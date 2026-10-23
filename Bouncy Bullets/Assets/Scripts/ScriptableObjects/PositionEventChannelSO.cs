using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PositionEventChannelSO", menuName = "Events/Position Event Channel")]
public class PositionEventChannelSO : ScriptableObject
{
    private Action<Vector3> OnEventRaised;

    public void Subscribe(Action<Vector3> listener) => OnEventRaised += listener;
    public void Unsubscribe(Action<Vector3> listener) => OnEventRaised -= listener;

    public void RaiseEvent(Vector3 position) => OnEventRaised?.Invoke(position);
}
